using System;
using System.Diagnostics;
using Crimson.Entities;
using Crimson.Components;

namespace Crimson.Systems
{
    class MovementSystem: GameSystem
    {
        readonly EntityFilter<CTransform, CMovement> _filter;
        readonly EntityFilter<CTransform, CCollidable> _collidable; 

        public MovementSystem(World world)
        {
            _world = world;
            _filter = _world.GetFilter<EntityFilter<CTransform, CMovement>>();
            _collidable = _world.GetFilter<EntityFilter<CTransform, CCollidable>>();
        }

        public override void Update()
        {
            foreach (var (entity, transform, movement) in _filter)
            {
                var newPosition = transform.Location + movement.Acceleration.ScaledBy(movement.Speed);

                if (_world.EntityHasComponent<CCollidable>(entity))
                {
                    var bounds = _world.GetComponentForEntity<CCollidable>(entity);
                    foreach (var (entity2, transform2, bounds2) in _collidable)
                    {
                        if (entity2 != entity && 
                            CollideArea(newPosition, bounds.Size, transform2.Location, bounds2.Size) > 20 &&
                            !(_world.EntityHasComponent<CBullet>(entity) && _world.EntityHasComponent<CBullet>(entity2)))
                        {
                            newPosition = transform.Location;
                            _world.SetComponentOfEntity(entity, new CCollisionEvent(entity2));
                            _world.SetComponentOfEntity(entity2, new CCollisionEvent(entity));
                        }
                    }
                }

                if (newPosition != transform.Location)
                {
                    _world.SetComponentOfEntity(entity, new CTransform(newPosition));
                }
            }
        }

        double CollideArea(Vector aLocation, Vector aSize, Vector bLocation, Vector bSize)
        {
            var X = Math.Min(aLocation.X + aSize.X, bLocation.X + bSize.X) - Math.Max(aLocation.X, bLocation.X);
            var Y = Math.Min(aLocation.Y + aSize.Y, bLocation.Y + bSize.Y) - Math.Max(aLocation.Y, bLocation.Y);
            return X > 0 && Y > 0 ? X * Y : 0;
        }
    }
}
