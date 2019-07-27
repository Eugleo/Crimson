using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Crimson.Entities;
using Crimson.Components;
using System.Drawing;
using System.Drawing.Imaging;

namespace Crimson.Systems
{
    class SteamSystem : GameSystem
    {
        readonly Random rnd = new Random();
        readonly EntityGroup<COnFire, CTransform> _steamingHot;
        readonly EntityGroup<CWet, COnFire, CTransform> _wetHot_InYourArea;

        public SteamSystem(World world)
        {
            _world = world;
            _steamingHot = _world.GetGroup<EntityGroup<COnFire, CTransform>>();
            _wetHot_InYourArea = _world.GetGroup<EntityGroup<CWet, COnFire, CTransform>>();

            var cloud = Properties.Resources.cloud;
            images03 = new List<(Image Img, int Size)>() { (cloud, 55), (cloud, 58), (cloud, 60), (cloud, 64), (cloud, 69) }
                .Select(i => MainForm.ResizeImage(i.Img, i.Size, i.Size))
                .Select(i => (Image)ChangeOpacity(i, 0.3f))
                .ToList();
            images08 = new List<(Image Img, int Size)>() { (cloud, 55), (cloud, 58), (cloud, 60), (cloud, 64), (cloud, 69) }
                .Select(i => MainForm.ResizeImage(i.Img, i.Size, i.Size))
                .Select(i => (Image)ChangeOpacity(i, 0.8f))
                .ToList();
        }

        public override void Update()
        {
            foreach (var (entity, fire, transform) in _steamingHot)
            {
                CreateCloud(transform.Location, images03);
            }

            foreach (var (entity, water, fire, transform) in _wetHot_InYourArea)
            {
                entity.ScheduleComponentForRemoval(typeof(COnFire));
                entity.ScheduleComponentForRemoval(typeof(CWet));
                var count = 0;
                switch (rnd.Next(6))
                {
                    case 0:
                    case 1:
                        count = 1;
                        break;
                    case 2:
                    case 3:
                    case 4:
                        count = 2;
                        break;
                    case 5:
                        count = 3;
                        break;
                }
                foreach (var _ in Enumerable.Range(0, count)) { CreateCloud(transform.Location, images08); }
            }
        }

        Image image03 = ChangeOpacity(Properties.Resources.cloud, 0.3f);
        Image image08 = ChangeOpacity(Properties.Resources.cloud, 0.8f);

        List<Image> images08;
        List<Image> images03;

        void CreateCloud(Vector location, List<Image> images)
        {
            var cloud = _world.CreateEntity();
            cloud.AddComponent(new CAbove());
            cloud.AddComponent(new CGraphics(images[rnd.Next(images.Count)]));
            cloud.AddComponent(new CTransform(location + new Vector(Sign() * rnd.Next(10), Sign() * rnd.Next(10))));
            cloud.ScheduleForDeletion(5);
        }

        int Sign()
        {
            return rnd.Next(2) == 0 ? -1 : 1;
        }

        public static Bitmap ChangeOpacity(Image img, float opacityvalue)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height); // Determining Width and Height of Source Image
            Graphics graphics = Graphics.FromImage(bmp);
            ColorMatrix colormatrix = new ColorMatrix();
            colormatrix.Matrix33 = opacityvalue;
            ImageAttributes imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            graphics.Dispose();   // Releasing all resource used by graphics 
            return bmp;
        }
    }
}
