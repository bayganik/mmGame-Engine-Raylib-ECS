using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using Entitas;
using mmGameEngine;
using Transform = mmGameEngine.Transform;
using Raylib_cs;

namespace TestmmGame
{
    public class MouseComponent : Component
    {
        public Entity CurrentEntityChosen;
        //
        // entity moving on its own
        //
        public bool IsMoving;

        public MouseComponent()
        {

        }
    }
    public class ScoreComponent : Component
    {
        public string ScoreWords = "Score : ";
        public int Score = 0;
        public ScoreComponent()
        {
            Score = 0;
        }
    }
    public class DragComponent : Component
    {
        /*
         * Attached to an entity that is being dragged across scene
         */
        public Entity EntityOrig;                   //Stack Entity cards came from

        public DragComponent()
        {

        }
    }
    public class GameStatComponent : Component
    {
        public string ScoreWords = "Score : ";
        public int Score = 0;
        public bool EndOfGame = false;
        public GameStatComponent()
        {

        }
    }
}
