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

            renderSystem = new RenderSystem(_world, this);
            var movementSystem = new MovementSystem(_world);
            var inputSystem = new InputSystem(_world);
            _world.AddSystem(renderSystem);
            _world.AddSystem(movementSystem);
            _world.AddSystem(inputSystem);

            Image playerImage = ResizeImage(Properties.Resources.PlayerCharacter, 64, 64);
            var player = _world.CreateEntity();
            player.SetComponent(new CKeyboardNavigation());
            player.SetComponent(new CMovement((5, 5), (0, 0)));
            player.SetComponent(new CPosition(100, 100));
            player.SetComponent(new CGraphics(playerImage));

            MakeTree(630, 200);
            MakeTree(240, 310);
            MakeTree(315, 115);

            gameTimer.Enabled = true;
        }

        void MakeTree(int X, int Y)
        {
            Image treeImage = ResizeImage(Properties.Resources.Tree, 64, 64);
            var tree = _world.CreateEntity();
            tree.SetComponent(new CPosition(X, Y));
            tree.SetComponent(new CGraphics(treeImage));
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
