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
        EntityFilter<CPosition, CMovement> _filter = new EntityFilter<CPosition, CMovement>();

        public MovementSystem(World world)
        {
            _world = world;
            _filter = _world.GetFilter<EntityFilter<CPosition, CMovement>>();
        }

        public override void Update()
        {
            foreach (var i in Enumerable.Range(0, _filter.Entities.Count))
            {
                var entity = _filter.Entities[i];
                var position = _filter.Components1[i];
                var move = _filter.Components2[i];
                (var X, var Y) = position.Coords;
                var newPosition = (X + move.Speed.X * move.Acceleration.X, Y + move.Speed.Y * move.Acceleration.Y);

                if (newPosition != (X, Y))
                {
                    Debug.WriteLine("Hey");
                    _world.AddComponentToEntity(entity, new CPosition(newPosition));
                    _world.AddComponentToEntity(entity, new CMovement(move.Speed, (0, 0)));
                }
            }
        }
    }
}
