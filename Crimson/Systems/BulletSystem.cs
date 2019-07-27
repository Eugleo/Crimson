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
            foreach (var (entity, bullet) in _bullets)
            {
                if (bullet.RangeLeft <= 0)
                {
                    entity.ScheduleForDeletion();
                }
            }
        }
    }
}
