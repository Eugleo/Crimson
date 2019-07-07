using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crimson
{
    struct Entity: IEquatable<Entity>
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
    }

    struct EntityHandle
    {
        readonly Entity _entity;
        readonly World _world;

        public EntityHandle(Entity entity, World world)
        {
            _entity = entity;
            _world = world;
        }

        public void SetComponent<Component>(Component c)
        {
            _world.AddComponentToEntity<Component>(_entity, c);
        }

        public void RemoveComponent<Component>()
        {
            _world.RemoveComponentFromEntity<Component>(_entity);
        }

        public Entity Entity => _entity;
    }
}
