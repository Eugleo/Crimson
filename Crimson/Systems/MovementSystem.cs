using System;
using System.Diagnostics;
using Crimson.Entities;
using Crimson.Components;

namespace Crimson.Systems
{
    class MovementSystem: GameSystem
    {
        readonly EntityGroup<CTransform, CMovement> _moveable;
        readonly EntityGroup<CTransform, CCollidable> _collidable; 
        readonly EntityGroup<CMap> _map;

        public MovementSystem(World world)
        {
            _world = world;
            _moveable = _world.GetGroup<EntityGroup<CTransform, CMovement>>();
            _collidable = _world.GetGroup<EntityGroup<CTransform, CCollidable>>();
            _map = _world.GetGroup<EntityGroup<CMap>>();
        }

        public override void Update()
        {
            foreach (var (entity, transform, movement) in _moveable)
            {
                var newPosition = transform.Location + movement.Acceleration.ScaledBy(movement.Speed);

                if (IsOutOfMap(newPosition) && !entity.HasComponent<CBullet>())
                {
                    continue;
                }

                if (entity.HasComponent<CCollidable>())
                {
                    var bounds = entity.GetComponent<CCollidable>();
                    foreach (var (entity2, transform2, bounds2) in _collidable)
                    {
                        if (entity2.Entity != entity.Entity && 
                            CollideArea(newPosition, bounds.Size, transform2.Location, bounds2.Size) > 20 &&
                            !(entity.HasComponent<CBullet>() && entity2.HasComponent<CBullet>()))
                        {
                            newPosition = transform.Location;
                            entity.AddComponent(new CCollisionEvent(entity2));
                            entity2.AddComponent(new CCollisionEvent(entity));
                        }
                    }
                }

                if (entity.HasComponent<CBullet>())
                {
                    var bullet = entity.GetComponent<CBullet>();
                    Debug.WriteLine(bullet.RangeLeft);
                    var newBullet = new CBullet(bullet.Damage, bullet.RangeLeft - (newPosition - transform.Location).Size);
                    entity.AddComponent(newBullet);
                }

                if (newPosition != transform.Location)
                {
                     entity.AddComponent(new CTransform(newPosition));
                }
            }
        }

        bool IsOutOfMap(Vector location)
        {
            var map = _map[0].Item2;
            var X = location.X / map.TileSize;
            var Y = location.Y / map.TileSize;
            return (X < 0 || X > map.Width - 1 || Y < 0 || Y > map.Height - 1);
        }

        double CollideArea(Vector aLocation, Vector aSize, Vector bLocation, Vector bSize)
        {
            var X = Math.Min(aLocation.X + aSize.X, bLocation.X + bSize.X) - Math.Max(aLocation.X, bLocation.X);
            var Y = Math.Min(aLocation.Y + aSize.Y, bLocation.Y + bSize.Y) - Math.Max(aLocation.Y, bLocation.Y);
            return X > 0 && Y > 0 ? X * Y : 0;
        }
    }
}
