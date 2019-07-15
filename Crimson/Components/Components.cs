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
    interface Component { }

    struct CHealth : Component
    {
        public int MaxHealth { get; }
        public int CurrentHealth { get; }

        public CHealth(int maxHealth, int currentHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = currentHealth;
        }
    }

    struct CMovement : Component
    {
        public double Speed;
        public Vector Acceleration;

        public CMovement(double speed, Vector acceleration)
        {
            Speed = speed;
            Acceleration = acceleration;
        }
    }

    struct CTransform : Component
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

    struct CGraphics : Component
    {
        public Image Image { get; }
        public Image OriginalImage { get; set; }

        public CGraphics(Image image)
        {
            Image = image;
            OriginalImage = image;
        }
    }

    struct CKeyboardNavigation : Component { }

    struct CInputEvent : Component
    {
        public List<KeyEventArgs> KeyEventArgs { get; }

        public CInputEvent(List<KeyEventArgs> keyEventArgs)
        {
            KeyEventArgs = keyEventArgs;
        }
    }

    struct CCamera : Component
    {
        public int FollowDistance { get; }

        // Target entita musí mít CMovement a CPosition
        public EntityHandle Target { get; }

        public (double, double) WorldBounds { get; }
        public (double, double) ScreenBounds { get; }

        public CCamera(int followDistance, EntityHandle target, (double, double) worldBounds, (double, double) cameraBounds)
        {
            FollowDistance = followDistance;
            Target = target;
            WorldBounds = worldBounds;
            ScreenBounds = cameraBounds;
        }
    }

    struct CBullet : Component
    {
        public int Damage { get; }
        public double RangeLeft { get; }

        public CBullet(int damage, double rangeLeft)
        {
            Damage = damage;
            RangeLeft = rangeLeft;
        }
    }

    struct CCollidable : Component
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

    struct CCollisionEvent : Component
    {
        public EntityHandle Partner { get; }

        public CCollisionEvent(EntityHandle partner)
        {
            Partner = partner;
        }
    }

    struct CShootEvent : Component
    {
        public Vector TargetLocation { get; }

        public CShootEvent(Vector targetLocation)
        {
            TargetLocation = targetLocation;
        }
    }

    struct CGun : Component
    {
        public bool CanShoot;

        // TODO Možná by nebylo od věci změnit GunType na ShootingPattern nebo tak něco
        public enum ShootingPattern
        {
            Pistol, Shotgun, SMG
        }
        public ShootingPattern Type { get; }
        public int Damage { get; }
        public int ReloadSpeed { get; }
        public int Inaccuracy { get; }
        public int Cadence { get;  }
        public double BulletSpeed { get;  }
        public Double Range { get; }

        public CGun(ShootingPattern type, int damage, int reloadSpeed, int inaccuracy, int cadence, int bulletSpeed, double range) : this()
        {
            Type = type;
            Damage = damage;
            ReloadSpeed = reloadSpeed;
            Inaccuracy = inaccuracy;
            Cadence = cadence;
            BulletSpeed = bulletSpeed;
            CanShoot = true;
            Range = range;
        }
    }

    struct CTile : Component { }

    struct CGameObject : Component { }

    struct CMap : Component
    {
        public int Width { get; }
        public int Height { get; }
        public int TileSize { get; }

        public CMap(int width, int height, int tileSize)
        {
            Width = width;
            Height = height;
            TileSize = tileSize;
        }
    }

    enum Faction { PC, NPC }
    struct CFaction : Component
    {
        public Faction Faction { get; }

        public CFaction(Faction faction)
        {
            Faction = faction;
        }
    }
}