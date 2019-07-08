using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace Crimson
{
    public partial class MainForm : Form
    {
        readonly World _world = new World();
        readonly EntityHandle _player;

        public MainForm()
        {
            InitializeComponent();

            var renderSystem = new RenderSystem(_world, mainPanel, mapPanel);
            var movementSystem = new MovementSystem(_world);
            var inputSystem = new InputSystem(_world);
            var cameraSystem = new CameraSystem(_world);
            var gunSystem = new GunSystem(_world);
            _world.AddSystem(renderSystem);
            _world.AddSystem(movementSystem);
            _world.AddSystem(inputSystem);
            _world.AddSystem(cameraSystem);
            _world.AddSystem(gunSystem);

            Image playerImage = ResizeImage(Properties.Resources.Player, 64, 64);
            _player = _world.CreateEntity();
            _player.AddComponent(new CKeyboardNavigation());
            _player.AddComponent(new CMovement((5, 5), (0, 0)));
            _player.AddComponent(new CPosition(mainPanel.Width / 2, mainPanel.Height / 2));
            _player.AddComponent(new CGraphics(playerImage));
            _player.AddComponent(new CGameObject());
            _player.AddComponent(new CHasGun(CHasGun.GunType.Pistol));

            var camera = _world.CreateEntity();
            camera.AddComponent(new CCamera(20, _player.Entity, (30*64, 30*64), (mapPanel.Width, mapPanel.Height)));
            camera.AddComponent(new CPosition(mainPanel.Width / 2 + 1, mainPanel.Height / 2 + 1));

            MakeMap(30, 30, 64);

            gameTimer.Enabled = true;
            _entitiesWithInput = _world.GetFilter<EntityFilter<CKeyboardNavigation, CInputEvent>>();
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
                    tile.AddComponent(new CPosition(i * tileSize, j * tileSize));

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
            tree.AddComponent(new CPosition(X, Y));
            tree.AddComponent(new CGraphics(treeImage));
            tree.AddComponent(new CGameObject());
        }

        readonly Dictionary<Keys, KeyEventArgs> ke = new Dictionary<Keys, KeyEventArgs>();
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            ke[e.KeyCode] = e;
            if (ke.Count > 0)
            {
                moveTimer.Enabled = true;
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            _world.Tick();
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            // TODO upravit, ted předpokládám moc věcí
            ke.Remove(e.KeyCode);
            if (ke.Count == 0)
            {
                moveTimer.Enabled = false;
                _world.ForEachEntityWithComponents<CKeyboardNavigation>(entity => {
                    _world.RemoveComponentFromEntity<CInputEvent>(entity);
                    var move = _world.GetComponentForEntity<CMovement>(entity);
                    _world.AddComponentToEntity(entity, new CMovement(move.Speed, (0, 0)));
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

        EntityFilter<CKeyboardNavigation, CInputEvent> _entitiesWithInput;
        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("Count = {0}", ke.Count);
            _world.ForEachEntityWithComponents<CKeyboardNavigation>(en => _world.AddComponentToEntity(en, new CInputEvent(ke.Values.ToList())));
        }

        private void MainPanel_MouseClick(object sender, MouseEventArgs e)
        {
            var (X, Y) = _world.GetFilter<EntityFilter<CCamera, CPosition>>().Components2[0].Coords;
            var (relX, relY) = (mapPanel.Width / 2, mapPanel.Height / 2);
            _player.AddComponent(new CShootEvent((e.X - relX + X, e.Y - relY + Y)));
        }
    }
}
