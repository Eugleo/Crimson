using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Crimson.Components;
using Crimson.Entities;
using Crimson.Systems;
using System.Reflection;

namespace Crimson
{
    class World
    {
        readonly EntityManager _entityManager;
        readonly List<EntityGroup> _entityGroups = new List<EntityGroup>();
        readonly List<GameSystem> _systems = new List<GameSystem>();

        public World()
        {
            _entityManager = new EntityManager(this);
        }

        public EntityHandle CreateEntity()
        {
            return _entityManager.CreateEntity();
        }

        public void RemoveEntity(Entity e)
        {
            Debug.WriteLine("Remove {0}", e.ID);
            _entityManager.RemoveEntity(e);
            foreach (var cm in ComponentManagerDB.ComponentManagers.Values)
            {
                MethodInfo method = typeof(ComponentMask).GetMethod("DoesIncludeComponent");
                method = method.MakeGenericMethod(cm);
                if (Convert.ToBoolean(method.Invoke(_entityManager.GetComponentMask(e), new object[0])))
                {
                    MethodInfo method2 = typeof(World).GetMethod("RemoveComponentFromEntity");
                    method2 = method2.MakeGenericMethod(cm);
                    method2.Invoke(this, new object[1] { e });
                }
            }
        }

        public T GetComponentForEntity<T>(Entity e) where T : Component
        {
            return ComponentManager<T>.Instance.LookupComponentForEntity(e);
        }

        public void SetComponentOfEntity<T>(Entity e, T c) where T : Component
        {
            ComponentManager<T>.Instance.SetComponentOfEntity(e, c);
            var mask = _entityManager.GetComponentMask(e);
            mask.IncludeComponent<T>();
            UpdateFiltersForEntity(e);
        }

        public bool EntityHasComponent<T>(Entity e) where T : Component
        {
            return _entityManager.GetComponentMask(e).DoesIncludeComponent<T>();
        }

        public void RemoveComponentFromEntity<T>(Entity e) where T : Component
        {
            ComponentManager<Component>.Instance.RemoveComponentFromEntity(e);
            var mask = _entityManager.GetComponentMask(e);
            mask.RemoveComponent<T>();
            UpdateFiltersForEntity(e);
        }

        public void UpdateFiltersForEntity(Entity entity)
        {
            var mask = _entityManager.GetComponentMask(entity);

            foreach (var group in _entityGroups)
            {
                if (group.Entities.Select(e => e.Entity).Contains(entity))
                {
                    if (!group.Mask.CompatibleWith(mask))
                    {
                        group.Remove(entity);
                    }
                    else
                    {
                        group.UpdateComponentsFor(entity);
                    }
                }
                else if (group.Mask.CompatibleWith(mask))
                {
                    group.Add(entity);
                }
            }
        }

        public void ForEachEntityWithComponents<T>(Action<Entity> f) where T : Component
        { 
            foreach (var entity in _entityManager.Entities)
            {
                if (_entityManager.GetComponentMask(entity).DoesIncludeComponent<T>())
                {
                    f(entity);
                }
            }
        }

        public Group GetGroup<Group>() where Group : EntityGroup
        {
            var groupType = typeof(Group);
            foreach (var i in Enumerable.Range(0, _entityGroups.Count))
            {
                if (_entityGroups[i].GetType() == groupType)
                {
                    return _entityGroups[i] as Group;
                }
            }
            var group = Activator.CreateInstance(groupType, true) as EntityGroup;
            group._world = this;
            _entityGroups.Add(group);
            return group as Group;
        } 

        public void AddSystem(GameSystem system) {
            _systems.Add(system);
            _entityManager.Entities.ForEach(e => UpdateFiltersForEntity(e));
        }

        public void Tick()
        {
            _systems.ForEach(s => s.Update());
        }
    }
}
