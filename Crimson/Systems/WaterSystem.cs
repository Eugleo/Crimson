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
        readonly EntityGroup<CTransform, CCollidable, CSumbergable, CGameObject> _submergeableObjects;
        readonly Map _map;

        public WaterSystem(World world, Map map)
        {
            _world = world;
            _wetTiles = _world.GetGroup<EntityGroup<CWet, CTransform, CTile>>();
            _submergeableObjects = _world.GetGroup<EntityGroup<CTransform, CCollidable, CSumbergable, CGameObject>>();
            _map = map;
        }

        public override void Update()
        {
            SpreadTileToTile();
            SpreadTileToEntity();
        }

        void SpreadTileToTile()
        {
            foreach (var (entity, water, transform, _) in _wetTiles)
            {
                var x = (int)Math.Floor(transform.Location.X / _map.TileSize);
                var y = (int)Math.Floor(transform.Location.Y / _map.TileSize);

                if (water.Spread - 1 >= 0)
                {
                    var neighbors = new List<(int X, int Y)>() { (x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1) }
                       .Where(t => t.X >= 0 && t.Y >= 0 && t.X < _map.Height && t.Y < _map.Width)
                       .Select(t => _map.Plan[t.X, t.Y])
                       .Where(t => t.HasComponent<CSumbergable>())
                       .Where(t => !t.TryGetComponent(out CWet neighborWater) || neighborWater.Spread < water.Spread - 1);

                    foreach (var t in neighbors) { t.AddComponent(new CWet(water.Spread - 1)); }
                }

                if (water.Spread - Math.Sqrt(2) >= 0)
                {
                    var neighbors = new List<(int X, int Y)>() { (x - 1, y - 1), (x + 1, y + 1), (x + 1, y - 1), (x - 1, y + 1) }
                       .Where(t => t.X >= 0 && t.Y >= 0 && t.X < _map.Height && t.Y < _map.Width)
                       .Select(t => _map.Plan[t.X, t.Y])
                       .Where(t => t.HasComponent<CSumbergable>())
                       .Where(t => !t.TryGetComponent(out CWet neighborWater) || neighborWater.Spread < water.Spread - Math.Sqrt(2));

                    foreach (var t in neighbors) { t.AddComponent(new CWet(water.Spread - Math.Sqrt(2))); }
                }
            }
        }

        void SpreadTileToEntity()
        {
            (int X, int Y) FindTile((double X, double Y) coords)
            {
                return ((int)Math.Floor(coords.X / _map.TileSize), (int)Math.Floor(coords.Y / _map.TileSize));
            }

            foreach (var (entity, transform, bounds, _, _) in _submergeableObjects)
            {
                var size = bounds.Size;
                var loc = transform.Location;

                var wetGround = new List<(double X, double Y)>
                {
                    (loc.X, loc.Y),
                    (loc.X + 2 * size, loc.Y),
                    (loc.X, loc.Y + 2 * size),
                    (loc.X + 2 * size, loc.Y + 2 * size)
                }
                .Select(t => FindTile(t))
                .Where(t => t.X >= 0 && t.Y >= 0 && t.X < _map.Height && t.Y < _map.Width)
                .Select(t => _map.Plan[t.X, t.Y])
                .Any(t => t.HasComponent<CWet>());

                if (wetGround && !entity.HasComponent<CWet>())
                {
                    entity.AddComponent(new CWet(0));
                }
                else if (entity.HasComponent<CWet>() && !wetGround)
                {
                    entity.ScheduleComponentForRemoval(typeof(CWet), 40);
                }
            }
        }
    }
}
