using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using Crimson.Components;
using Crimson.Systems;
using Crimson.Entities;

namespace Crimson
{
    public partial class MainForm : Form
    {
        readonly Map _map;
        readonly World _world = new World();
        readonly EntityHandle _player;
        readonly Dictionary<CGun.ShootingPattern, CGun> _guns = new Dictionary<CGun.ShootingPattern, CGun>();
        readonly Dictionary<CGun.ShootingPattern, Label> _gunLabels = new Dictionary<CGun.ShootingPattern, Label>();

        public MainForm()
        {
            InitializeComponent();

            group = _world.GetGroup<EntityGroup<CKeyboardNavigation>>();

            DefineGuns();
            _map = LevelGenerator.Generate(_world, 30, 30, 64);
            AddSystemsToWorld();
            _player = MakePlayer();
            MakeCamera();

            gameTimer.Enabled = true;
        }

        void DefineGuns()
        {
            _guns[CGun.ShootingPattern.Pistol] = new CGun(CGun.ShootingPattern.Pistol, 15, 1000, 5, 600, 35, 350, 8);
            _gunLabels[CGun.ShootingPattern.Pistol] = gun1Label;
            _guns[CGun.ShootingPattern.Shotgun] = new CGun(CGun.ShootingPattern.Shotgun, 20, 1600, 0, 900, 30, 200, 2);
            _gunLabels[CGun.ShootingPattern.Shotgun] = gun2Label;
            _guns[CGun.ShootingPattern.SMG] = new CGun(CGun.ShootingPattern.SMG, 10, 500, 3, 150, 40, 400, 30);
            _gunLabels[CGun.ShootingPattern.SMG] = gun3Label;
            _guns[CGun.ShootingPattern.Grenade] = new CGun(CGun.ShootingPattern.Grenade, 3, 3000, 0, 400, 15, 600, 1);
            _gunLabels[CGun.ShootingPattern.Grenade] = gun4Label;
        }

        void AddSystemsToWorld()
        {
            _world.AddSystem(new InputSystem(_world));
            _world.AddSystem(new MovementSystem(_world, _map));
            _world.AddSystem(new CameraSystem(_world, _map));
            _world.AddSystem(new PursuitSystem(_world));
            _world.AddSystem(new AvoidObstaclesSystem(_world));
            _world.AddSystem(new AttackSystem(_world));
            _world.AddSystem(new GunSystem(_world));
            _world.AddSystem(new MeleeSystem(_world));
            _world.AddSystem(new CollisionResolverSystem(_world));
            _world.AddSystem(new HealthSystem(_world));
            _world.AddSystem(new BulletSystem(_world));
            _world.AddSystem(new RenderSystem(_world, mapPanel));
            _world.AddSystem(new FireSystem(_world, _map));
            _world.AddSystem(new WaterSystem(_world, _map));
            _world.AddSystem(new SteamSystem(_world));
            _world.AddSystem(new MetaSystem(_world));
        }

        EntityHandle MakePlayer()
        {
            Image playerImage = Utilities.ResizeImage(Properties.Resources.Player, 64, 64);
            var player = _world.CreateEntity();
            player.AddComponent(new CKeyboardNavigation());
            player.AddComponent(new CMovement(6, new Vector(0, 0)));
            player.AddComponent(new CTransform(mapPanel.Width / 2, mapPanel.Height / 2));
            player.AddComponent(new CGraphics(playerImage));
            player.AddComponent(new CGameObject());
            player.AddComponent(_guns[CGun.ShootingPattern.Pistol]);
            player.AddComponent(new CCollidable(32));
            player.AddComponent(new CFaction(Faction.PC));
            player.AddComponent(new CHealth(150, 150));
            player.AddComponent(new CFlammable(Utilities.ResizeImage(Properties.Resources.ohen, 64, 64)));
            player.AddComponent(new CSumbergable(Utilities.ResizeImage(Properties.Resources.water, 64, 64)));
            return player;
        }

        void MakeCamera()
        {
            var camera = _world.CreateEntity();
            camera.AddComponent(new CCamera(20, _player, (mapPanel.Width, mapPanel.Height)));
            camera.AddComponent(new CTransform(mapPanel.Width / 2 + 1, mapPanel.Height / 2 + 1));
            camera.AddComponent(new CMovement(5, new Vector(0, 0)));
        }

        readonly Random rnd = new Random();

        readonly Dictionary<Keys, KeyEventArgs> ke = new Dictionary<Keys, KeyEventArgs>();
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            ke[e.KeyCode] = e;
            if (ke.Count > 0)
            {
                _world.ForEachEntityWithComponents<CKeyboardNavigation>(en => 
                    _world.AddComponentToEntity(en, new CInputEvent(ke.Values.ToList()))
                );
            }
            switch (e.KeyCode)
            {
                case Keys.D1:
                    _player.AddComponent(_guns[CGun.ShootingPattern.Pistol]);
                    break;
                case Keys.D2:
                    _player.AddComponent(_guns[CGun.ShootingPattern.Shotgun]);
                    break;
                case Keys.D3:
                    _player.AddComponent(_guns[CGun.ShootingPattern.SMG]);
                    break;
                case Keys.D4:
                    _player.AddComponent(_guns[CGun.ShootingPattern.Grenade]);
                    break;
                case Keys.U:
                    MakeEnemy();
                    break;
                case Keys.Escape:
                    Pause();
                    break;
            }
        }

        void Pause()
        {
            if (gameTimer.Enabled)
            {
                gameTimer.Enabled = false;
                pauseLabel.Show();
            }
            else
            {
                gameTimer.Enabled = true;
                pauseLabel.Hide();
            }
        }

        void MakeEnemy()
        {
            var enemy = _world.CreateEntity();
            Image image = Utilities.ResizeImage(Properties.Resources.enemy, 64, 64);
            enemy.AddComponent(new CTransform(rnd.Next(5, 64 * 30 - 5 - 64), rnd.Next(5, 64 * 30 - 5 - 64)));
            enemy.AddComponent(new CGraphics(image));
            enemy.AddComponent(new CGameObject());
            enemy.AddComponent(new CMovement(5, new Vector(0, 0)));
            enemy.AddComponent(new CPursuitBehavior(_player, 5, 1, 30));
            enemy.AddComponent(new CCollidable(32));
            enemy.AddComponent(new CFaction(Faction.NPC));
            enemy.AddComponent(new CHealth(50, 50));
            enemy.AddComponent(new CAvoidObstaclesBehavior(5, 2, MakeFeelers(new int[] { 0, 20, 40, 60, 80 })));
            enemy.AddComponent(new CAttacker(_player));
            enemy.AddComponent(new CHasMeleeWeapon(70, 300, 100, true));
            enemy.AddComponent(new CFlammable(Utilities.ResizeImage(Properties.Resources.ohen, 64, 64)));
            enemy.AddComponent(new CSumbergable(Utilities.ResizeImage(Properties.Resources.water, 64, 64)));
        }

        List<(EntityHandle, int)> MakeFeelers(int[] distances)
        {
            var acc = new List<(EntityHandle, int)>();
            foreach (var d in distances)
            {
                acc.Add((MakeFeeler(new Vector(32, 0)), d));
                acc.Add((MakeFeeler(new Vector(32, 64)), d));
                acc.Add((MakeFeeler(new Vector(64, 32)), d));
                acc.Add((MakeFeeler(new Vector(0, 32)), d));
            }
            return acc;
        }

        EntityHandle MakeFeeler(Vector offset)
        {
            var feeler = _world.CreateEntity();
            feeler.AddComponent(new CMovement(0, new Vector(0, 0)));
            feeler.AddComponent(new CTransform(0, 0));
            feeler.AddComponent(new CFeeler(offset));
            return feeler;
        }

        void UpdateGunLabel(Label label, CGun gun)
        {
            if (gun.IsBeingReloaded)
            {
                label.ForeColor = Color.LightGray;
            }
            else
            {
                label.ForeColor = Color.Black;
            }
        }

        uint ticks;
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            _world.Tick();
            ticks += 1;

            foreach (var g in _guns.Values)
            {
                switch (g.Type)
                {
                    case CGun.ShootingPattern.Pistol:
                        UpdateGunLabel(gun1Label, g);
                        break;
                    case CGun.ShootingPattern.Shotgun:
                        UpdateGunLabel(gun2Label, g);
                        break;
                    case CGun.ShootingPattern.SMG:
                        UpdateGunLabel(gun3Label, g);
                        break;
                    case CGun.ShootingPattern.Grenade:
                        UpdateGunLabel(gun4Label, g);
                        break;
                }
            }

            _gunLabels.Values.ToList().ForEach(l => l.BackColor = Color.Transparent);
            if (_player.TryGetComponent(out CGun gun))
            {
                if (gun.IsBeingReloaded)
                {
                    reloadingLabel.Show();
                }
                else
                {
                    reloadingLabel.Hide();
                }
                ammoBar.Maximum = gun.MagazineSize;
                ammoBar.Val = gun.Ammo;
                _gunLabels[gun.Type].BackColor = Color.DarkGreen;
            }

            if (_player.TryGetComponent(out CHealth health))
            {
                healthBar.Maximum = health.MaxHealth;
                healthBar.Val = (int)Math.Round(health.CurrentHealth);
            }

            if (_isShooting)
            {
                var absoluteCenterLocation = _world.GetGroup<EntityGroup<CCamera, CMovement, CTransform>>().Components3[0].Location;
                var relativeOffset = new Vector(mapPanel.Width / 2, mapPanel.Height / 2);
                var targetLocation = _mouseLocation + absoluteCenterLocation - relativeOffset;
                _player.AddComponent(new CShootEvent(targetLocation));
            }
        }

        readonly EntityGroup<CKeyboardNavigation> group;
        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            ke.Remove(e.KeyCode);

            foreach (var (entity, _) in group)
            {
                if (ke.Count == 0)
                {
                    entity.RemoveComponent<CInputEvent>();
                    if (entity.TryGetComponent(out CMovement movement)) { movement.Velocity = new Vector(0, 0); }
                }
                else
                {
                    var keList = ke.Values.ToList();
                    if (entity.TryGetComponent(out CInputEvent input))
                    {
                        input.KeyEventArgs = keList;
                    }
                    else
                    {
                        entity.AddComponent(new CInputEvent(keList));
                    }
                }
            }
        }

        bool _isShooting = false;
        Vector _mouseLocation;
        private void MapPanel_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseLocation = Vector.FromPoint(e.Location);
            _isShooting = true;
            var absoluteCenterLocation = _world.GetGroup<EntityGroup<CCamera, CMovement, CTransform>>().Components3[0].Location;
            var relativeOffset = new Vector(mapPanel.Width / 2, mapPanel.Height / 2);
            _player.AddComponent(new CShootEvent(Vector.FromPoint(e.Location) + absoluteCenterLocation - relativeOffset));
        }

        private void MapPanel_MouseUp(object sender, MouseEventArgs e)
        {
            _isShooting = false;
            _player.RemoveComponent<CShootEvent>();
        }

        private void MapPanel_MouseMove(object sender, MouseEventArgs e)
        {
            _mouseLocation = Vector.FromPoint(e.Location);
        }

        private void FpsTimer_Tick(object sender, EventArgs e)
        {
            Text = string.Format("FPS: {0}", ticks);
            ticks = 0;
        }
    }
}
