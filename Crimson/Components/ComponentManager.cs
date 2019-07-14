using System;
using System.Collections.Generic;
using System.Diagnostics;
using Crimson.Entities;

namespace Crimson.Components
{
    // TODO: Existuje nějaký lepší způsob přidělování ID jednotlivým ComponentManagerům než toto?
    static class ComponentManagerDB
    {
        public static Dictionary<int, Type> ComponentManagers = new Dictionary<int, Type>();
    }

    interface IComponentManager { }

    class ComponentManager<T> : IComponentManager where T : Component
    {
        public static ComponentManager<T> Instance = new ComponentManager<T>();
        public int ID;

        readonly Dictionary<Entity, T> _components = new Dictionary<Entity, T>();

        ComponentManager() {
            ID = ComponentManagerDB.ComponentManagers.Count;
            ComponentManagerDB.ComponentManagers[ID] = typeof(T);
        }

        public void SetComponentOfEntity(Entity e, T c)
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
