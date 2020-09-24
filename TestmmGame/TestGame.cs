using System;
using System.Collections.Generic;
using System.Text;

using Raylib_cs;
using mmGameEngine;
using System.Reflection;

namespace TestmmGame
{
    //public class TestGame 
    //{
    //    public TestGame() 
    //    {


    //        //SplashScene ss = new SplashScene();
    //        //
    //        // Splash scene ends after 3000 frames
    //        //
    //        // PlayScene ps = new PlayScene();
    //        // or
    //        // use reflection to invoke multiple scenes
    //        //
    //        Global.NextScene = "TestmmGame.PlayScene";

    //          USING REFLECTION TO ACTIVATE A SCENE

    //        Assembly assm = Assembly.Load("TestmmGame");
    //        while (!Global.EndOfGame)
    //        {
    //            Type scene2play = assm.GetType(Global.NextScene);
    //            ConstructorInfo sceneCI = scene2play.GetConstructor(Type.EmptyTypes);
    //            object sceneObject = sceneCI.Invoke(new object[] { });
    //            //
    //            // We are here, scene was forced to exit
    //            //
    //        }
    //    }
    //}
    public class TestGame : mmGame
    {
        public TestGame() : base()
        {
            Scene = new SplashScene();
        }
    }
}

