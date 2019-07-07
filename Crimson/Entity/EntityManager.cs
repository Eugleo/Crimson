using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crimson
{
    class EntityManager
    {
        public List<Entity> Entities { get; } = new List<Entity>();
        readonly Dictionary<Entity, ComponentMask> _masks = new Dictionary<Entity, ComponentMask>();
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

        public void RemoveEntity(EntityHandle e)
        {
            Entities.Remove(e.Entity);
        }

        public void SetComponentMask(Entity e, ComponentMask mask)
        {
            _masks[e] = mask;
        }

        public ComponentMask GetComponentMask(Entity e)
        {
            if (!_masks.ContainsKey(e))
            {
                _masks[e] = new ComponentMask();
            }
            return _masks[e];
        }
    }
}
