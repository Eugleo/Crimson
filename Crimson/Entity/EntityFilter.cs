using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crimson
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

    class EntityFilter<Component1> : EntityFilter
    {
        public List<Component1> Components1 = new List<Component1>();

        public EntityFilter()
        {
            Mask = new ComponentMask();
            Mask.IncludeComponent<Component1>();
        }

        public override void Add(Entity e)
        {
            _entities.Add(e);
            Components1.Add(_world.GetComponentForEntity<Component1>(e));
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
    }

    class EntityFilter<Component1, Component2> : EntityFilter
    {
        public List<Component1> Components1 = new List<Component1>();
        public List<Component2> Components2 = new List<Component2>();

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
    }

    class EntityFilter<Component1, Component2, Component3> : EntityFilter 
    {
        public List<Component1> Components1 = new List<Component1>();
        public List<Component2> Components2 = new List<Component2>();
        public List<Component3> Components3 = new List<Component3>();

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
    }
}
