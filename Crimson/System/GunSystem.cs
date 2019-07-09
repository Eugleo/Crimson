using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Crimson
{
    class GunSystem : GameSystem
    {
        EntityFilter<CHasGun, CTransform, CShootEvent> _filter;

        public GunSystem(World world)
        {
            _world = world;
            _filter = _world.GetFilter<EntityFilter<CHasGun, CTransform, CShootEvent>>();
        }

        public override void Update()
        {
            foreach (var (entity, gun, transform, shot) in _filter)
            {
                var targetPosition = shot.TargetLocation;
                var position = transform.Location;

                switch (gun.Type)
                {
                    case CHasGun.GunType.Pistol:
                        ShootPistol(position, targetPosition, gun, entity);
                        break;
                    case CHasGun.GunType.SMG:
                        ShootSMG(position, targetPosition, gun, entity);
                        break;
                    case CHasGun.GunType.Shotgun:
                        ShootShotgun(position, targetPosition, gun, entity);
                        break;
                    default:
                        break;
                }
            }
        }

        readonly Random rnd = new Random();
        async void ShootPistol(Vector position, Vector targetPosition, CHasGun gun, Entity e)
        {
            if (gun.CanShoot)
            {
                MakeBullet(position, targetPosition, 20, 30, new Vector(Inaccuracy(5), Inaccuracy(5)));
                gun.CanShoot = false;
                _world.AddComponentToEntity(e, gun);
                // TODO tohle info mít v nějakém dictionary s daty různých druhů zbraní
                await Task.Delay(300);
                gun.CanShoot = true;
                _world.AddComponentToEntity(e, gun);
            }
        }

        async void ShootSMG(Vector position, Vector targetPosition, CHasGun gun, Entity e)
        {
            if (gun.CanShoot)
            {
                MakeBullet(position, targetPosition, 10, 30, new Vector(Inaccuracy(12), Inaccuracy(12)));
                gun.CanShoot = false;
                _world.AddComponentToEntity(e, gun);
                // TODO tohle info mít v nějakém dictionary s daty různých druhů zbraní
                await Task.Delay(45);
                gun.CanShoot = true;
                _world.AddComponentToEntity(e, gun);
            }
        }

        async void ShootShotgun(Vector position, Vector targetPosition, CHasGun gun, Entity e)
        {
            if (gun.CanShoot)
            {
                var acc = (targetPosition - position).Orthogonalized().Normalized(8);

                foreach (var i in Enumerable.Range(-2, 5))
                {
                    MakeBullet(position, targetPosition, 20, 15, acc.ScaledBy(i));
                }

                gun.CanShoot = false;
                _world.AddComponentToEntity(e, gun);
                // TODO tohle info mít v nějakém dictionary s daty různých druhů zbraní
                await Task.Delay(650);
                gun.CanShoot = true;
                _world.AddComponentToEntity(e, gun);
            }
        }

        void MakeBullet(Vector position, Vector targetPosition, int damage, int speed, Vector offset)
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
