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

        readonly Dictionary<Type, MethodInfo> _removers = new Dictionary<Type, MethodInfo>();
        public void RemoveEntity(Entity e)
        {
            foreach (var cm in ComponentManagerDB.ComponentManagers)
            {               
                if (_entityManager.GetComponentMask(e).DoesIncludeComponent(cm.ComponentID))
                {
                    if (!_removers.TryGetValue(cm.Component, out MethodInfo m2))
                    {
                        MethodInfo method = typeof(World).GetMethod("RemoveComponentFromEntity");
                        m2 = method.MakeGenericMethod(cm.Component);
                        _removers[cm.Component] = m2;
                    }
                    m2.Invoke(this, new object[1] { e });
                }
            }
        }

        public T GetComponentForEntity<T>(Entity e) where T : IComponent, new ()
        {
            return ComponentManager<T>.Instance.LookupComponentForEntity(e);
        }

        public void AddComponentToEntity<T>(Entity e, T c) where T : IComponent, new ()
        {
            ComponentManager<T>.Instance.AddComponentToEntity(e, c);
            var mask = _entityManager.GetComponentMask(e);
            var oldMask = mask.Clone();
            mask.IncludeComponent(c.Component);
            UpdateGroupsForEntity(e, mask, oldMask, c);
        }

        public bool EntityHasComponent<T>(Entity e) where T : IComponent, new ()
        {
            return _entityManager.GetComponentMask(e).DoesIncludeComponent(ComponentManager<T>.Instance.ComponentID);
        }

        public void RemoveComponentFromEntity<T>(Entity e) where T : IComponent, new ()
        {
            ComponentManager<T>.Instance.RemoveComponentFromEntity(e);
            var mask = _entityManager.GetComponentMask(e);
            var oldMask = mask.Clone();
            mask.RemoveComponent(ComponentManager<T>.Instance.ComponentID);
            UpdateGroupsForEntity(e, mask, oldMask, default);
        }

        void UpdateGroupsForEntity(Entity entity, ComponentMask newMask, ComponentMask oldMask, IComponent component)
        {
            foreach (var group in _entityGroups)
            {
                var before = group.Mask.CompatibleWith(oldMask);
                var after = group.Mask.CompatibleWith(newMask);

                if (!before && !after) { continue; }

                if (before && after)
                {
                    group.UpdateComponent(entity, component);
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

        public void ForEachEntityWithComponents<T>(Action<Entity> f) where T : IComponent, new ()
        { 
            foreach (var entity in _entityManager.Entities)
            {
                if (_entityManager.GetComponentMask(entity).DoesIncludeComponent(ComponentManager<T>.Instance.ComponentID))
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
