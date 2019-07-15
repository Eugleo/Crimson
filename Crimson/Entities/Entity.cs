using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crimson.Components;

namespace Crimson.Entities
{
    struct Entity : IEquatable<Entity>
    {
        int _id;

        public Entity(int id)
        {
            _id = id;
        }

        public int ID => _id;

        public bool Equals(Entity other)
        {
            return this.ID == other.ID;
        }

        static public bool operator ==(Entity a, Entity b)
        {
            return a.Equals(b);
        }

        static public bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }
    }

    struct EntityHandle
    {
        public Entity Entity { get; }
        readonly World _world;

        public EntityHandle(Entity entity, World world)
        {
            Entity = entity;
            _world = world;
        }

        public void AddComponent<T>(T c) where T : Component
        {
            _world.SetComponentOfEntity(Entity, c);
        }

        public void RemoveComponent<T>() where T : Component
        {
            _world.RemoveComponentFromEntity<T>(Entity);
        }

        public T GetComponent<T>() where T : Component
        {
            return ComponentManager<T>.Instance.LookupComponentForEntity(Entity);
        }

        public bool HasComponent<T>() where T : Component
        {
            return _world.EntityHasComponent<T>(Entity);
        }

        public void Delete()
        {
            _world.RemoveEntity(Entity);
        }

        public bool TryGetComponent<T>(out T component) where T : Component
        {

            if (HasComponent<T>())
            {
                component = GetComponent<T>();
                return true;
            }
            else
            {
                component = default;
                return false;
            }
        }
    }
}
