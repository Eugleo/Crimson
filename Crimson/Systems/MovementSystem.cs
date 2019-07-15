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
                var current = transform.Location;
                var next = transform.Location + movement.Acceleration.ScaledBy(movement.Speed);
                if (entity.TryGetComponent(out CCollidable bounds))
                {
                    foreach (var (entity2, transform2, bounds2) in _collidable)
                    {
                        if (entity2.Entity == entity.Entity) { continue; }
                        if (!entity.HasComponent<CBullet>() && !entity2.HasComponent<CBullet>())
                        {
                            var moveByX = new Vector(next.X, current.Y);
                            if (CollideArea(moveByX, bounds.Size, transform2.Location, bounds2.Size) > 0 || IsOutOfMap(moveByX))
                            {
                                next = new Vector(current.X, next.Y);
                            }
                            var moveByY = new Vector(current.X, next.Y);
                            if (CollideArea(moveByY, bounds.Size, transform2.Location, bounds2.Size) > 0 || IsOutOfMap(moveByY))
                            {
                                next = new Vector(next.X, current.Y);
                            }
                        }
                        else if (entity.HasComponent<CBullet>() && entity.TryGetComponent(out CFaction faction) && 
                                 CollideArea(current, bounds.Size, transform2.Location, bounds2.Size) > 0)
                        {
                            if ((entity2.TryGetComponent(out CFaction faction2) && faction.Faction != faction2.Faction) ||
                                !entity2.HasComponent<CFaction>())
                            {
                                entity.AddComponent(new CCollisionEvent(entity2));
                                entity2.AddComponent(new CCollisionEvent(entity));
                            }
                        }
                    }
                }

                if (entity.TryGetComponent(out CBullet bullet))
                {
                    entity.AddComponent(new CBullet(bullet.Damage, bullet.RangeLeft - (next - current).Size));
                }

                if (next != current)
                {
                     entity.AddComponent(new CTransform(next));
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
