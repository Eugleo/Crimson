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
            if (_cameras.Count > 0)
            {
                var camera = _cameras.Components1[0];

                var (worldWidth, worldHeight) = (camera.WorldBounds.Item1, camera.WorldBounds.Item2);
                var (screenWidth, screenHeight) = (camera.ScreenBounds.Item1, camera.ScreenBounds.Item2);
                var targetLocation = camera.Target.GetComponent<CTransform>().Location;
                var targetMovement = camera.Target.GetComponent<CMovement>();
                var (X, Y) = (_cameras.Components2[0].Location.X, (_cameras.Components2[0].Location.Y));

                double xOffset = 0;
                double yOffset = 0;

                var distanceX = X - targetLocation.X;
                if (Math.Abs(distanceX) > camera.FollowDistance)
                {
                    var newAcc = (distanceX > 0 ? -1 : 1) * Math.Abs(targetMovement.Acceleration.X);
                    var newX = X + targetMovement.Speed * newAcc;
                    if (newX > screenWidth / 2 && newX < worldWidth - screenWidth / 2)
                    {
                        xOffset = newAcc;
                    }
                }
                var distanceY = Y - targetLocation.Y;
                if (Math.Abs(distanceY) > camera.FollowDistance)
                {
                    var newAcc = (distanceY > 0 ? -1 : 1) * Math.Abs(targetMovement.Acceleration.Y);
                    var newY = Y + targetMovement.Speed * newAcc;
                    if (newY > screenHeight / 2 && newY < worldHeight - screenHeight / 2)
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
