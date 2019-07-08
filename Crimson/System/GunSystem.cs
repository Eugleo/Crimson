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
        EntityFilter<CHasGun, CShootEvent, CPosition> _filter;

        public GunSystem(World world)
        {
            _world = world;
            _filter = _world.GetFilter<EntityFilter<CHasGun, CShootEvent, CPosition>>();
        }

        public override void Update()
        {
            foreach (var i in Enumerable.Range(0, _filter.Entities.Count))
            {
                var gun = _filter.Components1[i];
                var targetPosition = _filter.Components2[i].Target;
                var position = _filter.Components3[i].Coords;

                switch (gun.Type)
                {
                    case CHasGun.GunType.Pistol:
                        ShootPistol(position, targetPosition);
                        break;
                    case CHasGun.GunType.SMG:
                        ShootSMG(position, targetPosition);
                        break;
                    case CHasGun.GunType.Shotgun:
                        ShootShotgun(position, targetPosition);
                        break;
                    default:
                        break;
                }
            }
            _world.ForEachEntityWithComponents<CShootEvent>(e => _world.RemoveComponentFromEntity<CShootEvent>(e));
        }

        void ShootPistol((double X, double Y) position, (double X, double Y) targetPosition)
        {
            Debug.WriteLine("Shoot!");

            var (X, Y) = position;
            var (targetX, targetY) = targetPosition;
            // TODO: Refaktorovat na vektor
            var size = Math.Sqrt(Math.Pow(targetPosition.X - position.X, 2) + Math.Pow(targetPosition.Y - position.Y, 2));

            var bullet = _world.CreateEntity();
            bullet.AddComponent(new CBullet(20));
            bullet.AddComponent(new CMovement((30, 30), ((targetPosition.X - position.X) / size, (targetPosition.Y - position.Y) / size)));
            bullet.AddComponent(new CPosition(position));
            bullet.AddComponent(new CGraphics(MainForm.ResizeImage(Properties.Resources.bullet, 20, 20)));
            bullet.AddComponent(new CGameObject());
        }

        void ShootSMG((double, double) position, (double, double) targetPosition)
        {
            var (X, Y) = position;
            var (targetX, targetY) = targetPosition;
        }

        void ShootShotgun((double, double) position, (double, double) targetPosition)
        {
            var (X, Y) = position;
            var (targetX, targetY) = targetPosition;
        }
    }
}
