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
        public Entity(int id)
        {
            ID = id;
        }

        public int ID { get; }

        public bool Equals(Entity other)
        {
            return ID == other.ID;
        }

        static public bool operator ==(Entity a, Entity b)
        {
            return a.ID == b.ID;
        }

        static public bool operator !=(Entity a, Entity b)
        {
            return a.ID != b.ID;
        }
    }

    struct EntityHandle : IEquatable<EntityHandle>
    {
        public Entity Entity { get; }
        readonly World _world;

        public EntityHandle(Entity entity, World world)
        {
            Entity = entity;
            _world = world;
        }

        public void AddComponent<T>(T c) where T : IComponent, new()
        {
            _world.AddComponentToEntity(Entity, c);
        }

        public void ScheduleForDeletion(double timeLeft)
        {
            AddComponent(new CScheduledAdd(new CCLeanup(), timeLeft));
        }

        public void ScheduleForDeletion()
        {
            ScheduleForDeletion(0);
        }

        public void ScheduleComponentForRemoval(Type component, double timeLeft)
        {
            if (TryGetComponent(out CScheduledRemove remover))
            {
                var index = remover.Components.FindIndex(f => f.Component == component);
                if (index != -1 && remover.Components[index].TimeLeft > timeLeft)
                {
                    remover.Components[index] = (component, timeLeft);
                }
                else
                {
                    remover.Components.Add((component, timeLeft));
                }
            }
            else
            {
                AddComponent(new CScheduledRemove(component, timeLeft));
            }
        }

        public void ScheduleComponentForRemoval(Type component)
        {
            ScheduleComponentForRemoval(component, 0);
        }

        public void RemoveComponent<T>() where T : IComponent, new()
        {
            _world.RemoveComponentFromEntity<T>(Entity);
        }

        public T GetComponent<T>() where T : IComponent, new ()
        {
            return _world.GetComponentForEntity<T>(Entity);
        }

        public bool HasComponent<T>() where T : IComponent, new()
        {
            return _world.EntityHasComponent<T>(Entity);
        }

        public void Delete()
        {
            _world.RemoveEntity(Entity);
        }

        public bool TryGetComponent<T>(out T component) where T : IComponent, new()
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

        public bool Equals(EntityHandle other)
        {
            return Entity == other.Entity;
        }

        public static bool operator ==(EntityHandle a, EntityHandle b)
        {
            return a.Entity == b.Entity;
        }

        public static bool operator !=(EntityHandle a, EntityHandle b)
        {
            return a.Entity != b.Entity;
        }
    }
}
