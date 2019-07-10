using System;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Crimson.Entities;
using Crimson.Components;

namespace Crimson.Systems
{
    class GunSystem : GameSystem
    {
        EntityFilter<CGun, CTransform, CShootEvent> _filter;

        public GunSystem(World world)
        {
            _world = world;
            _filter = _world.GetFilter<EntityFilter<CGun, CTransform, CShootEvent>>();
        }

        public override void Update()
        {
            foreach (var (entity, gun, transform, shot) in _filter)
            {
                var targetPosition = shot.TargetLocation;
                var position = transform.Location;

                switch (gun.Type)
                {
                    case CGun.GunType.Pistol:
                        ShootPistol(position, targetPosition, gun, entity);
                        break;
                    case CGun.GunType.SMG:
                        ShootSMG(position, targetPosition, gun, entity);
                        break;
                    case CGun.GunType.Shotgun:
                        ShootShotgun(position, targetPosition, gun, entity);
                        break;
                    default:
                        break;
                }
            }
        }

        readonly Random rnd = new Random();
        async void ShootPistol(Vector position, Vector targetPosition, CGun gun, Entity e)
        {
            if (gun.CanShoot)
            {
                MakeBullet(position, targetPosition, gun.Damage, gun.BulletSpeed, new Vector(Inaccuracy(gun.Inaccuracy), Inaccuracy(gun.Inaccuracy)));
                gun.CanShoot = false;
                _world.SetComponentOfEntity(e, gun);
                await Task.Delay(gun.Cadence);
                gun.CanShoot = true;
                _world.SetComponentOfEntity(e, gun);
            }
        }

        async void ShootSMG(Vector position, Vector targetPosition, CGun gun, Entity e)
        {
            if (gun.CanShoot)
            {
                MakeBullet(position, targetPosition, gun.Damage, gun.BulletSpeed, new Vector(Inaccuracy(gun.Inaccuracy), Inaccuracy(gun.Inaccuracy)));
                gun.CanShoot = false;
                _world.SetComponentOfEntity(e, gun);
                await Task.Delay(gun.Cadence);
                gun.CanShoot = true;
                _world.SetComponentOfEntity(e, gun);
            }
        }

        async void ShootShotgun(Vector position, Vector targetPosition, CGun gun, Entity e)
        {
            if (gun.CanShoot)
            {
                var acc = (targetPosition - position).Orthogonalized().Normalized(8);

                foreach (var i in Enumerable.Range(-2, 5))
                {
                    MakeBullet(position, targetPosition, gun.Damage, gun.BulletSpeed, acc.ScaledBy(i));
                }

                gun.CanShoot = false;
                _world.SetComponentOfEntity(e, gun);
                // TODO tohle info mít v nějakém dictionary s daty různých druhů zbraní
                await Task.Delay(gun.Cadence);
                gun.CanShoot = true;
                _world.SetComponentOfEntity(e, gun);
            }
        }

        void MakeBullet(Vector position, Vector targetPosition, int damage, double speed, Vector offset)
        {
            var acc = (targetPosition - position).Normalized(100);
            var bullet = _world.CreateEntity();
            bullet.AddComponent(new CBullet(damage));
            bullet.AddComponent(new CMovement(speed, (acc + offset).Normalized()));
            bullet.AddComponent(new CTransform(position));
            bullet.AddComponent(new CGraphics(MainForm.ResizeImage(Properties.Resources.bullet, 20, 20)));
            bullet.AddComponent(new CGameObject());
        }

        int Inaccuracy(int upperBound)
        {
            return rnd.Next(2) == 0 ? -rnd.Next(upperBound + 1) : rnd.Next(upperBound + 1);
        }
    }
}
