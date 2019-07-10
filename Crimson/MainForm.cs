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
        readonly World _world = new World();
        readonly EntityHandle _player;
        readonly Dictionary<CGun.GunType, CGun> _guns = new Dictionary<CGun.GunType, CGun>();

        public MainForm()
        {
            InitializeComponent();

            _guns[CGun.GunType.Pistol] = new CGun(CGun.GunType.Pistol, 15, 700, 5, 300, 30);
            _guns[CGun.GunType.Shotgun] = new CGun(CGun.GunType.Shotgun, 20, 800, 0, 700, 20);
            _guns[CGun.GunType.SMG] = new CGun(CGun.GunType.SMG, 10, 700, 15, 45, 30);

            var renderSystem = new RenderSystem(_world, mainPanel, mapPanel);
            var movementSystem = new MovementSystem(_world);
            var inputSystem = new InputSystem(_world);
            var cameraSystem = new CameraSystem(_world);
            var gunSystem = new GunSystem(_world);
            _world.AddSystem(inputSystem);
            _world.AddSystem(cameraSystem);
            _world.AddSystem(movementSystem);
            _world.AddSystem(gunSystem);
            _world.AddSystem(renderSystem);

            Image playerImage = ResizeImage(Properties.Resources.Player, 64, 64);
            _player = _world.CreateEntity();
            _player.AddComponent(new CKeyboardNavigation());
            _player.AddComponent(new CMovement(5, new Vector(0, 0)));
            _player.AddComponent(new CTransform(mainPanel.Width / 2, mainPanel.Height / 2));
            _player.AddComponent(new CGraphics(playerImage));
            _player.AddComponent(new CGameObject());
            _player.AddComponent(_guns[CGun.GunType.Pistol]);

            var camera = _world.CreateEntity();
            camera.AddComponent(new CCamera(20, _player.Entity, (30*64, 30*64), (mapPanel.Width, mapPanel.Height)));
            camera.AddComponent(new CTransform(mainPanel.Width / 2 + 1, mainPanel.Height / 2 + 1));

            MakeMap(30, 30, 64);

            gameTimer.Enabled = true;
        }

        readonly Random rnd = new Random();
        void MakeMap(int w, int h, int tileSize)
        {
            foreach (var i in Enumerable.Range(0, h))
            {
                foreach (var j in Enumerable.Range(0, w))
                {
                    var tile = _world.CreateEntity();
                    tile.AddComponent(new CTile());
                    tile.AddComponent(new CTransform(i * tileSize, j * tileSize));

                    Image rawImage;
                    switch (rnd.Next(3))
                    {
                        case 0:
                            rawImage = Properties.Resources.ground;
                            switch (rnd.Next(3))
                            {
                                case 0:
                                    MakeTree(i * tileSize, j * tileSize);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            rawImage = Properties.Resources.Grass;
                            switch (rnd.Next(5))
                            {
                                case 0:
                                    MakeTree(i * tileSize, j * tileSize);
                                    break;
                                default:
                                    break;
                            }
                            break;
                    }

                    Image image = ResizeImage(rawImage, tileSize, tileSize);
                    tile.AddComponent(new CGraphics(image));
                }
            }
        }

        void MakeTree(int X, int Y)
        {
            Image treeImage;
            switch (rnd.Next(3))
            {
                case 0:
                    treeImage = ResizeImage(Properties.Resources.baobab, 64, 64);
                    break;
                default:
                    treeImage = ResizeImage(Properties.Resources.smrk, 64, 64);
                    break;
            }
            var tree = _world.CreateEntity();
            tree.AddComponent(new CTransform(X, Y));
            tree.AddComponent(new CGraphics(treeImage));
            tree.AddComponent(new CGameObject());
        }

        readonly Dictionary<Keys, KeyEventArgs> ke = new Dictionary<Keys, KeyEventArgs>();
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            ke[e.KeyCode] = e;
            if (ke.Count > 0)
            {
                _world.ForEachEntityWithComponents<CKeyboardNavigation>(en => 
                    _world.SetComponentOfEntity(en, new CInputEvent(ke.Values.ToList()))
                );
            }
            switch (e.KeyCode)
            {
                case Keys.D1:
                    _player.AddComponent(_guns[CGun.GunType.Pistol]);
                    break;
                case Keys.D2:
                    _player.AddComponent(_guns[CGun.GunType.Shotgun]);
                    break;
                case Keys.D3:
                    _player.AddComponent(_guns[CGun.GunType.SMG]);
                    break;
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            _world.Tick();
            if (_isShooting)
            {
                var absoluteCenterLocation = _world.GetFilter<EntityFilter<CCamera, CTransform>>().Components2[0].Location;
                var relativeOffset = new Vector(mapPanel.Width / 2, mapPanel.Height / 2);
                _player.AddComponent(new CShootEvent(_mouseLocation + absoluteCenterLocation - relativeOffset));
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            // TODO upravit, ted předpokládám moc věcí
            ke.Remove(e.KeyCode);
            _world.ForEachEntityWithComponents<CKeyboardNavigation>(en => _world.SetComponentOfEntity(en, new CInputEvent(ke.Values.ToList())));
            if (ke.Count == 0)
            {
                _world.ForEachEntityWithComponents<CKeyboardNavigation>(entity => {
                    _world.RemoveComponentFromEntity<CInputEvent>(entity);
                    var move = _world.GetComponentForEntity<CMovement>(entity);
                    _world.SetComponentOfEntity(entity, new CMovement(move.Speed, new Vector(0, 0)));
                });
            }
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
            }
            return destImage;
        }

        bool _isShooting = false;
        Vector _mouseLocation;
        private void MainPanel_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseLocation = Vector.FromPoint(e.Location);
            _isShooting = true;
            var absoluteCenterLocation = _world.GetFilter<EntityFilter<CCamera, CTransform>>().Components2[0].Location;
            var relativeOffset = new Vector(mapPanel.Width / 2, mapPanel.Height / 2);
            _player.AddComponent(new CShootEvent(Vector.FromPoint(e.Location) + absoluteCenterLocation - relativeOffset));
        }

        private void MainPanel_MouseUp(object sender, MouseEventArgs e)
        {
            _isShooting = false;
            _player.RemoveComponent<CShootEvent>();
        }

        private void MainPanel_MouseMove(object sender, MouseEventArgs e)
        {
            _mouseLocation = Vector.FromPoint(e.Location);
        }
    }
}
