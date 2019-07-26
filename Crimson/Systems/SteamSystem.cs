using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Crimson.Entities;
using Crimson.Components;

namespace Crimson.Systems
{
    class SteamSystem : GameSystem
    {
        EntityGroup<CWet, COnFire, CTransform> _wetHot_InYourArea;

        public SteamSystem(World world)
        {
            _world = world;
            _wetHot_InYourArea = _world.GetGroup<EntityGroup<CWet, COnFire, CTransform>>();
        }

        public override void Update()
        {
            var toRemove = new List<EntityHandle>();
            foreach (var (entity, water, fire, transform) in _wetHot_InYourArea)
            {
                toRemove.Add(entity);
                var count = 0;
                switch (rnd.Next(6))
                {
                    case 0:
                    case 1:
                        count = 1;
                        break;
                    case 2:
                    case 3:
                    case 4:
                        count = 2;
                        break;
                    case 5:
                        count = 3;
                        break;
                }
                foreach (var _ in Enumerable.Range(0, count)) { CreateCloud(transform.Location); }
            }
            toRemove.ForEach(e => {
                e.RemoveComponent<COnFire>();
                e.RemoveComponent<CWet>();
            });
        }

        Random rnd = new Random();
        void CreateCloud(Vector location)
        {
            var cloud = _world.CreateEntity();
            cloud.AddComponent(new CAbove());
            var size = rnd.Next(55, 75);
            cloud.AddComponent(new CGraphics(MainForm.ResizeImage(Properties.Resources.cloud, size, size)));
            cloud.AddComponent(new CTransform(location + new Vector(Sign() * rnd.Next(10), Sign() * rnd.Next(10))));
        }

        int Sign()
        {
            return rnd.Next(2) == 0 ? -1 : 1;
        }
    }
}
