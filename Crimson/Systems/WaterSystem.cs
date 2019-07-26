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
        readonly EntityGroup<CWet, CTransform, CTile> _wetTiles;
        readonly EntityGroup<CTransform, CSumbergable, CGameObject> _submergeableObjects;
        readonly EntityGroup<CWet> _wet;
        readonly Map _map;

        public WaterSystem(World world, Map map)
        {
            _world = world;
            _wetTiles = _world.GetGroup<EntityGroup<CWet, CTransform, CTile>>();
            _submergeableObjects = _world.GetGroup<EntityGroup<CTransform, CSumbergable, CGameObject>>();
            _wet = _world.GetGroup<EntityGroup<CWet>>();
            _map = map;
        }

        public override void Update()
        {
            foreach (var (entity, water, transform, _) in _wetTiles)
            {
                var x = (int)Math.Floor(transform.Location.X / _map.TileSize);
                var y = (int)Math.Floor(transform.Location.Y / _map.TileSize);
                // Spread tile -> tile
                if (entity.HasComponent<CTile>())
                {
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


                    new List<(int X, int Y)>() { (x - 1, y - 1), (x + 1, y + 1), (x + 1, y - 1), (x - 1, y + 1) }
                       .Where(t => t.X >= 0 && t.Y >= 0 && t.X < _map.Height && t.Y < _map.Width)
                       .Select(t => _map.Plan[t.X, t.Y])
                       .ToList()
                       .ForEach(e =>
                       {
                           if (water.Spread > 0 && e.HasComponent<CSumbergable>())
                           {
                               var spread = water.Spread - Math.Sqrt(2);
                               if (e.TryGetComponent(out CWet water2))
                               {
                                   spread = Math.Max(water2.Spread, spread);
                               }
                               e.AddComponent(new CWet(spread, water.Potency));
                           }
                       });
                }
            }

            // Spread tile -> entity
            foreach (var (entity, transform, _, _) in _submergeableObjects)
            {
                if (entity.TryGetComponent(out CCollidable bounds))
                {
                    var size = bounds.Size;
                    var loc = transform.Location;

                    new List<(double X, double Y)> {
                        (loc.X, loc.Y),
                        (loc.X + 2 * size, loc.Y),
                        (loc.X, loc.Y + 2 * size),
                        (loc.X + 2 * size, loc.Y + 2 * size)
                    }.Select(t => FindTile(t))
                     .ToList()
                     .ForEach(t =>
                     {
                         if (_map.Plan[t.X, t.Y].TryGetComponent(out CWet water))
                         {
                             entity.AddComponent(new CWet(water.Spread, water.Potency));
                         }
                     });
                }
            }

            (int X, int Y) FindTile((double X, double Y) coords)
            {
                return ((int)Math.Floor(coords.X / _map.TileSize), (int)Math.Floor(coords.Y / _map.TileSize));
            }

            // Remove if time ran out
            var toRemoveWater = new List<EntityHandle>();
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
