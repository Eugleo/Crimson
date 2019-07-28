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
    [Flags]
    enum Components : long
    {
        None = 0,
        Movement = 1,
        Transform = 2,
        OnCollisionAdder = 4,
        Graphics = 16,
        KeyboardNavigation = 32,
        InputEvent = 64,
        Camera = 128,
        Bullet = 256,
        Collidable = 512,
        CollisionEvent = 1024,
        ShootEvent = 2048,
        ScheduledRemove = 4096,
        ScheduledAdd = 8192,
        PursuitBehavior = 16384,
        HasGun = 32768,
        Faction = 65536,
        Feeler = 131072,
        Attacker = 262144,
        Burning = 524288,
        Flammable = 1048576,
        HasMeleeWeapon = 2097152,
        MeleeAttackEvent = 4194304,
        AvoidObstaclesBehavior = 8388608,
        Wet = 16777216,
        Cleanup = 33554432,
        Submergeable = 67108864,
        Above = 134217728,
        Tile = 268435456,
        GameObject = 536870912,
        Health = 1073741824,
        DropGun = 2147483648,
        CAddOnStep = 4294967296
    }

    interface IComponent
    {
        Components Component { get; }
    }

    class CAddOnStep : IComponent
    {
        public Components Component => Components.CAddOnStep;
        public IComponent Comp;

        public CAddOnStep(IComponent comp)
        {
            Comp = comp;
        }

        public CAddOnStep() { }
    }

    class CDropGun : IComponent
    {
        public Components Component => Components.DropGun;
        public CGun Gun;

        public CDropGun(CGun gun)
        {
            Gun = gun;
        }

        public CDropGun() { }
    }

    class CHealth : IComponent
    {
        public int MaxHealth { get; }
        public double CurrentHealth { get; }
        public Components Component => Components.Health;

        public CHealth(int maxHealth, double currentHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = currentHealth;
        }

        public CHealth() { }
    }

    class CMovement : IComponent
    {
        public Vector Velocity { get; set; }
        public double MaxSpeed { get; }
        public Components Component => Components.Movement;

        public CMovement(double maxSpeed, Vector acceleration)
        {
            Velocity = acceleration;
            MaxSpeed = maxSpeed;
        }

        public CMovement() { }
    }

    class CTransform : IComponent
    {
        public Vector Location { get; set; }
        public Components Component => Components.Transform;

        public CTransform(double x, double y)
        {
            Location = new Vector(x, y);
        }

        public CTransform(Vector location)
        {
            Location = location;
        }

        public CTransform() { }
    }

    class COnCollisionAdder : IComponent
    {
        public List<IComponent> Components { get; set; }
        public Components Component => Crimson.Components.Components.OnCollisionAdder;

        public COnCollisionAdder(List<IComponent> components)
        {
            Components = components;
        }

        public COnCollisionAdder() { }
    }

    class CGraphics : IComponent
    {
        public Image Image { get; }
        public Image OriginalImage { get; set; }
        public Components Component => Components.Graphics;

        public CGraphics(Image image)
        {
            Image = image;
            OriginalImage = image;
        }

        public CGraphics() { }
    }

    class CKeyboardNavigation : IComponent
    {
        public Components Component => Components.KeyboardNavigation;

        public CKeyboardNavigation() { }
    }

    class CInputEvent : IComponent
    {
        public List<KeyEventArgs> KeyEventArgs { get; set; }
        public Components Component => Components.InputEvent;

        public CInputEvent(List<KeyEventArgs> keyEventArgs)
        {
            KeyEventArgs = keyEventArgs;
        }

        public CInputEvent() { }
    }

    class CCamera : IComponent
    {
        public int FollowDistance { get; }
        public Components Component => Components.Camera;

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

        public CCamera() { }
    }

    class CBullet : IComponent
    {
        public int Damage { get; }
        public double RangeLeft { get; set; }
        public Components Component => Components.Bullet;

        public CBullet(int damage, double rangeLeft)
        {
            Damage = damage;
            RangeLeft = rangeLeft;
        }

        public CBullet() { }
    }

    class CCollidable : IComponent
    {
        public int Size { get; }
        public Components Component => Components.Collidable;

        public CCollidable(int size)
        {
            Size = size;
        }

        public CCollidable() { }
    }

    class CCollisionEvent : IComponent
    {
        public EntityHandle Partner { get; }
        public Components Component => Components.CollisionEvent;

        public CCollisionEvent(EntityHandle partner)
        {
            Partner = partner;
        }

        public CCollisionEvent() { }
    }

    class CShootEvent : IComponent
    {
        public Vector TargetLocation { get; }
        public Components Component => Components.ShootEvent;

        public CShootEvent(Vector targetLocation)
        {
            TargetLocation = targetLocation;
        }

        public CShootEvent() { }
    }

    class CScheduledRemove : IComponent
    {
        public List<(Type Component, double TimeLeft)> Components { get; set; }
        public Components Component => Crimson.Components.Components.ScheduledRemove;

        public CScheduledRemove(List<(Type, double)> components)
        {
            Components = components;
        }

        public CScheduledRemove(Type component, double timeLeft)
        {
            Components = new List<(Type, double)>() { (component, timeLeft) };
        }

        public CScheduledRemove() { }
    }

    class CScheduledAdd : IComponent
    {
        public double TimeLeft { get; }
        public IComponent ToRemove { get; }
        public Components Component => Components.ScheduledAdd;

        public CScheduledAdd(IComponent component, double timeLeft)
        {
            ToRemove = component;
            TimeLeft = timeLeft;
        }

        public CScheduledAdd() { }
    }

    class CPursuitBehavior : IComponent
    {
        // TODO Target musí mít movement a position
        public EntityHandle Target;
        public int Prediction;
        public int ReactionSpeed;
        public int Distance;
        public Components Component => Components.PursuitBehavior;

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

        public CPursuitBehavior() { }
    }

    class CGun : IComponent
    {
        public bool IsBeingReloaded { get; set; }
        public bool CanShoot { get; set; }
        public Components Component => Components.HasGun;

        public enum ShootingPattern
        {
            Pistol, Shotgun, SMG, Grenade
        }
        public ShootingPattern Type { get; }
        public int Damage { get; }
        public int ReloadSpeed { get; }
        public int Inaccuracy { get; }
        public int Cadence { get; }
        public double BulletSpeed { get; }
        public double Range { get; }
        public int MagazineSize { get; }
        public int Ammo { get; set; }

        public CGun(ShootingPattern type, int damage, int reloadSpeed, int inaccuracy, int cadence, int bulletSpeed, double range, int magazineSize)
        {
            Type = type;
            Damage = damage;
            ReloadSpeed = reloadSpeed;
            Inaccuracy = inaccuracy;
            Cadence = cadence;
            BulletSpeed = bulletSpeed;
            CanShoot = true;
            Range = range;
            MagazineSize = magazineSize;
            Ammo = MagazineSize;
            IsBeingReloaded = false;
        }

        public CGun() { }
    }

    class CTile : IComponent
    {
        public Components Component => Components.Tile;

        public CTile() { }
    }

    class CGameObject : IComponent
    {
        public Components Component => Components.GameObject;

        public CGameObject() { }
    }

    enum Faction { PC, NPC }
    class CFaction : IComponent
    {
        public Faction Faction { get; }
        public Components Component => Components.Faction;

        public CFaction(Faction faction)
        {
            Faction = faction;
        }

        public CFaction() { }
    }

    class CFeeler : IComponent
    {
        public Vector Offset { get; }
        public Components Component => Components.Feeler;

        public CFeeler(Vector offset)
        {
            Offset = offset;
        }

        public CFeeler() { }
    }

    class CAttacker : IComponent
    {
        public EntityHandle Target;
        public Components Component => Components.Attacker;

        public CAttacker(EntityHandle target)
        {
            Target = target;
        }

        public CAttacker() { }
    }

    class CBurning : IComponent
    {
        public double Spread { get; }
        public Components Component => Components.Burning;

        public CBurning(double spread)
        {
            Spread = spread;
        }

        public CBurning() { }
    }
    class CFlammable: IComponent
    {
        public Image Image;
        public Components Component => Components.Flammable;

        public CFlammable(Image image)
        {
            Image = image;
        }

        public CFlammable() { }
    }

    class CHasMeleeWeapon : IComponent
    {
        public int Range;
        public int RateOfAttack;
        public int Damage;
        public bool CanAttack;
        public Components Component => Components.HasMeleeWeapon;

        public CHasMeleeWeapon(int range, int rateOfAttack, int damage, bool canAttack)
        {
            Range = range;
            RateOfAttack = rateOfAttack;
            Damage = damage;
            CanAttack = canAttack;
        }

        public CHasMeleeWeapon() { }
    }

    class CMeleeAttackEvent : IComponent
    {
        public EntityHandle Target { get; }
        public Components Component => Components.MeleeAttackEvent;

        public CMeleeAttackEvent(EntityHandle target)
        {
            Target = target;
        }

        public CMeleeAttackEvent() { }
    }

    class CAvoidObstaclesBehavior : IComponent
    {
        public int Prediction;
        public int ReactionSpeed;
        public List<(EntityHandle, int)> Feelers;
        public Components Component => Components.AvoidObstaclesBehavior;

        public CAvoidObstaclesBehavior(int prediction, int reactionSpeed, List<(EntityHandle, int)> feelers)
        {
            Prediction = prediction;
            ReactionSpeed = reactionSpeed;
            Feelers = feelers;
        }

        public CAvoidObstaclesBehavior() { }
    }

    class CWet : IComponent
    {
        public double Spread { get; }
        public Components Component => Components.Wet;

        public CWet(double spread)
        {
            Spread = spread;
        }

        public CWet() { }
    }

    class CCLeanup : IComponent
    {
        public Components Component => Components.Cleanup;

        public CCLeanup() { }
    }

    class CSumbergable : IComponent
    {
        public Image Image;
        public Components Component => Components.Submergeable;

        public CSumbergable(Image image)
        {
            Image = image;
        }

        public CSumbergable() { }
    }

    class CAbove : IComponent
    {
        public Components Component => Components.Above;

        public CAbove() { }
    }
}