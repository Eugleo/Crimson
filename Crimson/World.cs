using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Crimson
{
    class World
    {
        readonly EntityManager _entityManager;
        readonly List<EntityFilter> _filters = new List<EntityFilter>();
        readonly List<GameSystem> _systems = new List<GameSystem>();

        public World()
        {
            _entityManager = new EntityManager(this);
        }

        public EntityHandle CreateEntity()
        {
            return _entityManager.CreateEntity();
        }

        public Component GetComponentForEntity<Component>(Entity e)
        {
            return ComponentManager<Component>.Instance.LookupComponentForEntity(e);
        }

        public void AddComponentToEntity<Component>(Entity e, Component c)
        {
            ComponentManager<Component>.Instance.SetComponentForEntity(e, c);
            var mask = _entityManager.GetComponentMask(e);
            mask.IncludeComponent<Component>();
            UpdateFiltersForEntity(e);
        }

        public void RemoveComponentFromEntity<Component>(Entity e)
        {
            ComponentManager<Component>.Instance.RemoveComponentFromEntity(e);
            var mask = _entityManager.GetComponentMask(e);
            mask.ExcludeComponent<Component>();
            UpdateFiltersForEntity(e);
        }

        public void UpdateFiltersForEntity(Entity entity)
        {
            var mask = _entityManager.GetComponentMask(entity);
            foreach (var filter in _filters)
            {
                if (filter.Entities.Contains(entity))
                {
                    if (!filter.Mask.CompatibleWith(mask))
                    {
                        filter.Remove(entity);
                    }
                    else
                    {
                        filter.UpdateComponentsFor(entity);
                    }
                }
                else if (filter.Mask.CompatibleWith(mask))
                {
                    filter.Add(entity);
                }
            }
        }

        public void ForEachEntityWithComponents<Component>(Action<Entity> f)
        {
            foreach (var entity in _entityManager.Entities)
            {
                if (_entityManager.GetComponentMask(entity).IncludesComponent<Component>())
                {
                    f(entity);
                }
            }
        }

        public FilterT GetFilter<FilterT>() where FilterT : EntityFilter
        {
            var filterType = typeof(FilterT);
            foreach (var i in Enumerable.Range(0, _filters.Count))
            {
                if (_filters[i].GetType() == filterType)
                {
                    return _filters[i] as FilterT;
                }
            }
            var filter = Activator.CreateInstance(filterType, true) as EntityFilter;
            filter._world = this;
            _filters.Add(filter);
            return filter as FilterT;
        } 

        public void AddSystem(GameSystem system) {
            _systems.Add(system);
        }

        public void Tick()
        {
            _systems.ForEach(s => s.Update());
        }
    }
}
