using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Crimson
{
    struct CHealth
    {
        public int MaxHealth { get; }
        public int CurrentHealth { get; }

        public CHealth(int maxHealth, int currentHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = currentHealth;
        }
    }

    struct CMovement
    {
        public double Speed;
        public Vector Acceleration;

        public CMovement(double speed, Vector acceleration)
        {
            Speed = speed;
            Acceleration = acceleration;
        }
    }

    struct CTransform
    {
        public Vector Location;

        public CTransform(double x, double y)
        {
            Location = new Vector(x, y);
        }

        public CTransform(Vector location)
        {
            Location = location;
        }
    }

    struct CGraphics
    {
        public Image Image { get; }

        public CGraphics(Image image)
        {
            Image = image;
        }
    }

    struct CKeyboardNavigation { }

    struct CInputEvent
    {
        public List<KeyEventArgs> KeyEventArgs { get; }

        public CInputEvent(List<KeyEventArgs> keyEventArgs)
        {
            KeyEventArgs = keyEventArgs;
        }
    }

    struct CCamera
    {
        public int FollowDistance { get; }

        // Target entita musí mít CMovement a CPosition
        public Entity Target { get; }

        public (double, double) WorldBounds { get; }
        public (double, double) ScreenBounds { get; }

        public CCamera(int followDistance, Entity target, (double, double) worldBounds, (double, double) cameraBounds)
        {
            FollowDistance = followDistance;
            Target = target;
            WorldBounds = worldBounds;
            ScreenBounds = cameraBounds;
        }
    }

    struct CBullet
    {
        public int Damage { get; }

        public CBullet(int damage)
        {
            Damage = damage;
        }
    }

    struct CCollides
    {
        public Rectangle Bounds { get; }

        public CCollides(Rectangle bounds)
        {
            Bounds = bounds;
        }
    }

    struct CCollisionEvent { }

    struct CShootEvent
    {
        public Vector TargetLocation { get; }

        public CShootEvent(Vector targetLocation)
        {
            TargetLocation = targetLocation;
        }
    }

    struct CHasGun
    {
        public bool CanShoot;
        public enum GunType
        {
            Pistol, Shotgun, SMG
        }

        public GunType Type { get; }

        public CHasGun(GunType type)
        {
            Type = type;
            CanShoot = true;
        }
    }

    struct CTile { }

    struct CGameObject { }
}