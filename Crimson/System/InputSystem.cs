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
        readonly EntityFilter<CInput, CMovement, CKeyboardNavigation> _filter = new EntityFilter<CInput, CMovement, CKeyboardNavigation>();

        public InputSystem(World world)
        {
            _world = world;
            _filter = _world.GetFilter<EntityFilter<CInput, CMovement, CKeyboardNavigation>>();
        }

        public override void Update()
        {
            foreach (var i in Enumerable.Range(0, _filter.Entities.Count))
            {
                var entity = _filter.Entities[i];
                var keyEventArgs = _filter.Components1[i].KeyEventArgs;
                var acc = keyEventArgs.Count > 1 ? Math.Sqrt(2) / 2 : 1;

                foreach (var ke in keyEventArgs)
                {
                    var move = _filter.Components2[i];
                    switch (ke.KeyCode)
                    {
                        case Keys.Down:
                        case Keys.S:
                            Debug.WriteLine("Down!");
                            _world.AddComponentToEntity(entity, new CMovement(move.Speed, (move.Acceleration.X, acc)));
                            break;
                        case Keys.Up:
                        case Keys.W:
                            Debug.WriteLine("Up!");
                            _world.AddComponentToEntity(entity, new CMovement(move.Speed, (move.Acceleration.X, -acc)));
                            break;
                        case Keys.Left:
                        case Keys.A:
                            Debug.WriteLine("Left!");
                            _world.AddComponentToEntity(entity, new CMovement(move.Speed, (-acc, move.Acceleration.Y)));
                            break;
                        case Keys.Right:
                        case Keys.D:
                            Debug.WriteLine("Right!");
                            _world.AddComponentToEntity(entity, new CMovement(move.Speed, (acc, move.Acceleration.Y)));
                            break;
                    }
                }
            }
            _world.ForEachEntityWithComponents<CInput>(e => _world.RemoveComponentFromEntity<CInput>(e));
        }
    }
}
