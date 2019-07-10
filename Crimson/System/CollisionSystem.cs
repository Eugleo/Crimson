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
            foreach (var (entity, transform, bounds) in _filter)
            {
                var o = _filter[2];
            }
        }
    }
}
