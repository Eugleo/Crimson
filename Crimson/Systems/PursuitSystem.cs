﻿using Crimson.Components;
using Crimson.Entities;

namespace Crimson.Systems
{
    class PursuitSystem : GameSystem
    {
        readonly EntityGroup<CTransform, CMovement, CPursuitBehavior> _pursuing;

        public PursuitSystem(World world)
        {
            _world = world;
            _pursuing = _world.GetGroup<EntityGroup<CTransform, CMovement, CPursuitBehavior>>();
        }

        public override void Update()
        {
            foreach (var (entity, transform, movement, pursuit) in _pursuing)
            {
                if (pursuit.Target.TryGetComponent(out CTransform targetTransform) &&
                    pursuit.Target.TryGetComponent(out CMovement targetMovement))
                {
                    var projectedLocation = targetTransform.Location + targetMovement.Velocity.ScaledBy(pursuit.Prediction);
                    var direction = projectedLocation - transform.Location;
                    var adjustedDirection = direction + direction.ScaledBy(-1).Normalized(pursuit.Distance);

                    if (adjustedDirection.Size > 10)
                    {
                        var steering = (adjustedDirection - movement.Velocity).Normalized(pursuit.ReactionSpeed);
                        movement.Velocity = (movement.Velocity + steering).Normalized(movement.MaxSpeed);
                    }
                    else
                    {
                        var steering = (adjustedDirection - movement.Velocity).Normalized(pursuit.ReactionSpeed);
                        movement.Velocity = (movement.Velocity + steering).Normalized(movement.Velocity.Size / 1.5);
                    }
                }
            }
        }
    }
}
