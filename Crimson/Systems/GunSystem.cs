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
        readonly EntityGroup<CGun, CTransform, CShootEvent> _filter;

        public GunSystem(World world)
        {
            _world = world;
            _filter = _world.GetGroup<EntityGroup<CGun, CTransform, CShootEvent>>();
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

                if (entity.HasComponent<CGraphics>())
                {
                    var image = entity.GetComponent<CGraphics>().Image;
                    position += new Vector(image.Width / 2, image.Height / 2);
                    offset = (int)Math.Ceiling(new Vector(image.Width / 2, image.Height / 2).Size);
                    startPosition = position + (targetPosition - position).Normalized(offset);
                }

                switch (gun.Type)
                {
                    case CGun.ShootingPattern.Pistol:
                        ShootPistol(position, targetPosition, gun, entity, startPosition);
                        break;
                    case CGun.ShootingPattern.SMG:
                        ShootSMG(position, targetPosition, gun, entity, startPosition);
                        break;
                    case CGun.ShootingPattern.Shotgun:
                        ShootShotgun(position, targetPosition, gun, entity, startPosition);
                        break;
                    default:
                        break;
                }
            }
        }

        readonly Random rnd = new Random();
        async void ShootPistol(Vector position, Vector targetPosition, CGun gun, EntityHandle e, Vector startPosition)
        {
            if (gun.CanShoot)
            {
                MakeBullet(position, targetPosition, gun.Damage, gun.BulletSpeed, new Vector(Inaccuracy(gun.Inaccuracy), Inaccuracy(gun.Inaccuracy)), startPosition, gun.Range);
                gun.CanShoot = false;
                e.AddComponent(gun);
                await Task.Delay(gun.Cadence);
                gun.CanShoot = true;
                e.AddComponent(gun);
            }
        }

        async void ShootSMG(Vector position, Vector targetPosition, CGun gun, EntityHandle e, Vector startPosition)
        {
            if (gun.CanShoot)
            {
                MakeBullet(position, targetPosition, gun.Damage, gun.BulletSpeed, new Vector(Inaccuracy(gun.Inaccuracy), Inaccuracy(gun.Inaccuracy)), startPosition, gun.Range);
                gun.CanShoot = false;
                e.AddComponent(gun);
                await Task.Delay(gun.Cadence);
                gun.CanShoot = true;
                e.AddComponent(gun);
            }
        }

        async void ShootShotgun(Vector position, Vector targetPosition, CGun gun, EntityHandle e, Vector startPosition)
        {
            if (gun.CanShoot)
            {
                var acc = (targetPosition - position).Orthogonalized().Normalized(8);

                foreach (var i in Enumerable.Range(-2, 5))
                {
                    MakeBullet(position, targetPosition, gun.Damage, gun.BulletSpeed, acc.ScaledBy(i), startPosition, gun.Range);
                }

                gun.CanShoot = false;
                e.AddComponent(gun);
                await Task.Delay(gun.Cadence);
                gun.CanShoot = true;
                e.AddComponent(gun);
            }
        }

        void MakeBullet(Vector position, Vector targetPosition, int damage, double speed, Vector offset, Vector startPosition, double range)
        {
            var acc = (targetPosition - position).Normalized(100);
            var bullet = _world.CreateEntity();
            bullet.AddComponent(new CBullet(damage, range));
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
