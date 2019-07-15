using System;
using System.Diagnostics;
using Crimson.Entities;
using Crimson.Components;

namespace Crimson.Systems
{
    class CameraSystem : GameSystem
    {
        readonly EntityGroup<CCamera, CTransform> _cameras;
        readonly EntityGroup<CMap> _maps;

        public CameraSystem(World world)
        {
            _world = world;
            _cameras = _world.GetGroup<EntityGroup<CCamera, CTransform>>();
            _maps = _world.GetGroup<EntityGroup<CMap>>();
        }

        public override void Update()
        {
            foreach (var (entity, camera, transform) in _cameras)
            {
                var map = _maps.Components1[0];

                if (camera.Target.TryGetComponent(out CMovement targetMovement) &&
                    camera.Target.TryGetComponent(out CTransform targetTransform))
                {
                    var (worldWidth, worldHeight) = (map.Width * map.TileSize, map.Height * map.TileSize);
                    var (screenWidth, screenHeight) = (camera.ScreenBounds.Item1, camera.ScreenBounds.Item2);
                    var targetLocation = targetTransform.Location;
                    var (X, Y) = (_cameras.Components2[0].Location.X, (_cameras.Components2[0].Location.Y));

                    double xOffset = 0;
                    double yOffset = 0;

                    var distanceX = X - targetLocation.X;
                    if (Math.Abs(distanceX) > camera.FollowDistance)
                    {
                        var newAcc = (distanceX > 0 ? -1 : 1) * Math.Abs(targetMovement.Acceleration.X);
                        var newX = X + targetMovement.Speed * newAcc;
                        if (screenWidth / 2 < newX && newX < worldWidth - screenWidth / 2)
                        {
                            xOffset = newAcc;
                        }
                    }
                    var distanceY = Y - targetLocation.Y;
                    if (Math.Abs(distanceY) > camera.FollowDistance)
                    {
                        var newAcc = (distanceY > 0 ? -1 : 1) * Math.Abs(targetMovement.Acceleration.Y);
                        var newY = Y + targetMovement.Speed * newAcc;
                        if (screenHeight / 2 < newY && newY < worldHeight - screenHeight / 2)
                        {
                            yOffset = newAcc;
                        }
                    }

                    var move = new CMovement(targetMovement.Speed, new Vector(xOffset, yOffset));
                    _cameras.Entities[0].AddComponent(move);
                }
            }
        }
    }
}
