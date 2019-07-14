using System.Diagnostics;
using System.Windows.Forms;
using Crimson.Entities;
using Crimson.Components;

namespace Crimson.Systems
{
    class RenderSystem: GameSystem
    {
        readonly Control _mainControl;
        readonly Control _mapControl;
        readonly EntityGroup<CTransform, CGraphics, CGameObject> _renderable;
        readonly EntityGroup<CTransform, CCamera> _cameras;
        readonly EntityGroup<CTransform, CGraphics, CTile> _tiles;

        public RenderSystem(World world, Control mainControl, Control mapControl)
        {
            _world = world;
            _mainControl = mainControl;
            _mapControl = mapControl;
            _mainControl.Paint += MainControl_Paint;
            _mapControl.Paint += MapControl_Paint;

            _renderable = _world.GetGroup<EntityGroup<CTransform, CGraphics, CGameObject>>();
            _cameras = _world.GetGroup<EntityGroup<CTransform, CCamera>>();
            _tiles = _world.GetGroup<EntityGroup<CTransform, CGraphics, CTile>>();
        }

        public override void Update()
        {
            if (_cameras.Count > 0)
            {
                _mainControl.Refresh();
                _mapControl.Refresh();
            }
        }

        // TODO: Zredukovat duplikovaný kod v následujících dvou funkcích
        void MainControl_Paint(object sender, PaintEventArgs e)
        {
            if (_cameras.Count > 0)
            {
                var location = _cameras.Components1[0].Location;
                var width = _mapControl.Width;
                var height = _mapControl.Height;
                var left = location.X - width / 2;
                var right = location.X + width / 2;
                var top = location.Y - height / 2;
                var bottom = location.Y + height / 2;

                foreach (var (entity, transform, graphics, _) in _renderable)
                {
                    var entityLocation = transform.Location;
                    var image = graphics.Image;

                    if (entityLocation.X + image.Width > left && entityLocation.X < right &&
                        entityLocation.Y + image.Height > top && entityLocation.Y < bottom)
                    {
                        e.Graphics.DrawImage(image, (float)(entityLocation.X - left), (float)(entityLocation.Y - top));
                    }
                }
            }
        }

        void MapControl_Paint(object sender, PaintEventArgs e)
        {
            if (_cameras.Count > 0)
            {
                var location = _cameras.Components1[0].Location;
                var width = _mapControl.Width;
                var height = _mapControl.Height;
                var left = location.X - width / 2;
                var right = location.X + width / 2;
                var top = location.Y - height / 2;
                var bottom = location.Y + height / 2;

                foreach (var (entity, transform, graphics, _) in _tiles)
                {
                    var tileLocation = transform.Location;
                    var image = graphics.Image;

                    if (tileLocation.X + image.Width > left && tileLocation.X < right &&
                        tileLocation.Y + image.Height > top && tileLocation.Y < bottom)
                    {
                        e.Graphics.DrawImage(image, (float)(tileLocation.X - left), (float)(tileLocation.Y - top));
                    }
                }
            }
        }
    }
}
