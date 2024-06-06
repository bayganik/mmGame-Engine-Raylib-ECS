using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using System.Reflection;

using mmGameEngine;
using Transform = mmGameEngine.TransformComponent;

using Entitas;
//using System.Reflection.Emit;


namespace TestmmGame
{
    /*
     * Scene entities do not obey camera.  They are always within the screen space
     * Game  entities do obey camera.  Must call Global.GetMousePosition() or
     *                                           Global.GetMouseX()
     *                                           Global.GetMouseY()
     *                                           Global.WorldPosition(Vector2)
     */
    public class MenuScene : Scene
    {
        //
        // Part of scene collection that we access in TestmmGame
        //
        Assembly assmbly;
        string assName;             //full assembly name
        string[] SceneNames;        //Scenes we want to invoke

        Entity entPanel;
        Vector2 position;
        public MenuScene()
        {
            Global.SceneHeight = 600;
            Global.SceneWidth = 600;
            Global.SceneTitle = "Game Menu Scene";
            Global.SceneClearColor = Color.Blank;
            SceneNames = new string[4]
                {"PlayScene", "SplashScene","CardScene","PianoScene"};

            assName = (string)System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            assmbly = Assembly.Load(assName);

        }
        public override void Play()
        {
            Global.CurrentScene = this;
            Global.DebugRenderEnabled = false;
            //
            // Contents
            //
            Content.LoadMenu();
            //------------------------------
            // Scrolling background
            //------------------------------
            Entity backG = Global.CreateGameEntity(new Vector2(0, 0));
            backG.Name = "Scrolling";
            //textureImage = Raylib.LoadTexture("Assets/Img/background.png");
            ScrollingImage si = new ScrollingImage(Content.backGround);
            si.ScrollSpeedX = 10;
            si.FitWindow= true;

            backG.Add(si);
            backG.Get<Transform>().Visiable = true;
            //-----------------------------
            // Panel (menu type)
            //-----------------------------
            entPanel = Global.CreateSceneEntity(new Vector2(150, 150));         //position of panel
            entPanel.Name = "panel";
            position = entPanel.Get<Transform>().Position;

            Panel menuPanel = new Panel(position, 300, 300, Color.Blank);
            entPanel.Add(menuPanel);

            //-----------------
            // label on top
            //-----------------
            Label lbl = new Label("This is a Menu");
            menuPanel.AddComponent(lbl, new Vector2(80, 10));

            //--------------------------------------------------------
            // play button (position relative to the panel)
            //--------------------------------------------------------
            Button playBt = new Button(74, 35, "Play", -2, +5);
            playBt.Tag = 1;
            playBt.Click += ActionButton;
            menuPanel.AddComponent(playBt, new Vector2(110, 40));

            //-------------------
            // splash button
            //-------------------
            Button setupBt = new Button( 74, 35, "Splash", -25, +5);
            setupBt.Tag = 2;
            setupBt.Click += ActionButton;
            menuPanel.AddComponent(setupBt, new Vector2(110, 80));
            //-----------------
            // Cards button
            //-----------------
            Button cardBt = new Button(74, 35, "Cards", -12, +5);
            cardBt.Tag = 3;
            cardBt.Click += ActionButton;
            menuPanel.AddComponent(cardBt, new Vector2(110, 120));
            //---------------------
            // extra Play button
            //---------------------
            Button tankBt = new Button( 74, 35, "Piano", -12, +5);
            tankBt.Tag = 4;
            tankBt.Click += ActionButton;
            menuPanel.AddComponent(tankBt, new Vector2(110, 170));
            //------------------------------------------------------
            // Exit button (using Entity to show mixing things !!
            //------------------------------------------------------
            Entity genEnt = Global.CreateSceneEntity(new Vector2(110, 210));
            Button exitBt = new Button(74, 35, "Exit", -2, +5);
            exitBt.Tag = 5;
            exitBt.Click += ExitButton;
            genEnt.Add(exitBt);
            genEnt.Get<Transform>().Parent = entPanel.Get<Transform>();

            //
            // Move the panel, all components within it move as well
            //
            //entPanel.Get<Transform>().Position = new Vector2(10, 10);
        }
        public void ExitButton(object btn)
        {
            ForceEndScene = true;
        }
        public void ActionButton(object btn)
        {
            Button bt = (Button)btn;
            int SceneNum = bt.Tag - 1;
            if (string.IsNullOrEmpty(SceneNames[SceneNum]))
                return;

            Global.NextScene = assName + "." + SceneNames[SceneNum];
            //
            // Using reflection to invoke the Scene
            //
            Type scene2play = assmbly.GetType(Global.NextScene);
            ConstructorInfo sceneInfo = scene2play.GetConstructor(Type.EmptyTypes);
            object sceneObj = sceneInfo.Invoke(new object[] { });
            mmGame.Scene = (Scene)sceneObj;                 //Scene is static field

            //Global.NextScene = "TestmmGame.CardScene";
            //Scene otherScene = new CardScene();
            //mmGame.Scene = otherScene;
        }
    }

}
