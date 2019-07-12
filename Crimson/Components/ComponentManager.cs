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

    class ComponentManager<Component> : IComponentManager
    {
        public static ComponentManager<Component> Instance = new ComponentManager<Component>();
        public int ID;

        ComponentManager() {
            ID = ComponentManagerDB.ComponentManagers.Count;
            ComponentManagerDB.ComponentManagers[ID] = typeof(Component);
        }

        Dictionary<Entity, Component> _components = new Dictionary<Entity, Component>();

        public void SetComponentOfEntity(Entity e, Component c)
        {
            _components[e] = c;
        }

        public Component LookupComponentForEntity(Entity e)
        {
            return _components[e];
        }

        public void RemoveComponentFromEntity(Entity e)
        {
            _components.Remove(e);
        }
    }
}
