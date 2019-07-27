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

        readonly EntityGroup<COnFire, CHealth> _filter1;
        readonly EntityGroup<COnFire, CTransform> _filter2;
        readonly EntityGroup<CTransform, CFlammable, CGameObject> _filter3;
        readonly EntityGroup<COnFire> _onFire;
        readonly Map _map;

        public FireSystem(World world, Map map)
        {
            _world = world;
            _filter1 = _world.GetGroup<EntityGroup<COnFire, CHealth>>();
            _filter2 = _world.GetGroup<EntityGroup<COnFire, CTransform>>();
            _filter3 = _world.GetGroup<EntityGroup<CTransform, CFlammable, CGameObject>>();
            _onFire = _world.GetGroup<EntityGroup<COnFire>>();
            _map = map;
        }

        (int X, int Y) FindTile((double X, double Y) coords)
        {
            return ((int)Math.Floor(coords.X / _map.TileSize), (int)Math.Floor(coords.Y / _map.TileSize));
        }

        public override void Update()
        {
            // Damage
            foreach (var (entity, fire, health) in _filter1)
            {
                var newHealth = new CHealth(health.MaxHealth, health.CurrentHealth - 1);
                entity.AddComponent(newHealth);
            }

            // Spread entity -> tile
            foreach (var (entity, fire, transform) in _filter2)
            {
                if (entity.TryGetComponent(out CCollidable bounds))
                {
                    var size = bounds.Size;
                    var loc = transform.Location;

                    new List<(double X, double Y)> {
                        (loc.X, loc.Y),
                        (loc.X + 2 * size - 1, loc.Y),
                        (loc.X, loc.Y + 2 * size - 1),
                        (loc.X + 2 * size - 1, loc.Y + 2 * size - 1)
                    }.Select(t => FindTile(t))
                     .ToList()
                     .ForEach(t =>
                     {
                         if (_map.Plan[t.X, t.Y].HasComponent<CFlammable>() && !_map.Plan[t.X, t.Y].HasComponent<COnFire>())
                         {
                             _map.Plan[t.X, t.Y].AddComponent(new COnFire(fire.Spread));
                             _map.Plan[t.X, t.Y].ScheduleComponentForRemoval(typeof(COnFire), 40);
                         }
                     });
                }

                // Spread tile -> tile
                if (entity.HasComponent<CTile>())
                {
                    if (fire.Spread <= 0) { continue; }

                    var x = (int)Math.Floor(transform.Location.X / _map.TileSize);
                    var y = (int)Math.Floor(transform.Location.Y / _map.TileSize);

                    new List<(int X, int Y)>() { (x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1) }
                        .Where(t => t.X >= 0 && t.Y >= 0 && t.X < _map.Height && t.Y < _map.Width)
                        .Select(t => _map.Plan[t.X, t.Y])
                        .ToList()
                        .ForEach(e =>
                        {
                            if (e.HasComponent<CFlammable>() && entity.TryGetComponent(out CTimedRemover remover))
                            {
                                var newFire = new COnFire(fire.Spread - 1);
                                e.AddComponent(newFire);
                                e.ScheduleComponentForRemoval(typeof(COnFire), GetLifetime(remover));
                            }
                        });

                    new List<(int X, int Y)>() { (x - 1, y - 1), (x + 1, y + 1), (x + 1, y - 1), (x - 1, y + 1) }
                       .Where(t => t.X >= 0 && t.Y >= 0 && t.X < _map.Height && t.Y < _map.Width)
                       .Select(t => _map.Plan[t.X, t.Y])
                       .ToList()
                       .ForEach(e =>
                       {
                           if (e.HasComponent<CFlammable>() && entity.TryGetComponent(out CTimedRemover remover))
                           {
                               var spread = fire.Spread - Math.Sqrt(2);
                               if (e.TryGetComponent(out COnFire fire2))
                               {
                                   spread = Math.Max(fire2.Spread, spread);
                               }
                               var newFire = new COnFire(spread);
                               e.AddComponent(newFire);
                               e.ScheduleComponentForRemoval(typeof(COnFire), GetLifetime(remover));
                           }
                       });
                }
            }

            // Spread tile -> entity
            foreach (var (entity, transform, flame, _) in _filter3)
            {
                if (entity.TryGetComponent(out CCollidable bounds))
                {
                    var size = bounds.Size;
                    var loc = transform.Location;

                    new List<(double X, double Y)> {
                        (loc.X, loc.Y),
                        (loc.X + 2 * size - 1, loc.Y),
                        (loc.X, loc.Y + 2 * size - 1),
                        (loc.X + 2 * size - 1, loc.Y + 2 * size - 1)
                    }.Select(t => FindTile(t))
                     .Where(t => t.X >= 0 && t.Y >= 0 && t.X < _map.Height && t.Y < _map.Width)
                     .ToList()
                     .ForEach(t =>
                     {
                         if (_map.Plan[t.X, t.Y].TryGetComponent(out COnFire fire) && _map.Plan[t.X, t.Y].TryGetComponent(out CTimedRemover remover))
                         {
                             var newFire = new COnFire(fire.Spread);
                             entity.AddComponent(newFire);
                             entity.ScheduleComponentForRemoval(typeof(COnFire), GetLifetime(remover));
                         }
                     });
                }
            }

            double GetLifetime(CTimedRemover remover)
            {
                return remover.Components.Find(c => c.Item1 == typeof(COnFire)).Item2;
            }
        }
    }
}
