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
        readonly EntityFilter<CGun, CTransform, CShootEvent> _filter;

        public GunSystem(World world)
        {
            _world = world;
            _filter = _world.GetFilter<EntityFilter<CGun, CTransform, CShootEvent>>();
        }

        // TODO Refaktorovat MakeBullet
        public override void Update()
        {
            foreach (var (entity, gun, transform, shot) in _filter)
            {
                var targetPosition = shot.TargetLocation;
                var position = transform.Location;
                var startPosition = position;
                int offset = 0;

                if (_world.EntityHasComponent<CGraphics>(entity))
                {
                    var image = _world.GetComponentForEntity<CGraphics>(entity).Image;
                    position += new Vector(image.Width / 2, image.Height / 2);
                    offset = (int)Math.Ceiling(new Vector(image.Width / 2, image.Height / 2).Size);
                    startPosition = position + (targetPosition - position).Normalized(offset);
                }

                switch (gun.Type)
                {
                    case CGun.GunType.Pistol:
                        ShootPistol(position, targetPosition, gun, entity, startPosition);
                        break;
                    case CGun.GunType.SMG:
                        ShootSMG(position, targetPosition, gun, entity, startPosition);
                        break;
                    case CGun.GunType.Shotgun:
                        ShootShotgun(position, targetPosition, gun, entity, startPosition);
                        break;
                    default:
                        break;
                }
            }
        }

        readonly Random rnd = new Random();
        async void ShootPistol(Vector position, Vector targetPosition, CGun gun, Entity e, Vector startPosition)
        {
            if (gun.CanShoot)
            {
                MakeBullet(position, targetPosition, gun.Damage, gun.BulletSpeed, new Vector(Inaccuracy(gun.Inaccuracy), Inaccuracy(gun.Inaccuracy)), startPosition);
                gun.CanShoot = false;
                _world.SetComponentOfEntity(e, gun);
                await Task.Delay(gun.Cadence);
                gun.CanShoot = true;
                _world.SetComponentOfEntity(e, gun);
            }
        }

        async void ShootSMG(Vector position, Vector targetPosition, CGun gun, Entity e, Vector startPosition)
        {
            if (gun.CanShoot)
            {
                MakeBullet(position, targetPosition, gun.Damage, gun.BulletSpeed, new Vector(Inaccuracy(gun.Inaccuracy), Inaccuracy(gun.Inaccuracy)), startPosition);
                gun.CanShoot = false;
                _world.SetComponentOfEntity(e, gun);
                await Task.Delay(gun.Cadence);
                gun.CanShoot = true;
                _world.SetComponentOfEntity(e, gun);
            }
        }

        async void ShootShotgun(Vector position, Vector targetPosition, CGun gun, Entity e, Vector startPosition)
        {
            if (gun.CanShoot)
            {
                var acc = (targetPosition - position).Orthogonalized().Normalized(8);

                foreach (var i in Enumerable.Range(-2, 5))
                {
                    MakeBullet(position, targetPosition, gun.Damage, gun.BulletSpeed, acc.ScaledBy(i), startPosition);
                }

                gun.CanShoot = false;
                _world.SetComponentOfEntity(e, gun);
                await Task.Delay(gun.Cadence);
                gun.CanShoot = true;
                _world.SetComponentOfEntity(e, gun);
            }
        }

        void MakeBullet(Vector position, Vector targetPosition, int damage, double speed, Vector offset, Vector startPosition)
        {
            var acc = (targetPosition - position).Normalized(100);
            var bullet = _world.CreateEntity();
            bullet.AddComponent(new CBullet(damage));
            bullet.AddComponent(new CMovement(speed, (acc + offset).Normalized()));
            bullet.AddComponent(new CTransform(startPosition));
            bullet.AddComponent(new CGraphics(MainForm.ResizeImage(Properties.Resources.bullet, 10, 10)));
            bullet.AddComponent(new CGameObject());
            bullet.AddComponent(new CCollidable(10, 10));
        }

        int Inaccuracy(int upperBound)
        {
            return rnd.Next(2) == 0 ? -rnd.Next(upperBound + 1) : rnd.Next(upperBound + 1);
        }
    }
}
