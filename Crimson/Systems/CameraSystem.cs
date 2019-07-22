using System;
using System.Diagnostics;
using Crimson.Entities;
using Crimson.Components;

namespace Crimson.Systems
{
    class CameraSystem : GameSystem
    {
        readonly EntityGroup<CCamera, CMovement, CTransform> _cameras;
        readonly Map _map;

        public CameraSystem(World world, Map map)
        {
            _world = world;
            _cameras = _world.GetGroup<EntityGroup<CCamera, CMovement, CTransform>>();
            _map = map;
        }

        public override void Update()
        {
            foreach (var (entity, camera, movement, transform) in _cameras)
            {
                if (camera.Target.TryGetComponent(out CTransform targetTransform))
                {
                    var (worldWidth, worldHeight) = (_map.Width * _map.TileSize, _map.Height * _map.TileSize);
                    var (screenWidth, screenHeight) = (camera.ScreenBounds.Item1, camera.ScreenBounds.Item2);
                    var targetLocation = targetTransform.Location;
                    var (X, Y) = (transform.Location.X, transform.Location.Y);

                    double xOffset = 0;
                    double yOffset = 0;

                    if (!camera.Target.TryGetComponent(out CMovement targetMovement)) { targetMovement = movement;  }

                    var distanceX = X - targetLocation.X;
                    if (Math.Abs(distanceX) > camera.FollowDistance)
                    {
                        var newAcc = (distanceX > 0 ? -1 : 1) * Math.Abs(targetMovement.Velocity.X);
                        var newX = X + newAcc;
                        if (screenWidth / 2 < newX && newX < worldWidth - screenWidth / 2)
                        {
                            xOffset = newAcc;
                        }
                    }
                    var distanceY = Y - targetLocation.Y;
                    if (Math.Abs(distanceY) > camera.FollowDistance)
                    {
                        var newAcc = (distanceY > 0 ? -1 : 1) * Math.Abs(targetMovement.Velocity.Y);
                        var newY = Y + newAcc;
                        if (screenHeight / 2 < newY && newY < worldHeight - screenHeight / 2)
                        {
                            yOffset = newAcc;
                        }
                    }

                    if (xOffset == 0 && yOffset == 0)
                    {
                        entity.AddComponent(new CCamera(camera.FollowDistance, camera.Target, camera.ScreenBounds) { NeedsRefresh = false });
                    }
                    entity.AddComponent(new CMovement(targetMovement.MaxSpeed, new Vector(xOffset, yOffset)));
                }
            }
        }
    }
}
