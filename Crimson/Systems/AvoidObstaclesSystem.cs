﻿using Crimson.Components;
using Crimson.Entities;

namespace Crimson.Systems
{
    class AvoidObstaclesSystem : GameSystem
    {
        readonly EntityGroup<CTransform, CMovement, CAvoidObstaclesBehavior> _filter;

        public AvoidObstaclesSystem(World world)
        {
            _world = world;
            _filter = _world.GetGroup<EntityGroup<CTransform, CMovement, CAvoidObstaclesBehavior>>();
        }

        public override void Update()
        {
            foreach (var (entity, transform, movement, avoiding) in _filter)
            {
                foreach (var (feeler, distance) in avoiding.Feelers)
                {
                    var feelerCenter = feeler.GetComponent<CTransform>().Location + new Vector(20, 20);
                    if (feeler.TryGetComponent(out CCollisionEvent collision) &&
                        collision.Partner.TryGetComponent(out CTransform transform2) &&
                        collision.Partner.TryGetComponent(out CCollidable bounds) &&
                        collision.Partner.Entity != entity.Entity &&
                        (!entity.TryGetComponent(out CPursuitBehavior pursuit) || pursuit.Target.Entity != collision.Partner.Entity))
                    {
                        var center = transform2.Location + new Vector(bounds.Size, bounds.Size);
                        var steering = (feelerCenter - center).Normalized(avoiding.ReactionSpeed);
                        movement.Velocity = (movement.Velocity + steering).Normalized(movement.MaxSpeed);
                        break;
                    }
                }

                foreach (var (feeler, distance) in avoiding.Feelers)
                {
                    if (feeler.TryGetComponent(out CTransform feelerTansform))
                    {
                        var feelerLocation = transform.Location + feeler.GetComponent<CFeeler>().Offset + movement.Velocity.Normalized(distance) - new Vector(20, 20);
                        feelerTansform.Location = feelerLocation;
                    }
                }
            }
        }
    }
}
