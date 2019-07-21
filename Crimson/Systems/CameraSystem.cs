using System;
using System.Diagnostics;
using Crimson.Entities;
using Crimson.Components;

namespace Crimson.Systems
{
    class CameraSystem : GameSystem
    {
        readonly EntityGroup<CCamera, CMovement, CTransform> _cameras;
        readonly EntityGroup<CMap> _maps;

        public CameraSystem(World world)
        {
            _world = world;
            _cameras = _world.GetGroup<EntityGroup<CCamera, CMovement, CTransform>>();
            _maps = _world.GetGroup<EntityGroup<CMap>>();
        }

        public override void Update()
        {
            foreach (var (entity, camera, movement, transform) in _cameras)
            {
                var map = _maps.Components1[0];

                if (camera.Target.TryGetComponent(out CTransform targetTransform))
                {
                    var (worldWidth, worldHeight) = (map.Width * map.TileSize, map.Height * map.TileSize);
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
                    _cameras.Entities[0].AddComponent(new CMovement(targetMovement.MaxSpeed, new Vector(xOffset, yOffset)));
                }
            }
        }
    }
}
