using System;
using System.Collections.Generic;
using System.Text;

using Raylib_cs;
using mmGameEngine;
using System.Reflection;

namespace TestmmGame
{
    public class TestGame : mmGame
    {
        public TestGame() : base()
        {
            Scene = new SplashScene();
        }
    }
}

