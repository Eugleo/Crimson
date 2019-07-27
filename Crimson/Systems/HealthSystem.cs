﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crimson.Entities;
using Crimson.Components;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace Crimson.Systems
{
    class HealthSystem : GameSystem
    {
        readonly EntityGroup<CHealth> _filter;

        public HealthSystem(World world)
        {
            _world = world;
            _filter = _world.GetGroup<EntityGroup<CHealth>>();
        }
        public override void Update()
        {
            foreach (var (entity, health) in _filter)
            {
                if (health.CurrentHealth <= 0)
                {
                    entity.ScheduleForDeletion();
                }
            }
        }
    }
}
