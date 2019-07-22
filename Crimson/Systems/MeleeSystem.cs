using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crimson.Entities;
using Crimson.Components;


namespace Crimson.Systems
{
    class MeleeSystem : GameSystem
    {
        readonly EntityGroup<CTransform, CMeleeWeapon, CMeleeAttackEvent> _attackers;
        readonly int MELEE_RANGE = 8;

        public MeleeSystem(World world)
        {
            _world = world;
            _attackers = _world.GetGroup<EntityGroup<CTransform, CMeleeWeapon, CMeleeAttackEvent>>();
        }

        public override void Update()
        {
            foreach (var (entity, transform, weapon, attack) in _attackers)
            {
                if (!weapon.CanAttack) { return; }

                if (attack.Target.TryGetComponent(out CHealth health))
                {
                    var newHealth = new CHealth(health.MaxHealth, health.CurrentHealth - weapon.Damage);
                    attack.Target.AddComponent(newHealth);
                }
                CoolDown(weapon, entity);
            }
        }
        async void CoolDown(CMeleeWeapon weapon, EntityHandle entity)
        {
            weapon.CanAttack = false;
            entity.AddComponent(weapon);
            await Task.Delay(weapon.RateOfAttack);
            weapon.CanAttack = true;
            entity.AddComponent(weapon);
        }
    }
}
