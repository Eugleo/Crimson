using Crimson.Components;
using Crimson.Entities;
using Crimson.Systems;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Crimson
{
    public partial class MainForm : Form
    {
        readonly Map _map;
        readonly World _world = new World();
        readonly EntityHandle _player;
        readonly EntityHandle _camera;
        readonly Dictionary<CGun.ShootingPattern, CGun> _guns = new Dictionary<CGun.ShootingPattern, CGun>();
        readonly Dictionary<CGun.ShootingPattern, Label> _gunLabels = new Dictionary<CGun.ShootingPattern, Label>();
        readonly EntityGroup<CTransform, CTile> _tiles;

        public MainForm()
        {
            InitializeComponent();

            _keyboardNavigable = _world.GetGroup<EntityGroup<CKeyboardNavigation>>();
            _tiles = _world.GetGroup<EntityGroup<CTransform, CTile>>();

            DefineGuns();
            _map = LevelGenerator.Generate(_world, 30, 30, 64);
            var freeLocations = _tiles
                .Where(t => !t.Item3.Occupied)
                .Select(t => t.Item2.Location)
                .Where(t => t.X > mapPanel.Width / 2 && t.X < 30 * 64 - mapPanel.Width / 2 && t.Y > mapPanel.Height / 2 && t.Y < 30 * 64 - mapPanel.Height / 2)
                .ToList();
            var location = freeLocations[rnd.Next(freeLocations.Count)];
            _player = MakePlayer(location);
            AddSystemsToWorld();
            _camera = MakeCamera(location);
            AddSpawner();

            gameTimer.Enabled = true;
        }

        void AddSpawner()
        {
            var spawner = _world.CreateEntity();
            spawner.AddComponent(new CSpawner(5500));
        }

        void DefineGuns()
        {
            _guns[CGun.ShootingPattern.Pistol] = new CGun(CGun.ShootingPattern.Pistol, 18, 1000, 5, 600, 35, 250, 8);
            _gunLabels[CGun.ShootingPattern.Pistol] = gun1Label;
            _guns[CGun.ShootingPattern.Shotgun] = new CGun(CGun.ShootingPattern.Shotgun, 8, 1800, 0, 900, 30, 200, 2);
            _gunLabels[CGun.ShootingPattern.Shotgun] = gun2Label;
            _guns[CGun.ShootingPattern.SMG] = new CGun(CGun.ShootingPattern.SMG, 10, 1200, 3, 150, 40, 400, 30);
            _gunLabels[CGun.ShootingPattern.SMG] = gun3Label;
            _guns[CGun.ShootingPattern.Grenade] = new CGun(CGun.ShootingPattern.Grenade, 3, 4000, 0, 400, 15, 500, 1);
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
            _world.AddSystem(new SpawnerSystem(_world, _player));
            _world.AddSystem(new MetaSystem(_world, PlayerDied, KilledEnemy));
        }

        EntityHandle MakePlayer(Vector location)
        {
            Image playerImage = Utilities.ResizeImage(Properties.Resources.Player, 64, 64);
            var player = _world.CreateEntity();
            player.AddComponent(new CKeyboardNavigation());
            player.AddComponent(new CMovement(6, new Vector(0, 0)));
            player.AddComponent(new CTransform(location));
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

        EntityHandle MakeCamera(Vector location)
        {
            var camera = _world.CreateEntity();
            camera.AddComponent(new CCamera(20, _player, (mapPanel.Width, mapPanel.Height)));
            camera.AddComponent(new CTransform(location));
            camera.AddComponent(new CMovement(5, new Vector(0, 0)));
            return camera;
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

        void UpdateGunLabel(Label label, CGun gun)
        {
            if (gun.IsBeingReloaded)
            {
                label.BackColor = Color.DarkGray;
            }
            else
            {
                label.BackColor = Color.MidnightBlue;
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            _world.Tick();
            killCounterBar.Maximum = KILL_COUNT;
            killCounterBar.Val = KILL_COUNT - killCounter;

            if (_isShooting)
            {
                var absoluteCenterLocation = _camera.GetComponent<CTransform>().Location;
                var relativeOffset = new Vector(mapPanel.Width / 2, mapPanel.Height / 2);
                var targetLocation = _mouseLocation + absoluteCenterLocation - relativeOffset;
                _player.AddComponent(new CShootEvent(targetLocation));
            }

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

            _gunLabels.Values.ToList().ForEach(l => l.ForeColor = Color.White);
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
                _gunLabels[gun.Type].ForeColor = Color.LightBlue;
            }

            if (_player.TryGetComponent(out CHealth health))
            {
                healthBar.Maximum = health.MaxHealth;
                healthBar.Val = (int)Math.Round(health.CurrentHealth);
            }
        }

        readonly EntityGroup<CKeyboardNavigation> _keyboardNavigable;
        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            ke.Remove(e.KeyCode);

            foreach (var (entity, _) in _keyboardNavigable)
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
            var absoluteCenterLocation = _camera.GetComponent<CTransform>().Location;
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

        uint ticks;
        private void FpsTimer_Tick(object sender, EventArgs e)
        {
            ticks += 1;
            if (ticks > 5)
            {
                enemiesLeftLabel.Hide();
                ammoLabel.Hide();
                healthLabel.Hide();
                weaponLabel.Hide();
            }
        }

        readonly int KILL_COUNT = 15;
        int killCounter = 0;
        void KilledEnemy()
        {
            killCounter += 1;

            if (killCounter >= KILL_COUNT)
            {
                EndGame(true);
            }
        }

        void PlayerDied()
        {
            EndGame(false);
        }

        void EndGame(bool won)
        {
            fpsTimer.Enabled = false;
            gameTimer.Enabled = false;
            _camera.RemoveComponent<CMovement>();

            string message;
            if (won)
            {
                message = string.Format("You WIN! You fought for: {0}min {1}sec.", ticks / 60, ticks % 60);
            }
            else
            {
                message = string.Format("You LOSE! You struggled for: {0}min {1}sec", ticks / 60, ticks % 60);
            }

            var frm = new Endgame(message)
            {
                Location = Location,
                StartPosition = FormStartPosition.Manual
            };
            frm.FormClosing += delegate { Close(); };
            frm.Show();
            Hide();
        }
    }
}
