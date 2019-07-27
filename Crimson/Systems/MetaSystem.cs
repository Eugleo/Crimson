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
        readonly EntityGroup<CTimedRemover> _toRemoveComponent;
        readonly EntityGroup<CTimedAdder> _toAddComponent;

        public MetaSystem(World world)
        {
            _world = world;
            _toDelete = _world.GetGroup<EntityGroup<CCLeanup>>();
            _toRemoveComponent = _world.GetGroup<EntityGroup<CTimedRemover>>();
            _toAddComponent = _world.GetGroup<EntityGroup<CTimedAdder>>();
        }

        public override void Update()
        {
            MethodInfo addComponent = typeof(EntityHandle).GetMethod("AddComponent");
            var toRemoveAdder = new List<EntityHandle>();
            foreach (var (entity, adder) in _toAddComponent)
            {
                if (adder.TimeLeft <= 0)
                {
                    var m = addComponent.MakeGenericMethod(adder.Component.GetType());
                    _ = m.Invoke(entity, new object[1] { adder.Component });
                    toRemoveAdder.Add(entity);
                }
                else
                {
                    entity.AddComponent(new CTimedAdder(adder.Component, adder.TimeLeft - 1));
                }
            }
            toRemoveAdder.ForEach(e => e.RemoveComponent<CTimedAdder>());

            MethodInfo removeComponent = typeof(EntityHandle).GetMethod("RemoveComponent");
            var toRemoveRemover = new List<EntityHandle>();
            foreach (var (entity, remove) in _toRemoveComponent)
            {
                foreach (var i in Enumerable.Range(0, remove.Components.Count))
                {
                    var (component, timeleft) = remove.Components[i];
                    if (timeleft <= 0)
                    {
                        var m = removeComponent.MakeGenericMethod(component);
                        _ = m.Invoke(entity, new object[0]);
                        toRemoveRemover.Add(entity);
                    }
                    else
                    {
                        remove.Components[i] = (component, timeleft - 1);
                    }
                }
            }
            toRemoveRemover.ForEach(e => e.RemoveComponent<CTimedRemover>());

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
    }
}
