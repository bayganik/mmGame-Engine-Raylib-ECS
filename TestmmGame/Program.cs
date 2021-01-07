using System;
using Raylib_cs;
using mmGameEngine;

namespace TestmmGame
{
    class Program
    {
        static void Main(string[] args)
        {

            TestGame game = new TestGame();

        }
    }
    public class TestGame : mmGame
    {
        public TestGame() : base()
        {
            Scene = new CardScene();
        }
    }
}
