using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crimson.Components
{
    class ComponentMask
    {
        public Components IncludedComponents { get; set; } = Components.None;
        public Components ExcludedComponents { get; set; } = Components.None;

        public ComponentMask() { }

        public void IncludeComponent(Components component)
        {
            IncludedComponents |= component;
            ExcludedComponents &= ~component;
        }

        public void ExcludeComponent(Components component)
        {
            IncludedComponents &= ~component;
            ExcludedComponents |= component; 
        }

        public void RemoveComponent(Components component)
        {
            IncludedComponents &= ~component;
            ExcludedComponents &= ~component;
        }

        public bool CompatibleWith(ComponentMask other)
        {
            return ((IncludedComponents & other.IncludedComponents) == IncludedComponents) &&
                   ((ExcludedComponents & other.ExcludedComponents) == ExcludedComponents);
        }

        public bool DoesIncludeComponent(Components component)
        {
            return IncludedComponents.HasFlag(component);
        }

        public bool DoesExcludeComponent(Components component)
        {
            return ExcludedComponents.HasFlag(component);
        }

        public ComponentMask Clone()
        {
            var mask = new ComponentMask()
            {
                IncludedComponents = IncludedComponents,
                ExcludedComponents = ExcludedComponents
            };

            return mask;
        }
    }
}
