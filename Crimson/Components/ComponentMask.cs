using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crimson.Components
{
    class ComponentMask
    {
        // TODO vyměnit za bitmask (kdy každý komponent má hodnotu mocniny dvojky a maska je součet komponentů)
        public HashSet<string> IncludedComponents { get; } = new HashSet<string>();
        public HashSet<string> ExcludedComponents { get; } = new HashSet<string>();

        public ComponentMask() { }

        public void IncludeComponent(Type componentType)
        {
            IncludedComponents.Add(componentType.Name);
        }

        public void IncludeComponent<T>() where T : Component
        {
            IncludedComponents.Add(typeof(T).Name);
            ExcludedComponents.Remove(typeof(T).Name);
        }

        public void ExcludeComponent<T>() where T : Component
        {
            IncludedComponents.Remove(typeof(T).Name);
            ExcludedComponents.Add(typeof(T).Name);
        }

        public void RemoveComponent<T>() where T : Component
        {
            IncludedComponents.Remove(typeof(T).Name);
            ExcludedComponents.Remove(typeof(T).Name);
        }

        public bool CompatibleWith(ComponentMask componentMask)
        {
            return IncludedComponents.All(c => componentMask.IncludedComponents.Contains(c)) &&
                   ExcludedComponents.All(c => !componentMask.IncludedComponents.Contains(c));
        }

        public bool DoesIncludeComponent<T>() where T : Component
        {
            return IncludedComponents.Contains(typeof(T).Name);
        }

        public bool DoesExcludeComponent<T>() where T : Component
        {
            return ExcludedComponents.Contains(typeof(T).Name);
        }
    }
}
