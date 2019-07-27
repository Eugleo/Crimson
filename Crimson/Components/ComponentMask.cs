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
        public HashSet<Type> IncludedComponents { get; set; } = new HashSet<Type>();
        public HashSet<Type> ExcludedComponents { get; set; } = new HashSet<Type>();

        public ComponentMask() { }

        public void IncludeComponent(Type componentType)
        {
            IncludedComponents.Add(componentType);
        }

        public void IncludeComponent<T>() where T : Component
        {
            IncludedComponents.Add(typeof(T));
            ExcludedComponents.Remove(typeof(T));
        }

        public void ExcludeComponent(Type componentType)
        {
            ExcludedComponents.Add(componentType);
        }

        public void ExcludeComponent<T>() where T : Component
        {
            IncludedComponents.Remove(typeof(T));
            ExcludedComponents.Add(typeof(T));
        }

        public void RemoveComponent<T>() where T : Component
        {
            IncludedComponents.Remove(typeof(T));
            ExcludedComponents.Remove(typeof(T));
        }

        public bool CompatibleWith(ComponentMask componentMask)
        {
            return IncludedComponents.All(c => componentMask.IncludedComponents.Contains(c)) &&
                   ExcludedComponents.All(c => !componentMask.IncludedComponents.Contains(c));
        }

        public bool DoesIncludeComponent<T>() where T : Component
        {
            return IncludedComponents.Contains(typeof(T));
        }

        public bool DoesExcludeComponent<T>() where T : Component
        {
            return ExcludedComponents.Contains(typeof(T));
        }

        public ComponentMask Clone()
        {
            var mask = new ComponentMask();
            foreach (var component in IncludedComponents) { mask.IncludeComponent(component); }
            foreach (var component in ExcludedComponents) { mask.ExcludeComponent(component); }
            return mask;
        }
    }
}
