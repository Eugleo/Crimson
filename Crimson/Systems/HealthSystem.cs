using Crimson.Components;
using Crimson.Entities;

namespace Crimson.Systems
{
    class HealthSystem : GameSystem
    {
        readonly EntityGroup<CHealth> _filter;

        public HealthSystem(World world)
        {
            _world = world;
            _filter = _world.GetGroup<EntityGroup<CHealth>>();
        }
        public override void Update()
        {
            foreach (var (entity, health) in _filter)
            {
                if (health.CurrentHealth <= 0)
                {
                    entity.ScheduleForDeletion();
                }
            }
        }
    }
}
