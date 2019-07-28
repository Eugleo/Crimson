using Crimson.Components;
using Crimson.Entities;
using System.Threading.Tasks;


namespace Crimson.Systems
{
    class MeleeSystem : GameSystem
    {
        readonly EntityGroup<CTransform, CHasMeleeWeapon, CMeleeAttackEvent> _attackers;
        readonly int MELEE_RANGE = 8;

        public MeleeSystem(World world)
        {
            _world = world;
            _attackers = _world.GetGroup<EntityGroup<CTransform, CHasMeleeWeapon, CMeleeAttackEvent>>();
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
        async void CoolDown(CHasMeleeWeapon weapon, EntityHandle entity)
        {
            weapon.CanAttack = false;
            entity.AddComponent(weapon);
            await Task.Delay(weapon.RateOfAttack);
            weapon.CanAttack = true;
            entity.AddComponent(weapon);
        }
    }
}
