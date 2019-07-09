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
            foreach (var (entity, input, move, _) in _filter)
            {
                var keyEventArgs = input.KeyEventArgs;
                var acc = keyEventArgs.Count > 1 ? Math.Sqrt(2) / 2 : 1;

                double accX = 0;
                double accY = 0;
                foreach (var ke in keyEventArgs)
                {
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
                    _world.AddComponentToEntity(entity, new CMovement(move.Speed, new Vector(accX, accY)));
                }
            }
        }
    }
}
