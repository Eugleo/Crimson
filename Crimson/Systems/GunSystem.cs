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
        readonly EntityGroup<CGun, CTransform, CShootEvent> _shootingEntities;

        public GunSystem(World world)
        {
            _world = world;
            _shootingEntities = _world.GetGroup<EntityGroup<CGun, CTransform, CShootEvent>>();
        }

        public override void Update()
        {
            foreach (var (entity, gun, transform, shot) in _shootingEntities)
            {
                if (!gun.CanShoot) { return; }

                Vector startingLocation;
                Vector direction;
                if (entity.HasComponent<CGraphics>())
                {
                    var image = entity.GetComponent<CGraphics>().Image;
                    var location = transform.Location + new Vector(image.Width / 2, image.Height / 2);
                    direction = (shot.TargetLocation - location).Normalized(100);
                    var offset = (int)Math.Ceiling(new Vector(image.Width / 2, image.Height / 2).Size);
                    startingLocation = location + direction.Normalized(offset);
                }
                else
                {
                    direction = (shot.TargetLocation - transform.Location).Normalized(100);
                    startingLocation = transform.Location;
                }

                switch (gun.Type)
                {
                    case CGun.ShootingPattern.Pistol:
                    case CGun.ShootingPattern.SMG:
                        ShootBullet(startingLocation, direction, gun);
                        break;
                    case CGun.ShootingPattern.Shotgun:
                        var spread = direction.Orthogonalized().Normalized(8);
                        foreach (var i in Enumerable.Range(-2, 5))
                        {
                            ShootBullet(startingLocation, direction + spread.ScaledBy(i), gun);
                        }
                        break;
                    default:
                        break;
                }
                CoolDown(gun, entity);
            }
        }

        void ShootBullet(Vector startPosition, Vector direction, CGun gun)
        {
            var bullet = _world.CreateEntity();
            var inaccuracy = new Vector(Inaccuracy(gun.Inaccuracy), Inaccuracy(gun.Inaccuracy));
            bullet.AddComponent(new CBullet(gun.Damage, gun.Range));
            bullet.AddComponent(new CMovement(gun.BulletSpeed, (direction + inaccuracy).Normalized()));
            bullet.AddComponent(new CTransform(startPosition));
            bullet.AddComponent(new CGraphics(MainForm.ResizeImage(Properties.Resources.bullet, 10, 10)));
            bullet.AddComponent(new CGameObject());
            bullet.AddComponent(new CCollidable(10, 10));
        }

        readonly Random rnd = new Random();
        int Inaccuracy(int upperBound)
        {
            return rnd.Next(2) == 0 ? -rnd.Next(upperBound + 1) : rnd.Next(upperBound + 1);
        }

        async void CoolDown(CGun gun, EntityHandle entity)
        {
            gun.CanShoot = false;
            entity.AddComponent(gun);
            await Task.Delay(gun.Cadence);
            gun.CanShoot = true;
            entity.AddComponent(gun);
        }
    }
}
