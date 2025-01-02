using System;
using System.Numerics;
using System.Collections.Generic;
using System.Text;
using mmGameEngine;
using Raylib_cs;
using System.Reflection;

namespace TestmmGame
{
    /*
     * Special splash scene using its own Update/Render override
     * This means Scene ECS is completely ignored.  You must handle 
     * all aspects of a scene.
     */
    public class SplashScene : Scene
    {
        private int _frame = 0;
        Texture2D background = new Texture2D();
        Assembly assmbly;                           // = Assembly.Load("TestmmGame");
        public SplashScene() 
        {
            Global.SceneHeight = 600;
            Global.SceneWidth = 800;
            Global.SceneTitle = "splash";
            Global.SceneClearColor = Color.Blue;

            string assName = (string)System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            Global.NextScene = assName + ".MenuScene";              //make sure it has a dot TestmmGame.MenuScene
        }
        public override void Play()
        {

            background = Raylib.LoadTexture("Assets/SplashScreen.png");
            //
            // Start the game loop for this scene
            //
        }
        public override void Update()
        {
            //
            // We are overriding Scene Update(), so ECS will not work in this scene
            //
            _frame++;

            if (_frame > 200)
            {
                //
                // Tell game engine to activate a specific scene
                //
                mmGame.ActiveScene = Global.NextScene;
            }
        }
        public override void Render()
        {
            //
            // Draw the text
            //
            Raylib.DrawText("Splash screen will go back to menu", 200, 10, 25, Color.Yellow);
            //
            // show the mmGame image
            //
            Raylib.DrawTexturePro(background, 
                                  new Rectangle(0,0,background.Width, background.Height), 
                                  new Rectangle(0,0,800,600), 
                                  new Vector2(0,0), 0, Color.White);

        }
    }
}
