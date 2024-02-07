using System;
using System.Collections.Generic;
using System.Numerics;
using Entitas;
using mmGameEngine;
using Raylib_cs;

namespace mmGameEngine
{
    public class InputController : Component
    {
        public KeyboardKey Right = KeyboardKey.Right;
        public KeyboardKey Left = KeyboardKey.Left;
        public KeyboardKey Up = KeyboardKey.Up;
        public KeyboardKey Down = KeyboardKey.Down;
        public bool IsKeyDownBehavior = true;                   //uses is key Down for continues action
        //
        // Result of the input is direction to move
        //
        public Vector2 Direction = Vector2.Zero;
        /// <summary>
        /// Controller using arrow KeyboardKey for input or "true" for WASD
        /// </summary>
        public InputController()
        {
            IsKeyDownBehavior = true;
        }
        public InputController(bool _wasd_KeyboardKey)
        {
            Right = KeyboardKey.D;
            Left = KeyboardKey.A;
            Up = KeyboardKey.W;
            Down = KeyboardKey.S;

            IsKeyDownBehavior = true;
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            //
            // Has Entity been assigned yet?
            //
            if (OwnerEntity == null)
                return;
            if (!Enabled)
                return;

            Direction = Vector2.Zero;
            if (IsKeyDownBehavior)
            {
                if (Raylib.IsKeyDown(Right))
                    Direction = new Vector2(1, 0);
                else if (Raylib.IsKeyDown(Left))
                    Direction = new Vector2(-1, 0);
                else if (Raylib.IsKeyDown(Up))
                    Direction = new Vector2(0, -1);
                else if (Raylib.IsKeyDown(Down))
                    Direction = new Vector2(0, 1);
            }
            else
            {
                if (Raylib.IsKeyPressed(Right))
                    Direction = new Vector2(1, 0);
                else if (Raylib.IsKeyPressed(Left))
                    Direction = new Vector2(-1, 0);
                else if (Raylib.IsKeyPressed(Up))
                    Direction = new Vector2(0, -1);
                else if (Raylib.IsKeyPressed(Down))
                    Direction = new Vector2(0, 1);
            }
        }
    }
}
