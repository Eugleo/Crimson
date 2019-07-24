using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crimson.Entities;
using Crimson.Components;
using System.Diagnostics;

namespace Crimson.Systems
{
    class WaterSystem : GameSystem
    {
        readonly EntityGroup<CWet, CTransform, CTile> _filter2;
        readonly EntityGroup<CTransform, CSumbergable, CGameObject> _filter3;
        readonly EntityGroup<COnFire, CWet> _filter4;
        readonly EntityGroup<CWet> _wet;
        readonly Map _map;

        public WaterSystem(World world, Map map)
        {
            _world = world;
            _filter2 = _world.GetGroup<EntityGroup<CWet, CTransform, CTile>>();
            _filter3 = _world.GetGroup<EntityGroup<CTransform, CSumbergable, CGameObject>>();
            _filter4 = _world.GetGroup<EntityGroup<COnFire, CWet>>();
            _wet = _world.GetGroup<EntityGroup<CWet>>();
            _map = map;
        }

        public override void Update()
        {
            foreach (var (entity, water, transform, _) in _filter2)
            {
                var x = (int)Math.Floor(transform.Location.X / _map.TileSize);
                var y = (int)Math.Floor(transform.Location.Y / _map.TileSize);
                // Spread tile -> tile
                if (entity.HasComponent<CTile>())
                {
                    // TODO přidat digaonály
                    new List<(int X, int Y)>() { (x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1) }
                        .Where(t => t.X >= 0 && t.Y >= 0 && t.X < _map.Height && t.Y < _map.Width)
                        .Select(t => _map.Plan[t.X, t.Y])
                        .ToList()
                        .ForEach(e =>
                        {
                            if (water.Spread > 0 && e.HasComponent<CSumbergable>())
                            {
                                e.AddComponent(new CWet(water.Spread - 1, water.Potency));
                            }
                        });
                }
            }

            // Spread tile -> entity
            foreach (var (entity, transform, _, _) in _filter3)
            {
                var x = (int)Math.Floor(transform.Location.X / _map.TileSize);
                var y = (int)Math.Floor(transform.Location.Y / _map.TileSize);
                if (_map.Plan[x, y].TryGetComponent(out CWet water))
                {
                    entity.AddComponent(new CWet(water.Spread, water.Potency));
                }
            }

            // Remove if time ran out
            var toRemoveFire = new List<EntityHandle>();
            var toRemoveWater = new List<EntityHandle>();
            foreach (var (entity, fire, water) in _filter4)
            {
                toRemoveFire.Add(entity);

                if (water.Potency - fire.Longevity <= 0)
                {
                    toRemoveWater.Add(entity);
                }
                else
                {
                    entity.AddComponent(new CWet(water.Spread, water.Potency - fire.Longevity));
                }
            }
            toRemoveFire.ForEach(e => e.RemoveComponent<COnFire>());

            foreach (var (entity, water) in _wet)
            {
                if (entity.HasComponent<CTile>()) { continue; }
                entity.AddComponent(new CWet(water.Spread, water.Potency - 0.1));
                if (water.Potency <= 0)
                {
                    toRemoveWater.Add(entity);
                }
            }
            toRemoveWater.ForEach(e => e.RemoveComponent<CWet>());
        }
    }
}
