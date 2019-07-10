using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crimson.Components;

namespace Crimson.Entities
{
    abstract class EntityFilter
    {
        public ComponentMask Mask;
        protected List<Entity> _entities = new List<Entity>();
        public World _world;

        public List<Entity> Entities => _entities;

        public abstract void Add(Entity e);
        public abstract void Remove(Entity e);
        public abstract void UpdateComponentsFor(Entity entity);
    }

    class EntityFilter<Component1> : EntityFilter, IEnumerable<(Entity, Component1)>, IReadOnlyList<(Entity, Component1)>
    {
        public List<Component1> Components1 = new List<Component1>();

        public EntityFilter()
        {
            Mask = new ComponentMask();
            Mask.IncludeComponent<Component1>();
        }

        public (Entity, Component1) this[int index] => (Entities[index], Components1[index]);

        public int Count => Entities.Count;

        public override void Add(Entity e)
        {
            _entities.Add(e);
            Components1.Add(_world.GetComponentForEntity<Component1>(e));
        }

        public IEnumerator<(Entity, Component1)> GetEnumerator()
        {
            foreach (var i in Enumerable.Range(0, Entities.Count))
            {
                yield return (Entities[i], Components1[i]);
            }
        }

        public override void Remove(Entity e)
        {
            var i = _entities.IndexOf(e);
            _entities.RemoveAt(i);
            Components1.RemoveAt(i);
        }

        public override void UpdateComponentsFor(Entity entity)
        {
            var i = _entities.IndexOf(entity);
            Components1[i] = _world.GetComponentForEntity<Component1>(entity);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class EntityFilter<Component1, Component2> : 
        EntityFilter, 
        IEnumerable<(Entity, Component1, Component2)>, 
        IReadOnlyList<(Entity, Component1, Component2)>
    {
        public List<Component1> Components1 = new List<Component1>();
        public List<Component2> Components2 = new List<Component2>();

        public int Count => Entities.Count;

        public (Entity, Component1, Component2) this[int index] => (Entities[index], Components1[index], Components2[index]);

        public EntityFilter()
        {
            Mask = new ComponentMask();
            Mask.IncludeComponent<Component1>();
            Mask.IncludeComponent<Component2>();
        }

        public override void Add(Entity e)
        {
            _entities.Add(e);
            Components1.Add(_world.GetComponentForEntity<Component1>(e));
            Components2.Add(_world.GetComponentForEntity<Component2>(e));
        }

        public override void Remove(Entity e)
        {
            var i = _entities.IndexOf(e);
            _entities.RemoveAt(i);
            Components1.RemoveAt(i);
            Components2.RemoveAt(i);
        }

        public override void UpdateComponentsFor(Entity entity)
        {
            var i = _entities.IndexOf(entity);
            Components1[i] = _world.GetComponentForEntity<Component1>(entity);
            Components2[i] = _world.GetComponentForEntity<Component2>(entity);
        }

        public IEnumerator<(Entity, Component1, Component2)> GetEnumerator()
        {
            foreach (var i in Enumerable.Range(0, Entities.Count))
            {
                yield return (Entities[i], Components1[i], Components2[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class EntityFilter<Component1, Component2, Component3> : 
        EntityFilter, 
        IEnumerable<(Entity, Component1, Component2, Component3)>, 
        IReadOnlyList<(Entity, Component1, Component2, Component3)>
    {
        public List<Component1> Components1 = new List<Component1>();
        public List<Component2> Components2 = new List<Component2>();
        public List<Component3> Components3 = new List<Component3>();

        public int Count => Entities.Count;

        public (Entity, Component1, Component2, Component3) this[int index] => 
            (Entities[index], Components1[index], Components2[index], Components3[index]);

        public EntityFilter()
        {
            Mask = new ComponentMask();
            Mask.IncludeComponent<Component1>();
            Mask.IncludeComponent<Component2>();
            Mask.IncludeComponent<Component3>();
        }

        public override void Add(Entity e)
        {
            _entities.Add(e);
            Components1.Add(_world.GetComponentForEntity<Component1>(e));
            Components2.Add(_world.GetComponentForEntity<Component2>(e));
            Components3.Add(_world.GetComponentForEntity<Component3>(e));
        }

        public override void Remove(Entity e)
        {
            var i = _entities.IndexOf(e);
            _entities.RemoveAt(i);
            Components1.RemoveAt(i);
            Components2.RemoveAt(i);
            Components3.RemoveAt(i);
        }

        public override void UpdateComponentsFor(Entity entity)
        {
            var i = _entities.IndexOf(entity);
            Components1[i] = _world.GetComponentForEntity<Component1>(entity);
            Components2[i] = _world.GetComponentForEntity<Component2>(entity);
            Components3[i] = _world.GetComponentForEntity<Component3>(entity);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<(Entity, Component1, Component2, Component3)> GetEnumerator()
        {
            foreach (var i in Enumerable.Range(0, Entities.Count))
            {
                yield return (Entities[i], Components1[i], Components2[i], Components3[i]);
            }
        }
    }
}
