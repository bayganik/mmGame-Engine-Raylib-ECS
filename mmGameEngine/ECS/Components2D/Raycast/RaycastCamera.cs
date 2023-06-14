using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;
using System.Numerics;
using mmGameEngine;

namespace mmGameEngine
{
    public class GTime
    {
        public GTime()
        { }
    }
    public class RaycastCamera
    {
        public Vector2 Position;                    //x, y position on map
        public Vector2 Direction;                   //camera facing
        public Vector2 CameraPlane;                 //2d camera plane

        public float VectorPlaneLength = 0.66f;
        public RaycastCamera()
        {
            Position = new Vector2(1.5f, 1.5f);
            Direction = new Vector2(1, 0);
            Vector2Ext.Normalize(ref Direction);

            CameraPlane = new Vector2(0, -1) * VectorPlaneLength;
            Move(Position, Direction);
        }

        public Action<GTime> Movement = null;
        public void Update(GTime _time)
        {
            if (Movement != null)
            {
                Movement.Invoke(_time);

            }
        }
        public void MoveBackwards()
        {
            if (Movement == null)
            {
                var targetDirection = Direction * -1;
                var targetPosition = Position + targetDirection;

                Move(targetPosition, targetDirection);
            }
        }

        public void MoveForward()
        {
            if (Movement == null)
            {
                var targetPosition = (Position + Direction);
                Move(targetPosition, Direction);
            }
        }
        private void Move(Vector2 targetPosition, Vector2 targetDirection)
        {
            var vectorLength = (Position - targetPosition).Length();

            // Create the action
            Movement = (gameTime) =>
            {
                // Update the current position by a factor of the direction
                var updatePosition = (targetDirection / 350.0f) * 40.0f;
                Position += updatePosition;
                vectorLength -= updatePosition.Length();

                // Finished
                if (vectorLength <= 0)
                {
                    // Stop overshooting
                    Position = targetPosition;
                    Movement = null;
                }
            };
        }

        private void Rotate(Double target, Int32 direction)
        {

            var length = Math.PI / 2;

            Movement = (gameTime) =>
            {
                float rotation = 0.004f * direction * 40.0f;
                rotation = (float)Math.Min(length, rotation);
                length -= Math.Abs(rotation);

                // Convert rotation to direction
                Matrix2D rotMatrix = Matrix2D.CreateRotationZ(rotation);

                Vector2 newDirection = Vector2.Transform(Direction, Matrix2D.Convert(rotMatrix));
                Direction = newDirection;

                Vector2 newVectorPlane = Vector2.Transform(CameraPlane, Matrix2D.Convert(rotMatrix));
                CameraPlane = newVectorPlane;

                if (length <= 0)
                {
                    // set the direction vector and update 
                    Direction.X = (float)Math.Cos(target);
                    Direction.Y = (float)Math.Sin(target);

                    // Set the vector plane as the normal to the direction vector
                    CameraPlane.X = Direction.Y;
                    CameraPlane.Y = -1 * Direction.X;

                    Vector2Ext.Normalize(ref CameraPlane);
                    CameraPlane *= VectorPlaneLength;

                    Movement = null;
                }
            };
        }

        public void RotateLeft()
        {
            if (Movement == null)
            {
                // Current Rotation
                var theta = Math.Atan2(Direction.Y, Direction.X);

                // Target Rotation
                var target = theta + (Math.PI / 2);

                Rotate(target, 1);
            }
        }

        public void RotateRight()
        {
            if (Movement == null)
            {
                // Current Rotation
                var theta = Math.Atan2(Direction.Y, Direction.X);

                // Target Rotation
                var target = theta - (Math.PI / 2);

                Rotate(target, -1);
            }
        }
    }
}
