using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Crimson
{
    public partial class MainForm : Form
    {
        readonly World _world = new World();
        RenderSystem renderSystem;

        public MainForm()
        {
            InitializeComponent();

            renderSystem = new RenderSystem(_world, mainPanel, mapPanel);
            var movementSystem = new MovementSystem(_world);
            var inputSystem = new InputSystem(_world);
            var cameraSystem = new CameraSystem(_world);
            _world.AddSystem(renderSystem);
            _world.AddSystem(movementSystem);
            _world.AddSystem(inputSystem);
            _world.AddSystem(cameraSystem);

            Image playerImage = ResizeImage(Properties.Resources.Player, 64, 64);
            var player = _world.CreateEntity();
            player.AddComponent(new CKeyboardNavigation());
            player.AddComponent(new CMovement((5, 5), (0, 0)));
            player.AddComponent(new CPosition(mainPanel.Width / 2, mainPanel.Height / 2));
            player.AddComponent(new CGraphics(playerImage));
            player.AddComponent(new GameObject());

            var camera = _world.CreateEntity();
            camera.AddComponent(new CCamera(20, player.Entity, (30*64, 30*64), (mapPanel.Width, mapPanel.Height)));
            camera.AddComponent(new CPosition(mainPanel.Width / 2 + 1, mainPanel.Height / 2 + 1));

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
                    tile.AddComponent(new CPosition(i * tileSize, j * tileSize));

                    Image rawImage;
                    switch (rnd.Next(3))
                    {
                        case 0:
                            rawImage = Properties.Resources.ground;
                            switch (rnd.Next(2))
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
                                case 4:
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
            tree.AddComponent(new GameObject());
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
            ke.Remove(e.KeyCode);
            if (ke.Count == 0)
            {
                moveTimer.Enabled = false;
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

        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            _world.ForEachEntityWithComponents<CKeyboardNavigation>(en => _world.AddComponentToEntity(en, new CInput(ke.Values.ToList())));
        }
    }
}
