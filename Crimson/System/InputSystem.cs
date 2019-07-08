using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Crimson
{
    class InputSystem: GameSystem
    {
        readonly EntityFilter<CInputEvent, CMovement, CKeyboardNavigation> _filter;

        public InputSystem(World world)
        {
            _world = world;
            _filter = _world.GetFilter<EntityFilter<CInputEvent, CMovement, CKeyboardNavigation>>();
        }

        public override void Update()
        {
            foreach (var i in Enumerable.Range(0, _filter.Entities.Count))
            {
                var entity = _filter.Entities[i];
                var keyEventArgs = _filter.Components1[i].KeyEventArgs;
                var acc = keyEventArgs.Count > 1 ? Math.Sqrt(2) / 2 : 1;

                double accX = 0;
                double accY = 0;
                foreach (var ke in keyEventArgs)
                {
                    var move = _filter.Components2[i];
                    switch (ke.KeyCode)
                    {
                        case Keys.Down:
                        case Keys.S:
                            accY = acc;
                            break;
                        case Keys.Up:
                        case Keys.W:
                            accY = -acc;
                            break;
                        case Keys.Left:
                        case Keys.A:
                            accX = -acc;
                            break;
                        case Keys.Right:
                        case Keys.D:
                            accX = acc;
                            break;
                    }
                    _world.AddComponentToEntity(entity, new CMovement(move.Speed, (accX, accY)));
                }
            }
        }
    }
}
