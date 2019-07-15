using System;
using System.Diagnostics;
using Crimson.Entities;
using Crimson.Components;

namespace Crimson.Systems
{
    class CameraSystem : GameSystem
    {
        readonly EntityGroup<CCamera, CTransform> _cameras;

        public CameraSystem(World world)
        {
            _world = world;
            _cameras = _world.GetGroup<EntityGroup<CCamera, CTransform>>();
        }

        public override void Update()
        {
            if (_cameras.Count > 0)
            {
                var camera = _cameras.Components1[0];
                var (right, bottom) = (camera.WorldBounds.Item1, camera.WorldBounds.Item2);
                var (width, height) = (camera.ScreenBounds.Item1, camera.ScreenBounds.Item2);
                var targetLocation = camera.Target.GetComponent<CTransform>().Location;
                var targetMovement = camera.Target.GetComponent<CMovement>();
                var (X, Y) = (_cameras.Components2[0].Location.X, (_cameras.Components2[0].Location.Y));

                double xOffset = 0;
                double yOffset = 0;

                if (Math.Abs(X - targetLocation.X) > camera.FollowDistance)
                {
                    var newX = X + targetMovement.Speed * (X - targetLocation.X > 0 ? -1 : 1) * Math.Abs(targetMovement.Acceleration.X);
                    if (newX > width / 2 && newX < right - width / 2)
                    {
                        xOffset = (X - targetLocation.X > 0 ? -1 : 1) * Math.Abs(targetMovement.Acceleration.X);
                    }
                }

                if (Math.Abs(Y - targetLocation.Y) > camera.FollowDistance)
                {
                    var newY = Y + targetMovement.Speed * (Y - targetLocation.Y > 0 ? -1 : 1) * Math.Abs(targetMovement.Acceleration.Y);
                    if (newY > height / 2 && newY < bottom - height / 2)
                    {
                        yOffset = (Y - targetLocation.Y > 0 ? -1 : 1) * Math.Abs(targetMovement.Acceleration.Y);
                    }
                }

                var move = new CMovement(targetMovement.Speed, new Vector(xOffset, yOffset));
                _cameras.Entities[0].AddComponent(move);
            }
        }
    }
}
