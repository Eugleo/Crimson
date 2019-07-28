using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crimson.Components;

namespace Crimson.Entities
{
    abstract class EntityGroup
    {
        public ComponentMask Mask;
        protected List<EntityHandle> _entities = new List<EntityHandle>();
        public World _world;

        public List<EntityHandle> Entities => _entities;

        public abstract void Add(Entity entity);
        public abstract void Remove(Entity entity);
        public abstract void UpdateComponent(Entity entity, IComponent component);
    }

    class EntityGroup<Component1> : EntityGroup, IEnumerable<(EntityHandle, Component1)>, IReadOnlyList<(EntityHandle, Component1)> 
        where Component1 : class, IComponent, new ()
    {
        public List<Component1> Components1 = new List<Component1>();

        public EntityGroup()
        {
            Mask = new ComponentMask();
            Mask.IncludeComponent(new Component1().Component);
        }

        public (EntityHandle, Component1) this[int index] => (Entities[index], Components1[index]);

        public int Count => Entities.Count;

        public override void Add(Entity entity)
        {
            var eh = new EntityHandle(entity, _world);
            _entities.Add(eh);
            Components1.Add(eh.GetComponent<Component1>());
        }

        public IEnumerator<(EntityHandle, Component1)> GetEnumerator()
        {
            foreach (var i in Enumerable.Range(0, Entities.Count))
            {
                yield return (Entities[i], Components1[i]);
            }
        }

        public override void Remove(Entity entity)
        {
            var i = _entities.FindIndex(e => e.Entity == entity);
            _entities.RemoveAt(i);
            Components1.RemoveAt(i);
        }

        public override void UpdateComponent(Entity entity, IComponent component)
        {
            if (component == default) { return; }

            var t = component.GetType();
            if (t == typeof(Component1))
            {
                var i = _entities.FindIndex(e => e.Entity == entity);
                Components1[i] = component as Component1;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class EntityGroup<Component1, Component2> : 
        EntityGroup, 
        IEnumerable<(EntityHandle, Component1, Component2)>, 
        IReadOnlyList<(EntityHandle, Component1, Component2)>
        where Component1 : class, IComponent, new()
        where Component2 : class, IComponent, new()
    {
        public List<Component1> Components1 = new List<Component1>();
        public List<Component2> Components2 = new List<Component2>();

        public int Count => Entities.Count;

        public (EntityHandle, Component1, Component2) this[int index] => (Entities[index], Components1[index], Components2[index]);

        public EntityGroup()
        {
            Mask = new ComponentMask();
            Mask.IncludeComponent(new Component1().Component);
            Mask.IncludeComponent(new Component2().Component);
        }

        public override void Add(Entity e)
        {
            var eh = new EntityHandle(e, _world);
            _entities.Add(eh);
            Components1.Add(eh.GetComponent<Component1>());
            Components2.Add(eh.GetComponent<Component2>());
        }

        public override void Remove(Entity entity)
        {
            var i = _entities.FindIndex(e => e.Entity == entity);
            _entities.RemoveAt(i);
            Components1.RemoveAt(i);
            Components2.RemoveAt(i);
        }

        public override void UpdateComponent(Entity entity, IComponent component)
        {
            if (component == default) { return; }

            var t = component.GetType();
            if (t == typeof(Component1))
            {
                var i = _entities.FindIndex(e => e.Entity == entity);
                Components1[i] = component as Component1;
            } else if (t == typeof(Component2))
            {
                var i = _entities.FindIndex(e => e.Entity == entity);
                Components2[i] = component as Component2;
            }
        }

        public IEnumerator<(EntityHandle, Component1, Component2)> GetEnumerator()
        {
            foreach (var i in Enumerable.Range(0, Math.Min(Entities.Count, Math.Min(Components1.Count, Components2.Count))))
            {
                yield return (Entities[i], Components1[i], Components2[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class EntityGroup<Component1, Component2, Component3> : 
        EntityGroup, 
        IEnumerable<(EntityHandle, Component1, Component2, Component3)>, 
        IReadOnlyList<(EntityHandle, Component1, Component2, Component3)>
        where Component1 : class, IComponent, new()
        where Component2 : class, IComponent, new()
        where Component3 : class, IComponent, new()
    {
        public List<Component1> Components1 = new List<Component1>();
        public List<Component2> Components2 = new List<Component2>();
        public List<Component3> Components3 = new List<Component3>();

        public int Count => Entities.Count;

        public (EntityHandle, Component1, Component2, Component3) this[int index] => 
            (Entities[index], Components1[index], Components2[index], Components3[index]);

        public EntityGroup()
        {
            Mask = new ComponentMask();
            Mask.IncludeComponent(new Component1().Component);
            Mask.IncludeComponent(new Component2().Component);
            Mask.IncludeComponent(new Component3().Component);
        }

        public override void Add(Entity e)
        {
            var eh = new EntityHandle(e, _world);
            _entities.Add(eh);
            Components1.Add(eh.GetComponent<Component1>());
            Components2.Add(eh.GetComponent<Component2>());
            Components3.Add(eh.GetComponent<Component3>());
        }

        public override void Remove(Entity entity)
        {
            var i = _entities.FindIndex(e => e.Entity == entity);
            _entities.RemoveAt(i);
            Components1.RemoveAt(i);
            Components2.RemoveAt(i);
            Components3.RemoveAt(i);
        }

        public override void UpdateComponent(Entity entity, IComponent component)
        {
            if (component == default) { return; }

            var t = component.GetType();
            if (t == typeof(Component1))
            {
                var i = _entities.FindIndex(e => e.Entity == entity);
                Components1[i] = component as Component1;
            }
            else if (t == typeof(Component2))
            {
                var i = _entities.FindIndex(e => e.Entity == entity);
                Components2[i] = component as Component2;
            }
            else if (t == typeof(Component3))
            {
                var i = _entities.FindIndex(e => e.Entity == entity);
                Components3[i] = component as Component3;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<(EntityHandle, Component1, Component2, Component3)> GetEnumerator()
        {
            foreach (var i in Enumerable.Range(0, Entities.Count))
            {
                yield return (Entities[i], Components1[i], Components2[i], Components3[i]);
            }
        }
    }

    class EntityGroup<Component1, Component2, Component3, Component4> :
        EntityGroup,
        IEnumerable<(EntityHandle, Component1, Component2, Component3, Component4)>,
        IReadOnlyList<(EntityHandle, Component1, Component2, Component3, Component4)>
        where Component1 : class, IComponent, new()
        where Component2 : class, IComponent, new()
        where Component3 : class, IComponent, new()
        where Component4 : class, IComponent, new()
    {
        public List<Component1> Components1 = new List<Component1>();
        public List<Component2> Components2 = new List<Component2>();
        public List<Component3> Components3 = new List<Component3>();
        public List<Component4> Components4 = new List<Component4>();

        public int Count => Entities.Count;

        public (EntityHandle, Component1, Component2, Component3, Component4) this[int index] =>
            (Entities[index], Components1[index], Components2[index], Components3[index], Components4[index]);

        public EntityGroup()
        {
            Mask = new ComponentMask();
            Mask.IncludeComponent(new Component1().Component);
            Mask.IncludeComponent(new Component2().Component);
            Mask.IncludeComponent(new Component3().Component);
            Mask.IncludeComponent(new Component4().Component);
        }

        public override void Add(Entity e)
        {
            var eh = new EntityHandle(e, _world);
            _entities.Add(eh);
            Components1.Add(eh.GetComponent<Component1>());
            Components2.Add(eh.GetComponent<Component2>());
            Components3.Add(eh.GetComponent<Component3>());
            Components4.Add(eh.GetComponent<Component4>());
        }

        public override void Remove(Entity entity)
        {
            var i = _entities.FindIndex(e => e.Entity == entity);
            _entities.RemoveAt(i);
            Components1.RemoveAt(i);
            Components2.RemoveAt(i);
            Components3.RemoveAt(i);
            Components4.RemoveAt(i);
        }

        public override void UpdateComponent(Entity entity, IComponent component)
        {
            if (component == default) { return; }

            var t = component.GetType();
            if (t == typeof(Component1))
            {
                var i = _entities.FindIndex(e => e.Entity == entity);
                Components1[i] = component as Component1;
            }
            else if (t == typeof(Component2))
            {
                var i = _entities.FindIndex(e => e.Entity == entity);
                Components2[i] = component as Component2;
            }
            else if (t == typeof(Component3))
            {
                var i = _entities.FindIndex(e => e.Entity == entity);
                Components3[i] = component as Component3;
            }
            else if (t == typeof(Component4))
            {
                var i = _entities.FindIndex(e => e.Entity == entity);
                Components4[i] = component as Component4;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<(EntityHandle, Component1, Component2, Component3, Component4)> GetEnumerator()
        {
            foreach (var i in Enumerable.Range(0, Entities.Count))
            {
                yield return (Entities[i], Components1[i], Components2[i], Components3[i], Components4[i]);
            }
        }
    }
}
