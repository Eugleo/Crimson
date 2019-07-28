using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crimson.Entities;
using Crimson.Components;


namespace Crimson.Systems
{
    class AttackSystem : GameSystem
    {
        readonly EntityGroup<CTransform, CAttacker, CHasMeleeWeapon> _meleeAttackers;
        readonly EntityGroup<CTransform, CAttacker, CHasGun> _shooters;

        public AttackSystem(World world)
        {
            _world = world;
            _meleeAttackers = _world.GetGroup<EntityGroup<CTransform, CAttacker, CHasMeleeWeapon>>();
            _shooters = _world.GetGroup<EntityGroup<CTransform, CAttacker, CHasGun>>();
        }

        public override void Update()
        {
            foreach (var (entity, transform, attack, gun) in _shooters)
            {
                if (attack.Target.TryGetComponent(out CTransform targetTransform) && InRange(transform, targetTransform, gun.Range))
                {
                    var shootEvent = new CShootEvent(targetTransform.Location);
                    entity.AddComponent(shootEvent);
                }
                else
                {
                    entity.RemoveComponent<CShootEvent>();
                }
            }


            foreach (var (entity, transform, attack, weapon) in _meleeAttackers)
            {
                if (attack.Target.TryGetComponent(out CTransform targetTransform) && InRange(transform, targetTransform, weapon.Range))
                {
                    var attackEvent = new CMeleeAttackEvent(attack.Target);
                    entity.AddComponent(attackEvent);
                }
                else
                {
                    entity.RemoveComponent<CMeleeAttackEvent>();
                }
            }
        }

        bool InRange(CTransform a, CTransform b, double range)
        {
            return (a.Location - b.Location).Size < range;
        }
    }
}
