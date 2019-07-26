using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crimson.Entities;
using Crimson.Components;
using System.Diagnostics;
using System.Reflection;

namespace Crimson.Systems
{
    class CollisionResolverSystem : GameSystem
    {
        readonly EntityGroup<CTransform, CCollisionEvent> _filter;

        public CollisionResolverSystem(World world)
        {
            _world = world;
            _filter = _world.GetGroup<EntityGroup<CTransform, CCollisionEvent>>();
        }

        public override void Update()
        {
            var toRemove = new List<EntityHandle>();
            foreach (var (entity, transform, collision) in _filter)
            {
                if (collision.Partner.TryGetComponent(out CBullet bullet) && entity.TryGetComponent(out CHealth health))
                {
                    entity.AddComponent(new CHealth(health.MaxHealth, health.CurrentHealth - bullet.Damage));
                    toRemove.Add(collision.Partner);
                }

                if (collision.Partner.TryGetComponent(out COnCollisionAdder add))
                {
                    MethodInfo method = typeof(EntityHandle).GetMethod("AddComponent");
                    method = method.MakeGenericMethod(add.Component.GetType());
                    _ = method.Invoke(entity, new object[1] { add.Component });
                }
            }
            toRemove.ForEach(e => e.Delete());
            _world.ForEachEntityWithComponents<CCollisionEvent>(e => _world.RemoveComponentFromEntity<CCollisionEvent>(e));
        }
    }
}
