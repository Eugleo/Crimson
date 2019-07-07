using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crimson
{
    class ComponentMask
    {
        public HashSet<int> Mask { get; } = new HashSet<int>();

        public ComponentMask() { }

        public void IncludeComponent<Component>()
        {
            Mask.Add(ComponentManager<Component>.Instance.ID);
        }

        public void ExcludeComponent<Component>()
        {
            Mask.Remove(ComponentManager<Component>.Instance.ID);
        }

        public bool CompatibleWith(ComponentMask componentMask)
        {
            return Mask.Intersect(componentMask.Mask).Count() == Mask.Count;
        }

        public bool IncludesComponent<Component>()
        {
            return Mask.Contains(ComponentManager<Component>.Instance.ID);
        }
    }
}
