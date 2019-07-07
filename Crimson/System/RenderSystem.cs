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
        readonly Control _control;
        EntityFilter<CPosition, CGraphics> _filter = new EntityFilter<CPosition, CGraphics>(); 

        public RenderSystem(World world, Control control)
        {
            _world = world;
            _control = control;
            _filter = _world.GetFilter<EntityFilter<CPosition, CGraphics>>();
            _control.Paint += Control_Paint;
        }

        public override void Update()
        {
            _control.Refresh();
        }

        void Control_Paint(object sender, PaintEventArgs e)
        {
            foreach (var i in Enumerable.Range(0, _filter.Entities.Count))
            {
                var entity = _filter.Entities[i];
                var position = _filter.Components1[i];
                var image = _filter.Components2[i].Image;

                var (X, Y) = position.Coords;
                position.Changed = false;
                _world.AddComponentToEntity(entity, position);
                e.Graphics.DrawImage(image, (float)X, (float)Y);
            }
        }
    }
}
