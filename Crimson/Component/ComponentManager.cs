using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Crimson
{
    // TODO: Existuje nějaký lepší způsob přidělování ID jednotlivým ComponentManagerům než toto?
    static class ComponentManagerDB
    {
        public static int ComponentManagerCount = 0;
    }

    class ComponentManager<Component> 
    {
        public static ComponentManager<Component> Instance = new ComponentManager<Component>();
        public int ID;

        ComponentManager() {
            ID = ComponentManagerDB.ComponentManagerCount;
            ComponentManagerDB.ComponentManagerCount += 1;
        }

        Dictionary<Entity, Component> _components = new Dictionary<Entity, Component>();

        public void SetComponentForEntity(Entity e, Component c)
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
