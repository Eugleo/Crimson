using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Crimson.Entities;
using Crimson.Components;

namespace Crimson.Systems
{
    class MetaSystem : GameSystem
    {
        readonly EntityGroup<CCLeanup> _toDelete;
        readonly EntityGroup<CScheduledRemove> _toRemoveComponent;
        readonly EntityGroup<CScheduledAdd> _toAddComponent;

        public MetaSystem(World world)
        {
            _world = world;
            _toDelete = _world.GetGroup<EntityGroup<CCLeanup>>();
            _toRemoveComponent = _world.GetGroup<EntityGroup<CScheduledRemove>>();
            _toAddComponent = _world.GetGroup<EntityGroup<CScheduledAdd>>();
        }

        readonly Dictionary<Type, MethodInfo> _adders = new Dictionary<Type, MethodInfo>();
        void Adder()
        {
            var toRemove = new List<EntityHandle>();
            foreach (var (entity, adder) in _toAddComponent)
            {
                if (adder.TimeLeft <= 0)
                {
                    if (!_adders.TryGetValue(adder.ToRemove.GetType(), out MethodInfo m))
                    {
                        MethodInfo addComponent = typeof(EntityHandle).GetMethod("AddComponent");
                        m = addComponent.MakeGenericMethod(adder.ToRemove.GetType());
                        _adders[adder.ToRemove.GetType()] = m;
                    }
                    _ = m.Invoke(entity, new object[1] { adder.ToRemove });
                    toRemove.Add(entity);
                }
                else
                {
                    entity.AddComponent(new CScheduledAdd(adder.ToRemove, adder.TimeLeft - 1));
                }
            }
            toRemove.ForEach(e => e.RemoveComponent<CScheduledAdd>());
        }

        readonly Dictionary<Type, MethodInfo> _removers = new Dictionary<Type, MethodInfo>();
        void Remover()
        {
            var toRemove = new List<EntityHandle>();
            foreach (var (entity, remove) in _toRemoveComponent)
            {
                foreach (var i in Enumerable.Range(0, remove.Components.Count))
                {
                    var (component, timeleft) = remove.Components[i];
                    if (timeleft <= 0)
                    {
                        if (!_removers.TryGetValue(component, out MethodInfo m))
                        {
                            MethodInfo removeComponent = typeof(EntityHandle).GetMethod("RemoveComponent");
                            m = removeComponent.MakeGenericMethod(component);
                            _removers[component] = m;
                        }
                        _ = m.Invoke(entity, new object[0]);
                        toRemove.Add(entity);
                    }
                    else
                    {
                        remove.Components[i] = (component, timeleft - 1);
                    }
                }
            }
            toRemove.ForEach(e => e.RemoveComponent<CScheduledRemove>());
        }

        void Deleter()
        {
            var toDelete = new List<EntityHandle>();
            foreach (var (entity, _) in _toDelete)
            {
                toDelete.Add(entity);
                if (entity.TryGetComponent(out CAvoidObstaclesBehavior avoid))
                {
                    toDelete.Concat(avoid.Feelers.Select(f => f.Item1));
                }
            }
            toDelete.ForEach(e => e.Delete());
        }

        public override void Update()
        {
            Adder();
            Remover();
            Deleter();
        }
    }
}
