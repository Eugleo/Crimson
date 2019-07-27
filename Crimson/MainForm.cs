using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using Crimson.Components;
using Crimson.Systems;
using Crimson.Entities;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Crimson
{
    public partial class MainForm : Form
    {
        Map map;
        readonly World _world = new World();
        readonly EntityHandle _player;
        readonly Dictionary<CGun.ShootingPattern, CGun> _guns = new Dictionary<CGun.ShootingPattern, CGun>();

        public MainForm()
        {
            InitializeComponent();

            _guns[CGun.ShootingPattern.Pistol] = new CGun(CGun.ShootingPattern.Pistol, 15, 700, 5, 300, 25, 350);
            _guns[CGun.ShootingPattern.Shotgun] = new CGun(CGun.ShootingPattern.Shotgun, 20, 800, 0, 700, 20, 200);
            _guns[CGun.ShootingPattern.SMG] = new CGun(CGun.ShootingPattern.SMG, 10, 700, 2, 50, 30, 600);

            map = LevelGenerator.Generate(_world, 30, 30, 64);

            _world.AddSystem(new InputSystem(_world));
            _world.AddSystem(new MovementSystem(_world, map));
            _world.AddSystem(new CameraSystem(_world, map));
            _world.AddSystem(new PursuitSystem(_world));
            _world.AddSystem(new AvoidObstaclesSystem(_world));
            _world.AddSystem(new AttackSystem(_world));
            _world.AddSystem(new GunSystem(_world));
            _world.AddSystem(new MeleeSystem(_world));
            _world.AddSystem(new CollisionResolverSystem(_world));
            _world.AddSystem(new HealthSystem(_world));
            _world.AddSystem(new BulletSystem(_world));
            _world.AddSystem(new RenderSystem(_world, mainPanel, mapPanel));
            _world.AddSystem(new FireSystem(_world, map));
            _world.AddSystem(new WaterSystem(_world, map));
            _world.AddSystem(new SteamSystem(_world));
            _world.AddSystem(new MetaSystem(_world));

            Image playerImage = ResizeImage(Properties.Resources.Player, 64, 64);
            _player = _world.CreateEntity();
            _player.AddComponent(new CKeyboardNavigation());
            _player.AddComponent(new CMovement(6, new Vector(0, 0)));
            _player.AddComponent(new CTransform(mainPanel.Width / 2, mainPanel.Height / 2));
            _player.AddComponent(new CGraphics(playerImage));
            _player.AddComponent(new CGameObject());
            _player.AddComponent(_guns[CGun.ShootingPattern.Pistol]);
            _player.AddComponent(new CCollidable(32));
            _player.AddComponent(new CFaction(Faction.PC));
            _player.AddComponent(new CHealth(150, 150));
            _player.AddComponent(new CFlammable(ResizeImage(Properties.Resources.ohen, 64, 64)));
            _player.AddComponent(new CSumbergable(ResizeImage(Properties.Resources.water, 64, 64)));

            var camera = _world.CreateEntity();
            camera.AddComponent(new CCamera(20, _player, (mapPanel.Width, mapPanel.Height)));
            camera.AddComponent(new CTransform(mainPanel.Width / 2 + 1, mainPanel.Height / 2 + 1));
            camera.AddComponent(new CMovement(5, new Vector(0, 0)));

            gameTimer.Enabled = true;
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
                case Keys.U:
                    var enemy = _world.CreateEntity();
                    Image image = ResizeImage(Properties.Resources.enemy, 64, 64);
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
                    enemy.AddComponent(new CMeleeWeapon(70, 300, 100, true));
                    enemy.AddComponent(new CFlammable(ResizeImage(Properties.Resources.ohen, 64, 64)));
                    enemy.AddComponent(new CSumbergable(ResizeImage(Properties.Resources.water, 64, 64)));
                    break;
            }
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
            Image image = ResizeImage(Properties.Resources.Player, 40, 40);
            var feeler = _world.CreateEntity();
            feeler.AddComponent(new CMovement(0, new Vector(0, 0)));
            feeler.AddComponent(new CTransform(0, 0));
            //feeler.AddComponent(new CGraphics(image));
            //feeler.AddComponent(new CGameObject());
            feeler.AddComponent(new CFeeler(offset));
            return feeler;
        }

        uint ticks;
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // TODO refaktorovat
            ticks += _world.Tick();

            /*
            var g = _world.GetComponentForEntity<CGraphics>(_player.Entity);
            var up = new Vector(1, 0);
            var dir = (targetLocation - (_world.GetComponentForEntity<CTransform>(_player.Entity).Location + new Vector(g.Image.Width / 2, g.Image.Height / 2))).Normalized();

            var angle = -Math.Atan2(dir.X * up.Y - dir.Y * up.X, dir.X * up.X + dir.Y * up.Y) * (180 / Math.PI);
            //_player.AddComponent(new CGraphics(RotateImage(g.OriginalImage, (float)angle, true, false, Color.Transparent)) { OriginalImage = g.OriginalImage });

            */
            if (_isShooting)
            {
                var absoluteCenterLocation = _world.GetGroup<EntityGroup<CCamera, CMovement, CTransform>>().Components3[0].Location;
                var relativeOffset = new Vector(mapPanel.Width / 2, mapPanel.Height / 2);
                var targetLocation = _mouseLocation + absoluteCenterLocation - relativeOffset;
                _player.AddComponent(new CShootEvent(targetLocation));
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            // TODO upravit, ted předpokládám moc věcí
            ke.Remove(e.KeyCode);
            _world.ForEachEntityWithComponents<CKeyboardNavigation>(en => _world.AddComponentToEntity(en, new CInputEvent(ke.Values.ToList())));
            if (ke.Count == 0)
            {
                _world.ForEachEntityWithComponents<CKeyboardNavigation>(entity => {
                    _world.RemoveComponentFromEntity<CInputEvent>(entity);
                    if (_world.EntityHasComponent<CMovement>(entity))
                    {
                        var movement = _world.GetComponentForEntity<CMovement>(entity);
                        _world.AddComponentToEntity(entity, new CMovement(movement.MaxSpeed, new Vector(0, 0)));
                    }
                });
            }
        }

        public static Image RotateImage(Image inputImage, float angleDegrees, bool upsizeOk,
                                   bool clipOk, Color backgroundColor)
        {
            // Test for zero rotation and return a clone of the input image
            if (angleDegrees == 0f)
                return (Bitmap)inputImage.Clone();

            // Set up old and new image dimensions, assuming upsizing not wanted and clipping OK
            int oldWidth = inputImage.Width;
            int oldHeight = inputImage.Height;
            int newWidth = oldWidth;
            int newHeight = oldHeight;
            float scaleFactor = 1f;

            // If upsizing wanted or clipping not OK calculate the size of the resulting bitmap
            if (upsizeOk || !clipOk)
            {
                double angleRadians = angleDegrees * Math.PI / 180d;

                double cos = Math.Abs(Math.Cos(angleRadians));
                double sin = Math.Abs(Math.Sin(angleRadians));
                newWidth = (int)Math.Round(oldWidth * cos + oldHeight * sin);
                newHeight = (int)Math.Round(oldWidth * sin + oldHeight * cos);
            }

            // If upsizing not wanted and clipping not OK need a scaling factor
            if (!upsizeOk && !clipOk)
            {
                scaleFactor = Math.Min((float)oldWidth / newWidth, (float)oldHeight / newHeight);
                newWidth = oldWidth;
                newHeight = oldHeight;
            }

            // Create the new bitmap object. If background color is transparent it must be 32-bit, 
            //  otherwise 24-bit is good enough.
            Bitmap newBitmap = new Bitmap(newWidth, newHeight, backgroundColor == Color.Transparent ?
                                             PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb);
            newBitmap.SetResolution(inputImage.HorizontalResolution, inputImage.VerticalResolution);

            // Create the Graphics object that does the work
            using (Graphics graphicsObject = Graphics.FromImage(newBitmap))
            {
                graphicsObject.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsObject.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphicsObject.SmoothingMode = SmoothingMode.HighQuality;

                // Fill in the specified background color if necessary
                if (backgroundColor != Color.Transparent)
                    graphicsObject.Clear(backgroundColor);

                // Set up the built-in transformation matrix to do the rotation and maybe scaling
                graphicsObject.TranslateTransform(newWidth / 2f, newHeight / 2f);

                if (scaleFactor != 1f)
                    graphicsObject.ScaleTransform(scaleFactor, scaleFactor);

                graphicsObject.RotateTransform(angleDegrees);
                graphicsObject.TranslateTransform(-oldWidth / 2f, -oldHeight / 2f);

                // Draw the result 
                graphicsObject.DrawImage(inputImage, 0, 0);
            }

            return newBitmap;
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
            var absoluteCenterLocation = _world.GetGroup<EntityGroup<CCamera, CMovement, CTransform>>().Components3[0].Location;
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

        private void FpsTimer_Tick(object sender, EventArgs e)
        {
            this.Text = String.Format("FPS: {0}", ticks);
            ticks = 0;
        }
    }
}
