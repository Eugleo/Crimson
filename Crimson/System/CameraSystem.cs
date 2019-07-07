using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Crimson
{
    class CameraSystem : GameSystem
    {
        readonly EntityFilter<CCamera, CPosition> _cameras;

        public CameraSystem(World world)
        {
            _world = world;
            _cameras = _world.GetFilter<EntityFilter<CCamera, CPosition>>();
        }

        public override void Update()
        {
            var camera = _cameras.Components1[0];
            var (left, top, right, bottom) = (0, 0, camera.WorldBounds.Item1, camera.WorldBounds.Item2);
            var (width, height) = (camera.ScreenBounds.Item1, camera.ScreenBounds.Item2);
            var (targetX, targetY) = _world.GetComponentForEntity<CPosition>(camera.Target).Coords;
            var targetMovement = _world.GetComponentForEntity<CMovement>(camera.Target);
            var (X, Y) = _cameras.Components2[0].Coords;

            double xOffset = 0;
            double yOffset = 0;

            if (Math.Abs(X - targetX) > camera.FollowDistance)
            {
                var newX = X + targetMovement.Speed.X * (X - targetX > 0 ? -1 : 1) * Math.Abs(targetMovement.Acceleration.X);
                if (newX > width / 2 && newX < right - width / 2)
                {
                    xOffset = (X - targetX > 0 ? -1 : 1) * Math.Abs(targetMovement.Acceleration.X);
                }
            }

            if (Math.Abs(Y - targetY) > camera.FollowDistance)
            {
                var newY = Y + targetMovement.Speed.Y * (Y - targetY > 0 ? -1 : 1) * Math.Abs(targetMovement.Acceleration.Y);
                if (newY > height / 2 && newY < bottom - height / 2)
                {
                    yOffset = (Y - targetY > 0 ? -1 : 1) * Math.Abs(targetMovement.Acceleration.Y);
                }
            }

            var move = new CMovement(targetMovement.Speed, (xOffset, yOffset));
            _world.AddComponentToEntity(_cameras.Entities[0], move);
        }
    }
}
