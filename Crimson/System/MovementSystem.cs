using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Crimson
{
    class MovementSystem: GameSystem
    {
        EntityFilter<CTransform, CMovement> _filter = new EntityFilter<CTransform, CMovement>();

        public MovementSystem(World world)
        {
            _world = world;
            _filter = _world.GetFilter<EntityFilter<CTransform, CMovement>>();
        }

        public override void Update()
        {
            foreach (var (entity, transform, movement) in _filter)
            {
                var newPosition = transform.Location + movement.Acceleration.ScaledBy(movement.Speed);

                if (newPosition != transform.Location)
                {
                    // TODO: Movement by se neměl sám zastavovat (nebo měl?) promyslet 
                    _world.AddComponentToEntity(entity, new CTransform(newPosition));
                    //_world.AddComponentToEntity(entity, new CMovement(move.Speed, (0, 0)));
                }
            }
        }
    }
}
