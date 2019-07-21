using System;
using System.Windows.Forms;
using System.Diagnostics;
using Crimson.Entities;
using Crimson.Components;

namespace Crimson.Systems
{
    class InputSystem: GameSystem
    {
        readonly EntityGroup<CInputEvent, CMovement, CKeyboardNavigation> _inputable;

        public InputSystem(World world)
        {
            _world = world;
            _inputable = _world.GetGroup<EntityGroup<CInputEvent, CMovement, CKeyboardNavigation>>();
        }

        public override void Update()
        {
            foreach (var (entity, input, movement, _) in _inputable)
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
                    entity.AddComponent(new CMovement(movement.MaxSpeed, new Vector(accX, accY).ScaledBy(movement.MaxSpeed)));
                }
            }
        }
    }
}
