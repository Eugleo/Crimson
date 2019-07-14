using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crimson.Entities;
using Crimson.Components;


namespace Crimson.Systems
{
    class BulletSystem : GameSystem
    {
        readonly EntityGroup<CBullet> _bullets;

        public BulletSystem(World world)
        {
            _world = world;
            _bullets = _world.GetGroup<EntityGroup<CBullet>>();
        }

        public override void Update()
        {
            var toRemove = new List<EntityHandle>();
            foreach (var (entity, bullet) in _bullets)
            {
                if (bullet.RangeLeft <= 0)
                {
                    toRemove.Add(entity);
                }
            }
            toRemove.ForEach(e => e.Delete());
        }
    }
}
