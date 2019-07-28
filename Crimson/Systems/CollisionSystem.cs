using Crimson.Components;
using Crimson.Entities;
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
            foreach (var (entity, transform, collision) in _filter)
            {
                if (collision.Partner.TryGetComponent(out CBullet bullet) && entity.TryGetComponent(out CHealth health))
                {
                    entity.AddComponent(new CHealth(health.MaxHealth, health.CurrentHealth - bullet.Damage));
                    collision.Partner.ScheduleForDeletion();
                }

                if (collision.Partner.TryGetComponent(out COnCollisionAdder add))
                {
                    foreach (var component in add.Components)
                    {
                        MethodInfo method = typeof(EntityHandle).GetMethod("AddComponent");
                        method = method.MakeGenericMethod(component.GetType());
                        _ = method.Invoke(entity, new object[1] { component });
                    }
                }
                entity.ScheduleComponentForRemoval(typeof(CCollisionEvent));
            }
        }
    }
}
