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
        readonly EntityFilter<CTransform, CCollisionEvent> _filter;

        public CollisionResolverSystem(World world)
        {
            _world = world;
            _filter = _world.GetFilter<EntityFilter<CTransform, CCollisionEvent>>();
        }

        public override void Update()
        {
            List<Entity> toRemove = new List<Entity>();
            foreach (var (entity, transform, collision) in _filter)
            {
                if (_world.EntityHasComponent<CBullet>(collision.Partner) && _world.EntityHasComponent<CHealth>(entity))
                {
                    var bullet = _world.GetComponentForEntity<CBullet>(collision.Partner);
                    var health = _world.GetComponentForEntity<CHealth>(entity);
                    var newHealth = new CHealth(health.MaxHealth, health.CurrentHealth - bullet.Damage);
                    _world.SetComponentOfEntity(entity, newHealth);
                    toRemove.Add(collision.Partner);
                }
            }
            toRemove.ForEach(e => _world.RemoveEntity(e));
            _world.ForEachEntityWithComponents<CCollisionEvent>(e => _world.RemoveComponentFromEntity<CCollisionEvent>(e));
        }
    }
}
