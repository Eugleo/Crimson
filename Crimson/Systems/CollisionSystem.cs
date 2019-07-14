using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crimson.Entities;
using Crimson.Components;
using System.Diagnostics;

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
                if (collision.Partner.HasComponent<CBullet>() && entity.HasComponent<CHealth>())
                {
                    var bullet = collision.Partner.GetComponent<CBullet>();
                    var health = entity.GetComponent<CHealth>();
                    var newHealth = new CHealth(health.MaxHealth, health.CurrentHealth - bullet.Damage);
                    entity.AddComponent(newHealth);
                    toRemove.Add(collision.Partner);
                }
            }
            toRemove.ForEach(e => e.Delete());
            _world.ForEachEntityWithComponents<CCollisionEvent>(e => _world.RemoveComponentFromEntity<CCollisionEvent>(e));
        }
    }
}
