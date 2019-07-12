using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using Crimson.Entities;

namespace Crimson.Components
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
        public Image OriginalImage { get; set; }

        public CGraphics(Image image)
        {
            Image = image;
            OriginalImage = image;
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

    struct CCollidable
    {
        public Vector Size { get; }

        public CCollidable(Vector size)
        {
            Size = size;
        }

        public CCollidable(double width, double height)
        {
            Size = new Vector(width, height);
        }
    }

    struct CCollisionEvent
    {
        public Entity Partner { get; }

        public CCollisionEvent(Entity partner)
        {
            Partner = partner;
        }
    }

    struct CShootEvent
    {
        public Vector TargetLocation { get; }

        public CShootEvent(Vector targetLocation)
        {
            TargetLocation = targetLocation;
        }
    }

    struct CGun
    {
        public bool CanShoot;

        // TODO Možná by nebylo od věci změnit GunType na ShootingPattern nebo tak něco
        public enum GunType
        {
            Pistol, Shotgun, SMG
        }
        public GunType Type { get; }
        public int Damage { get; }
        public int ReloadSpeed { get; }
        public int Inaccuracy { get; }
        public int Cadence { get;  }
        public double BulletSpeed { get;  }

        public CGun(GunType type, int damage, int reloadSpeed, int inaccuracy, int cadence, int bulletSpeed) : this()
        {
            Type = type;
            Damage = damage;
            ReloadSpeed = reloadSpeed;
            Inaccuracy = inaccuracy;
            Cadence = cadence;
            BulletSpeed = bulletSpeed;
            CanShoot = true;
        }
    }

    struct CTile { }

    struct CGameObject { }
}