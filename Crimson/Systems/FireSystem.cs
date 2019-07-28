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
    class FireSystem : GameSystem
    {

        readonly EntityGroup<CBurning, CHealth> _burnable;
        readonly EntityGroup<CBurning, CTransform> _burning;

        readonly EntityGroup<CBurning, CTransform, CCollidable, CGameObject> _burningObjects;
        readonly EntityGroup<CBurning, CTransform, CTile> _burningTiles;

        readonly EntityGroup<CTransform, CFlammable, CCollidable, CGameObject> _flammableObjects;
        readonly Map _map;

        public FireSystem(World world, Map map)
        {
            _world = world;
            _burnable = _world.GetGroup<EntityGroup<CBurning, CHealth>>();
            _burning = _world.GetGroup<EntityGroup<CBurning, CTransform>>();
            _burningObjects = _world.GetGroup<EntityGroup<CBurning, CTransform, CCollidable, CGameObject>>();
            _burningTiles = _world.GetGroup<EntityGroup<CBurning, CTransform, CTile>>();
            _flammableObjects = _world.GetGroup<EntityGroup<CTransform, CFlammable, CCollidable, CGameObject>>();
            _map = map;
        }

        (int X, int Y) FindTile((double X, double Y) coords)
        {
            return ((int)Math.Floor(coords.X / _map.TileSize), (int)Math.Floor(coords.Y / _map.TileSize));
        }

        void SpreadEntityToTile()
        {
            foreach (var (entity, fire, transform, bounds, _) in _burningObjects)
            {
                var size = bounds.Size;
                var loc = transform.Location;

                new List<(double X, double Y)>
                {
                    (loc.X, loc.Y),
                    (loc.X + 2 * size - 1, loc.Y),
                    (loc.X, loc.Y + 2 * size - 1),
                    (loc.X + 2 * size - 1, loc.Y + 2 * size - 1)
                }
                .Select(t => FindTile(t))
                .Where(t => t.X >= 0 && t.Y >= 0 && t.X < _map.Height && t.Y < _map.Width)
                .Select(t => _map.Plan[t.X, t.Y])
                .Where(t => t.HasComponent<CFlammable>() && !t.HasComponent<CBurning>())
                .ToList()
                .ForEach(t =>
                {
                    t.AddComponent(new CBurning(fire.Spread));
                    t.ScheduleComponentForRemoval(typeof(CBurning), 50);
                });
            }
        }

        void SpreadTileToTile()
        {
            foreach (var (entity, fire, transform, _) in _burningTiles)
            {
                var x = (int)Math.Floor(transform.Location.X / _map.TileSize);
                var y = (int)Math.Floor(transform.Location.Y / _map.TileSize);

                if (fire.Spread - 1 >= 0)
                {
                    var neighbors = new List<(int X, int Y)>() { (x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1) }
                        .Where(t => t.X >= 0 && t.Y >= 0 && t.X < _map.Height && t.Y < _map.Width)
                        .Select(t => _map.Plan[t.X, t.Y])
                        .Where(t => t.HasComponent<CFlammable>())
                        .Where(t => !t.TryGetComponent(out CBurning neighborFire) || neighborFire.Spread < fire.Spread - 1);

                    foreach (var t in neighbors)
                    {
                        t.AddComponent(new CBurning(fire.Spread - 1));
                        t.ScheduleComponentForRemoval(typeof(CBurning), 50);
                    }
                }

                if (fire.Spread - Math.Sqrt(2) >= 0)
                {
                    var neighbors = new List<(int X, int Y)>() { (x - 1, y - 1), (x + 1, y + 1), (x + 1, y - 1), (x - 1, y + 1) }
                        .Where(t => t.X >= 0 && t.Y >= 0 && t.X < _map.Height && t.Y < _map.Width)
                        .Select(t => _map.Plan[t.X, t.Y])
                        .Where(t => t.HasComponent<CFlammable>())
                        .Where(t => !t.TryGetComponent(out CBurning neighborFire) || neighborFire.Spread < fire.Spread - Math.Sqrt(2));

                    foreach (var t in neighbors)
                    {
                        t.AddComponent(new CBurning(fire.Spread - Math.Sqrt(2)));
                        t.ScheduleComponentForRemoval(typeof(CBurning), 50);
                    }
                }
            }
        }

        void SpreadTileToEntity()
        {
            foreach (var (entity, transform, flame, bounds, _) in _flammableObjects)
            {
                var size = bounds.Size;
                var loc = transform.Location;

                var burningGround = new List<(double X, double Y)>
                {
                    (loc.X, loc.Y),
                    (loc.X + 2 * size - 1, loc.Y),
                    (loc.X, loc.Y + 2 * size - 1),
                    (loc.X + 2 * size - 1, loc.Y + 2 * size - 1)
                }
                .Select(t => FindTile(t))
                .Where(t => t.X >= 0 && t.Y >= 0 && t.X < _map.Height && t.Y < _map.Width)
                .Select(t => _map.Plan[t.X, t.Y])
                .Any(t => t.HasComponent<CBurning>());

                if (burningGround && !entity.HasComponent<CBurning>())
                {
                    entity.AddComponent(new CBurning(1));
                }
                else if (entity.HasComponent<CBurning>() && !burningGround)
                {
                    entity.ScheduleComponentForRemoval(typeof(CBurning), 50);
                }
            }
        }

        public override void Update()
        {
            foreach (var (entity, fire, health) in _burnable)
            {
                var newHealth = new CHealth(health.MaxHealth, health.CurrentHealth - 1);
                entity.AddComponent(newHealth);
            }

            SpreadEntityToTile();
            SpreadTileToTile();
            SpreadTileToEntity();
        }
    }
}
