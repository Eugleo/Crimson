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
        readonly EntityGroup<CTransform, CGraphics, CAbove> _flying;
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
            _flying = _world.GetGroup<EntityGroup<CTransform, CGraphics, CAbove>>();
            _cameras = _world.GetGroup<EntityGroup<CTransform, CCamera>>();
            _tiles = _world.GetGroup<EntityGroup<CTransform, CGraphics, CTile>>();
        }

        public override void Update()
        {
            if (_cameras.Count > 0)
            {
                // TODO Nějak zařídit, aby se mapa refreshovala jen když je třeba (winforms bohužel neumí průhledné controls)
                _mapControl.Refresh();
                _mainControl.Refresh();
            }
        }

        void MainControl_Paint(object sender, PaintEventArgs e)
        {
            RenderAll(_renderable, e);
            RenderAll(_flying, e);
        }

        void MapControl_Paint(object sender, PaintEventArgs e)
        {
            RenderAll(_tiles, e);
        }

        void RenderAll<T>(EntityGroup<CTransform, CGraphics, T> group, PaintEventArgs e) where T : Component
        {
            if (_cameras.Count == 0) { return; }

            var location = _cameras.Components1[0].Location;
            var width = _mapControl.Width;
            var height = _mapControl.Height;
            var left = location.X - width / 2;
            var right = location.X + width / 2;
            var top = location.Y - height / 2;
            var bottom = location.Y + height / 2;

            foreach (var (entity, transform, graphics, _) in group)
            {
                var tileLocation = transform.Location;
                var image = graphics.Image;

                if (tileLocation.X + image.Width > left && tileLocation.X < right &&
                    tileLocation.Y + image.Height > top && tileLocation.Y < bottom)
                {
                    e.Graphics.DrawImage(image, (float)(tileLocation.X - left), (float)(tileLocation.Y - top));
                    if (entity.HasComponent<CBurning>() && entity.TryGetComponent(out CFlammable flame))
                    {
                        e.Graphics.DrawImage(flame.Image, (float)(tileLocation.X - left), (float)(tileLocation.Y - top));
                    }
                    if (entity.HasComponent<CWet>() && entity.TryGetComponent(out CSumbergable water))
                    {
                        e.Graphics.DrawImage(water.Image, (float)(tileLocation.X - left), (float)(tileLocation.Y - top));
                    }
                }
            }
        }
    }
}
