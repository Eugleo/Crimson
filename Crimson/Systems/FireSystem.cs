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
        readonly EntityGroup<COnFire> _filter4;
        readonly Map _map;

        public FireSystem(World world, Map map)
        {
            _world = world;
            _filter1 = _world.GetGroup<EntityGroup<COnFire, CHealth>>();
            _filter2 = _world.GetGroup<EntityGroup<COnFire, CTransform>>();
            _filter3 = _world.GetGroup<EntityGroup<CTransform, CFlammable, CGameObject>>();
            _filter4 = _world.GetGroup<EntityGroup<COnFire>>();
            _map = map;
        }

        public override void Update()
        {
            // Damage
            foreach (var (entity, fire, health) in _filter1)
            {
                var newHealth = new CHealth(health.MaxHealth, health.CurrentHealth - 0.1);
                entity.AddComponent(newHealth);
                entity.AddComponent(new COnFire(fire.Spread, fire.Longevity - 0.1));
            }

            // Spread entity -> tile
            foreach (var (entity, fire, transform) in _filter2)
            {
                var x = (int)Math.Floor(transform.Location.X / _map.TileSize);
                var y = (int)Math.Floor(transform.Location.Y / _map.TileSize);
                if (_map.Plan[x, y].Entity == entity.Entity) { continue; }

                if (_map.Plan[x, y].HasComponent<CFlammable>())
                {
                    _map.Plan[x, y].AddComponent(new COnFire(fire.Spread - 1, fire.Longevity));
                }

                // Spread tile -> tile
                if (entity.HasComponent<CTile>())
                {
                    // TODO Opravit indexy
                    new List<EntityHandle>() { _map.Plan[x - 1, y], _map.Plan[x + 1, y], _map.Plan[x, y - 1], _map.Plan[x, y + 1] }
                        .ForEach(e =>
                        {
                            if (e.HasComponent<CFlammable>())
                            {
                                e.AddComponent(new COnFire(fire.Spread - 1, fire.Longevity));
                            }
                        });
                }
            }

            // Spread tile -> entity
            foreach (var (entity, transform, flame, _) in _filter3)
            {
                var x = (int)Math.Floor(transform.Location.X / _map.TileSize);
                var y = (int)Math.Floor(transform.Location.Y / _map.TileSize);
                if (_map.Plan[x, y].TryGetComponent(out COnFire fire))
                {
                    entity.AddComponent(new COnFire(fire.Spread - 1, fire.Longevity));
                    entity.AddComponent(new CGraphics(flame.Image));
                }
            }

            // Douse if time ran out
            foreach (var (entity, fire) in _filter4)
            {
                if (fire.Longevity <= 0)
                {
                    entity.RemoveComponent<COnFire>();
                }
            }
        }
    }
}
