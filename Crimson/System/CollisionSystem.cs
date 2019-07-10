using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crimson.Entities;
using Crimson.Components;

namespace Crimson.Systems
{
    class CollisionCheckSystem : GameSystem
    {
        readonly EntityFilter<CTransform, CCollidable> _filter;

        public CollisionCheckSystem(World world)
        {
            _world = world;
            _filter = _world.GetFilter<EntityFilter<CTransform, CCollidable>>();
        }

        public override void Update()
        {
            foreach (var i in Enumerable.Range(0, _filter.Count))
            {
                var (entityA, transformA, collidableA) = _filter[i];
                foreach (var j in Enumerable.Range(i + 1, _filter.Count - i - 1))
                {
                    var (entityB, transformB, collidableB) = _filter[i];
                    if (Collide(transformA.Location, collidableA.Size, transformB.Location, collidableB.Size))
                    {
                        _world.SetComponentOfEntity(entityA, new CCollisionEvent(entityB));
                        _world.SetComponentOfEntity(entityB, new CCollisionEvent(entityA));
                    }
                }
            }
        }

        bool Collide(Vector aLocation, Vector aSize, Vector bLocation, Vector bSize)
        {
            return (
                Math.Min(aLocation.X + aSize.X, bLocation.X + bSize.X) >= Math.Max(aLocation.X, bLocation.X) &&
                Math.Min(aLocation.Y + aSize.Y, bLocation.Y + bSize.Y) >= Math.Max(aLocation.Y, bLocation.Y)
            );
        }
    }
}
