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
        public double CurrentHealth { get; }

        public CHealth(int maxHealth, double currentHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = currentHealth;
        }
    }

    class CMovement : Component
    {
        public Vector Velocity;
        public double MaxSpeed;

        public CMovement(double maxSpeed, Vector acceleration)
        {
            Velocity = acceleration;
            MaxSpeed = maxSpeed;
        }
    }

    class CTransform : Component
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

    struct COnCollisionAdder : Component
    {
        public List<Component> Components;

        public COnCollisionAdder(List<Component> components)
        {
            Components = components;
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
        public (double, double) ScreenBounds { get; }
        public bool NeedsRefresh;

        public CCamera(int followDistance, EntityHandle target, (double, double) cameraBounds)
        {
            FollowDistance = followDistance;
            Target = target;
            ScreenBounds = cameraBounds;
            NeedsRefresh = true;
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
        public int Size { get; }

        public CCollidable(int size)
        {
            Size = size;
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

    class CScheduledRemove : Component
    {
        public List<(Type Component, double TimeLeft)> Components { get; set; }

        public CScheduledRemove(List<(Type, double)> components)
        {
            Components = components;
        }

        public CScheduledRemove(Type component, double timeLeft)
        {
            Components = new List<(Type, double)>() { (component, timeLeft) };
        }
    }

    struct CScheduledAdd : Component
    {
        public double TimeLeft { get; }
        public Component Component { get; }

        public CScheduledAdd(Component component, double timeLeft)
        {
            Component = component;
            TimeLeft = timeLeft;
        }
    }

    struct CPursuitBehavior : Component
    {
        // TODO Target musí mít movement a position
        public EntityHandle Target;
        public int Prediction;
        public int ReactionSpeed;
        public int Distance;

        public CPursuitBehavior(EntityHandle target, int prediction, int reactionSpeed)
        {
            Target = target;
            Prediction = prediction;
            ReactionSpeed = reactionSpeed;
            Distance = 0;
        }

        public CPursuitBehavior(EntityHandle target, int prediction, int reactionSpeed, int distance)
        {
            Target = target;
            Prediction = prediction;
            ReactionSpeed = reactionSpeed;
            Distance = distance;
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

    enum Faction { PC, NPC }
    struct CFaction : Component
    {
        public Faction Faction { get; }

        public CFaction(Faction faction)
        {
            Faction = faction;
        }
    }

    struct CFeeler : Component
    {
        public Vector Offset { get; }

        public CFeeler(Vector offset)
        {
            Offset = offset;
        }
    }

    struct CAttacker : Component
    {
        public EntityHandle Target;

        public CAttacker(EntityHandle target)
        {
            Target = target;
        }
    }

    struct CBurning : Component
    {
        public double Spread { get; }

        public CBurning(double spread)
        {
            Spread = spread;
        }
    }
    struct CFlammable: Component
    {
        public Image Image;

        public CFlammable(Image image)
        {
            Image = image;
        }
    }

    struct CMeleeWeapon : Component
    {
        public int Range;
        public int RateOfAttack;
        public int Damage;
        public bool CanAttack;

        public CMeleeWeapon(int range, int rateOfAttack, int damage, bool canAttack)
        {
            Range = range;
            RateOfAttack = rateOfAttack;
            Damage = damage;
            CanAttack = canAttack;
        }
    }

    struct CMeleeAttackEvent : Component
    {
        public EntityHandle Target { get; }

        public CMeleeAttackEvent(EntityHandle target)
        {
            Target = target;
        }
    }

    struct CAvoidObstaclesBehavior : Component
    {
        public int Prediction;
        public int ReactionSpeed;
        public List<(EntityHandle, int)> Feelers;

        public CAvoidObstaclesBehavior(int prediction, int reactionSpeed, List<(EntityHandle, int)> feelers)
        {
            Prediction = prediction;
            ReactionSpeed = reactionSpeed;
            Feelers = feelers;
        }
    }

    struct CWet : Component
    {
        public double Spread { get; }

        public CWet(double spread)
        {
            Spread = spread;
        }
    }

    struct CCLeanup : Component { }

    struct CSumbergable : Component
    {
        public Image Image;

        public CSumbergable(Image image)
        {
            Image = image;
        }
    }

    struct CAbove : Component { }
}