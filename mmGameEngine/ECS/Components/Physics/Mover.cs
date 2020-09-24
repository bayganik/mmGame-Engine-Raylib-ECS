using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;


namespace mmGameEngine
{
    public class Mover : Component
    {
        //
        // Following items are data to be used by calling processing system
        //      they are NOT used by this component automatically
        //
        public float Speed = 200f;
        public Vector2 Direction;

        public Vector2 MoveFrom;                    //From/To are holding places they do not work here
        public Vector2 MoveTo;
        public bool IsMoving;                       //entity still is moving (true/false)
        public CollisionResult MoveCollisionResult; //if we hit any other collider
        public Mover()
        {

        }
    }
}
