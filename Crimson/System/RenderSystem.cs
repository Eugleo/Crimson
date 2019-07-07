using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace Crimson
{
    class RenderSystem: GameSystem
    {
        readonly Control _mainControl;
        readonly Control _mapControl;
        readonly EntityFilter<CPosition, CGraphics, GameObject> _renderable;
        readonly EntityFilter<CPosition, CCamera> _cameras;
        readonly EntityFilter<CPosition, CTile, CGraphics> _tiles;

        public RenderSystem(World world, Control mainControl, Control mapControl)
        {
            _world = world;
            _mainControl = mainControl;
            _mapControl = mapControl;
            _mainControl.Paint += MainControl_Paint;
            _mapControl.Paint += MapControl_Paint;

            _renderable = _world.GetFilter<EntityFilter<CPosition, CGraphics, GameObject>>();
            _cameras = _world.GetFilter<EntityFilter<CPosition, CCamera>>();
            _tiles = _world.GetFilter<EntityFilter<CPosition, CTile, CGraphics>>();
        }

        public override void Update()
        {
            _mainControl.Refresh();
            _mapControl.Refresh();
        }

        // TODO: Zredukovat duplikovaný kod v následujících dvou funkcích
        void MainControl_Paint(object sender, PaintEventArgs e)
        {
            var (cameraX, cameraY) = _cameras.Components1[0].Coords;
            var width = _mapControl.Width;
            var height = _mapControl.Height;
            var left = cameraX - width / 2;
            var right = cameraX + width / 2;
            var top = cameraY - height / 2;
            var bottom = cameraY + height / 2;

            foreach (var i in Enumerable.Range(0, _renderable.Entities.Count))
            {
                var entity = _renderable.Entities[i];
                var (X, Y) = _renderable.Components1[i].Coords;
                var image = _renderable.Components2[i].Image;

                if (X + image.Width > left && X < right && Y + image.Height > top && Y < bottom)
                {
                    e.Graphics.DrawImage(image, (float)(X - left), (float)(Y - top));
                }
            }
        }

        void MapControl_Paint(object sender, PaintEventArgs e)
        {

            var (cameraX, cameraY) = _cameras.Components1[0].Coords;
            var width = _mapControl.Width;
            var height = _mapControl.Height;
            var left = cameraX - width / 2;
            var right = cameraX + width / 2;
            var top = cameraY - height / 2;
            var bottom = cameraY + height / 2;

            foreach (var i in Enumerable.Range(0, _tiles.Entities.Count))
            {
                var (X, Y) = _tiles.Components1[i].Coords;
                var tile = _tiles.Components2[i];
                var image = _tiles.Components3[i].Image;

                if (X + image.Width > left && X < right && Y + image.Height > top && Y < bottom)
                {
                    e.Graphics.DrawImage(image, (float)(X - left), (float)(Y - top));
                }
            }
        }
    }
}
