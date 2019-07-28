using Crimson.Entities;
using System;
using System.Collections.Generic;

namespace Crimson.Components
{
    // TODO: Existuje nějaký lepší způsob přidělování ID jednotlivým ComponentManagerům než toto?
    static class ComponentManagerDB
    {
        public static List<IComponentManager> ComponentManagers = new List<IComponentManager>();
    }

    interface IComponentManager
    {
        Type Component { get; }
        Components ComponentID { get; }
    }

    class ComponentManager<T> : IComponentManager where T : IComponent, new()
    {
        public static ComponentManager<T> Instance = new ComponentManager<T>();
        public int ID;
        public Components ComponentID { get; }
        public Type Component { get; }

        readonly Dictionary<Entity, T> _components = new Dictionary<Entity, T>();

        ComponentManager()
        {
            ID = ComponentManagerDB.ComponentManagers.Count;
            ComponentManagerDB.ComponentManagers.Add(this);
            ComponentID = new T().Component;
            Component = typeof(T);
        }

        public void AddComponentToEntity(Entity e, T c)
        {
            _components[e] = c;
        }

        public T LookupComponentForEntity(Entity e)
        {
            return _components[e];
        }

        public void RemoveComponentFromEntity(Entity e)
        {
            _components.Remove(e);
        }
    }
}
