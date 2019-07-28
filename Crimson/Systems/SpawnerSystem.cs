using Crimson.Components;
using Crimson.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Crimson.Systems
{
    class SpawnerSystem : GameSystem
    {
        readonly EntityGroup<CSpawner> _spawners;
        readonly EntityGroup<CTransform, CTile> _tiles;
        readonly EntityGroup<CGameObject, CFaction> _possibleEnemies;
        readonly EntityHandle _player;
        readonly Dictionary<CGun.ShootingPattern, CGun> _guns = new Dictionary<CGun.ShootingPattern, CGun>();

        public SpawnerSystem(World world, EntityHandle player)
        {
            _world = world;
            _player = player;
            _spawners = _world.GetGroup<EntityGroup<CSpawner>>();
            _tiles = _world.GetGroup<EntityGroup<CTransform, CTile>>();
            _possibleEnemies = _world.GetGroup<EntityGroup<CGameObject, CFaction>>();
            DefineGuns();
        }

        readonly Random rnd = new Random();
        public override void Update()
        {
            if (_possibleEnemies.Where(e => !e.Item1.HasComponent<CBullet>()).Count() > 30) { return; }

            foreach (var (entity, spawner) in _spawners)
            {
                if (!spawner.CanSpawn) { continue; }

                var freeLocations = _tiles.Where(t => !t.Item3.Occupied).Select(t => t.Item2.Location).ToList();
                var location = freeLocations[rnd.Next(freeLocations.Count)];

                MakeEnemy(location);
                CoolDown(spawner);
            }
        }

        async void CoolDown(CSpawner spawner)
        {
            spawner.CanSpawn = false;
            await Task.Delay(spawner.SpawnRate);
            spawner.CanSpawn = true;
        }

        void DefineGuns()
        {
            _guns[CGun.ShootingPattern.Pistol] = new CGun(CGun.ShootingPattern.Pistol, 7, 1000, 5, 600, 35, 280, 4);
            _guns[CGun.ShootingPattern.Shotgun] = new CGun(CGun.ShootingPattern.Shotgun, 4, 1800, 0, 900, 30, 200, 2);
            _guns[CGun.ShootingPattern.SMG] = new CGun(CGun.ShootingPattern.SMG, 2, 1400, 3, 150, 40, 280, 10);
            _guns[CGun.ShootingPattern.Grenade] = new CGun(CGun.ShootingPattern.Grenade, 5, 8000, 0, 400, 15, 280, 1);
        }

        void MakeRanger(EntityHandle entity)
        {
            Image image = Utilities.ResizeImage(Properties.Resources.ranger, 64, 64);
            entity.AddComponent(new CGraphics(image));
            entity.AddComponent(new CMovement(6, new Vector(0, 0)));

            CGun gun;
            switch (rnd.Next(5))
            {
                case 0:
                    gun = _guns[CGun.ShootingPattern.Grenade];
                    break;
                case 1:
                    gun = _guns[CGun.ShootingPattern.Shotgun];
                    break;
                case 2:
                    gun = _guns[CGun.ShootingPattern.SMG];
                    break;
                default:
                    gun = _guns[CGun.ShootingPattern.Pistol];
                    break;
            }
            entity.AddComponent(gun);
            entity.AddComponent(new CPursuitBehavior(_player, 5, 1, (int)gun.Range - 20));
        }

        void MakeWarrior(EntityHandle entity)
        {
            Image image = Utilities.ResizeImage(Properties.Resources.warrior, 64, 64);
            entity.AddComponent(new CGraphics(image));
            entity.AddComponent(new CMovement(5, new Vector(0, 0)));
            entity.AddComponent(new CPursuitBehavior(_player, 5, 1, 30));
            entity.AddComponent(new CHasMeleeWeapon(70, 300, 20, true));
        }

        void MakeEnemy(Vector location)
        {
            var enemy = _world.CreateEntity();

            enemy.AddComponent(new CTransform(location));
            enemy.AddComponent(new CGameObject());
            enemy.AddComponent(new CCollidable(32));
            enemy.AddComponent(new CFaction(Faction.NPC));
            enemy.AddComponent(new CHealth(50, 50));
            enemy.AddComponent(new CAvoidObstaclesBehavior(5, 2, MakeFeelers(new int[] { 0, 20, 40, 60, 80 })));
            enemy.AddComponent(new CAttacker(_player));
            enemy.AddComponent(new CFlammable(Utilities.ResizeImage(Properties.Resources.ohen, 64, 64)));
            enemy.AddComponent(new CSumbergable(Utilities.ResizeImage(Properties.Resources.water, 64, 64)));

            if (rnd.Next(2) == 0)
            {
                MakeWarrior(enemy);
            }
            else
            {
                MakeRanger(enemy);
            }
        }

        List<(EntityHandle, int)> MakeFeelers(int[] distances)
        {
            var acc = new List<(EntityHandle, int)>();
            foreach (var d in distances)
            {
                acc.Add((MakeFeeler(new Vector(32, 0)), d));
                acc.Add((MakeFeeler(new Vector(32, 64)), d));
                acc.Add((MakeFeeler(new Vector(64, 32)), d));
                acc.Add((MakeFeeler(new Vector(0, 32)), d));
            }
            return acc;
        }

        EntityHandle MakeFeeler(Vector offset)
        {
            var feeler = _world.CreateEntity();
            feeler.AddComponent(new CMovement(0, new Vector(0, 0)));
            feeler.AddComponent(new CTransform(0, 0));
            feeler.AddComponent(new CFeeler(offset));
            return feeler;
        }
    }
}
