using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Crimson.Entities;
using Crimson.Components;

namespace Crimson.Systems
{
    class MovementSystem : GameSystem
    {
        readonly EntityGroup<CTransform, CMovement> _moveable;
        readonly EntityGroup<CTransform, CCollidable> _collidable;
        readonly Map _map;

        readonly double STEP_SIZE = 1;

        public MovementSystem(World world, Map map)
        {
            _world = world;
            _moveable = _world.GetGroup<EntityGroup<CTransform, CMovement>>();
            _collidable = _world.GetGroup<EntityGroup<CTransform, CCollidable>>();
            _map = map;
        }

        public override void Update()
        {
            foreach (var (entity, transform, movement) in _moveable)
            {
                var current = transform.Location;
                var next = transform.Location + movement.Velocity;
                if (entity.TryGetComponent(out CCollidable bounds))
                {
                    foreach (var (entity2, transform2, bounds2) in _collidable)
                    {
                        if (entity2.Entity == entity.Entity) { continue; }
                        if (!entity.HasComponent<CBullet>() && !entity2.HasComponent<CBullet>())
                        {
                            var moveByX = new Vector(next.X, current.Y);
                            if (IsColliding(moveByX, bounds.Size, transform2.Location, bounds2.Size) || IsOutOfMap(moveByX))
                            {
                                next = new Vector(current.X, next.Y);
                            }
                            var moveByY = new Vector(current.X, next.Y);
                            if (IsColliding(moveByY, bounds.Size, transform2.Location, bounds2.Size) || IsOutOfMap(moveByY))
                            {
                                next = new Vector(next.X, current.Y);
                            }
                        }
                    }
                }

                if (entity.HasComponent<CBullet>() && entity.TryGetComponent(out CFaction faction))
                {
                    var possiblePlaces = Enumerable.Range(1, (int)Math.Round(Math.Abs(movement.Velocity.Size) / STEP_SIZE))
                        .Select(i => current + movement.Velocity.Normalized(i * STEP_SIZE));

                    foreach (var location in possiblePlaces)
                    {
                        foreach (var (entity2, transform2, bounds2) in _collidable)
                        {
                            if (entity2.HasComponent<CBullet>() || (entity2.TryGetComponent(out CFaction faction2) && faction2.Faction == faction.Faction)) { continue; }

                            next = location;
                            if (IsColliding(location, bounds.Size, transform2.Location, bounds2.Size))
                            {
                                entity.AddComponent(new CCollisionEvent(entity2));
                                entity2.AddComponent(new CCollisionEvent(entity));
                                break;
                            }
                        }
                    }
                }

                if (entity.HasComponent<CFeeler>())
                {
                    foreach (var (entity2, transform2, bounds2) in _collidable)
                    {
                        if (entity2.Entity == entity.Entity) { continue; }
                        if (!entity2.HasComponent<CBullet>())
                        {
                            var moveByX = new Vector(next.X, current.Y);
                            var moveByY = new Vector(current.X, next.Y);
                            if (IsColliding(moveByX, 20, transform2.Location, bounds2.Size) || IsOutOfMap(moveByX) ||
                                IsColliding(moveByY, 20, transform2.Location, bounds2.Size) || IsOutOfMap(moveByY))
                            {
                                entity.AddComponent(new CCollisionEvent(entity2));
                            }
                        }
                    }
                }

                if (entity.TryGetComponent(out CBullet bullet))
                {
                    entity.AddComponent(new CBullet(bullet.Damage, bullet.RangeLeft - (next - current).Size));
                }

                entity.AddComponent(new CTransform(next));
            }

            bool IsOutOfMap(Vector location)
            {
                var X = location.X / _map.TileSize;
                var Y = location.Y / _map.TileSize;
                return (X < 0 || X > _map.Width - 1 || Y < 0 || Y > _map.Height - 1);
            }

            bool IsColliding(Vector aLocation, int aSize, Vector bLocation, int bSize)
            {
                var centerA = new Vector(aLocation.X + aSize, aLocation.Y + aSize);
                var centerB = new Vector(bLocation.X + bSize, bLocation.Y + bSize);
                return (centerA - centerB).Size < aSize + bSize;
            }
        }
    }
}
