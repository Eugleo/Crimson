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

        readonly Dictionary<Type, MethodInfo> _includeCheckers = new Dictionary<Type, MethodInfo>();
        readonly Dictionary<Type, MethodInfo> _removers = new Dictionary<Type, MethodInfo>();
        public void RemoveEntity(Entity e)
        {
            _entityManager.RemoveEntity(e);
            foreach (var cm in ComponentManagerDB.ComponentManagers.Values)
            {
                if (!_includeCheckers.TryGetValue(cm, out MethodInfo m))
                {
                    MethodInfo method = typeof(ComponentMask).GetMethod("DoesIncludeComponent");
                    m = method.MakeGenericMethod(cm);
                    _includeCheckers[cm] = m;
                }
               
                if (Convert.ToBoolean(m.Invoke(_entityManager.GetComponentMask(e), new object[0])))
                {
                    if (!_removers.TryGetValue(cm, out MethodInfo m2))
                    {
                        MethodInfo method = typeof(World).GetMethod("RemoveComponentFromEntity");
                        m2 = method.MakeGenericMethod(cm);
                        _removers[cm] = m2;
                    }
                    m2.Invoke(this, new object[1] { e });
                }
            }
        }

        public T GetComponentForEntity<T>(Entity e) where T : Component
        {
            return ComponentManager<T>.Instance.LookupComponentForEntity(e);
        }

        public void AddComponentToEntity<T>(Entity e, T c) where T : Component
        {
            ComponentManager<T>.Instance.AddComponentToEntity(e, c);
            var mask = _entityManager.GetComponentMask(e);
            var oldMask = mask.Clone();
            mask.IncludeComponent<T>();
            UpdateGroupsForEntity(e, mask, oldMask);
        }

        public bool EntityHasComponent<T>(Entity e) where T : Component
        {
            return _entityManager.GetComponentMask(e).DoesIncludeComponent<T>();
        }

        public void RemoveComponentFromEntity<T>(Entity e) where T : Component
        {
            ComponentManager<Component>.Instance.RemoveComponentFromEntity(e);
            var mask = _entityManager.GetComponentMask(e);
            var oldMask = mask.Clone();
            mask.RemoveComponent<T>();
            UpdateGroupsForEntity(e, mask, oldMask);
        }

        void UpdateGroupsForEntity(Entity entity, ComponentMask newMask, ComponentMask oldMask)
        {
            foreach (var group in _entityGroups)
            {
                var before = group.Mask.CompatibleWith(oldMask);
                var after = group.Mask.CompatibleWith(newMask);

                if (!before && !after) { continue; }

                if (before && after)
                {
                    group.UpdateComponentsFor(entity);
                }
                else if (before)
                {
                    group.Remove(entity);
                }
                else if (after)
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
            var index = _entityGroups.FindIndex(g => g.GetType() == groupType);
            if (index != -1)
            {
                return _entityGroups[index] as Group;
            }
            else
            {
                var group = Activator.CreateInstance(groupType, true) as EntityGroup;
                group._world = this;
                _entityGroups.Add(group);

                foreach (var entity in _entityManager.Entities)
                {
                    var mask = _entityManager.GetComponentMask(entity);
                    if (group.Mask.CompatibleWith(mask)) { group.Add(entity); }
                }

                return group as Group;
            }
        } 

        public void AddSystem(GameSystem system) {
            _systems.Add(system);
        }


        public uint Tick()
        {
            _systems.ForEach(s => s.Update());
            return 1;
        }


    }
}
