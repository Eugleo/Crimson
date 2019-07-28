using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crimson.Components;

namespace Crimson.Entities
{
    class EntityManager
    {
        public List<Entity> Entities { get; } = new List<Entity>();
        readonly World _world;

        public EntityManager(World world)
        {
            _world = world;
        }
        
        public EntityHandle CreateEntity()
        {
            var e = new Entity(Entities.Count);
            Entities.Add(e);
            return new EntityHandle(e, _world);
        }

        public void RemoveEntity(Entity e)
        {
            Entities.Remove(e);
        }

        public ComponentMask GetComponentMask(Entity e)
        {
            return e.Mask;
        }
    }
}
