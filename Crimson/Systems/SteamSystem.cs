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
        readonly EntityGroup<CBurning, CTransform> _steamingHot;
        readonly EntityGroup<CWet, CBurning, CTransform> _wetHot_InYourArea;

        public SteamSystem(World world)
        {
            _world = world;
            _steamingHot = _world.GetGroup<EntityGroup<CBurning, CTransform>>();
            _wetHot_InYourArea = _world.GetGroup<EntityGroup<CWet, CBurning, CTransform>>();

            var cloud = Properties.Resources.cloud;
            images03 = new List<(Image Img, int Size)>() { (cloud, 55), (cloud, 58), (cloud, 60), (cloud, 64), (cloud, 69) }
                .Select(i => Utilities.ResizeImage(i.Img, i.Size, i.Size))
                .Select(i => (Image)ChangeOpacity(i, 0.25f))
                .ToList();
            images08 = new List<(Image Img, int Size)>() { (cloud, 55), (cloud, 58), (cloud, 60), (cloud, 64), (cloud, 69) }
                .Select(i => Utilities.ResizeImage(i.Img, i.Size, i.Size))
                .Select(i => (Image)ChangeOpacity(i, 0.6f))
                .ToList();
        }

        public override void Update()
        {
            foreach (var (entity, fire, transform) in _steamingHot)
            {
                switch (rnd.Next(3))
                {
                    case 0:
                        CreateCloud(transform.Location, images03, 20);
                        break;
                }
            }

            foreach (var (entity, water, fire, transform) in _wetHot_InYourArea)
            {
                entity.ScheduleComponentForRemoval(typeof(CBurning));
                entity.ScheduleComponentForRemoval(typeof(CWet));
                var count = 0;
                switch (rnd.Next(5))
                {
                    case 0:
                        count = 1;
                        break;
                    default:
                        count = 0;
                        break;
                }
                //foreach (var _ in Enumerable.Range(0, count)) { CreateCloud(transform.Location, images08, 25); }
            }
        }

        List<Image> images08;
        List<Image> images03;

        void CreateCloud(Vector location, List<Image> images, int lifeTIme)
        {
            var cloud = _world.CreateEntity();
            cloud.AddComponent(new CAbove());
            cloud.AddComponent(new CGraphics(images[rnd.Next(images.Count)]));
            cloud.AddComponent(new CTransform(location + new Vector(Sign() * rnd.Next(10), Sign() * rnd.Next(10))));
            cloud.ScheduleForDeletion(lifeTIme);
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
