using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using System.Reflection;

using mmGameEngine;
using Transform = mmGameEngine.Transform;

using Entitas;


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
            Global.SceneClearColor = Color.BLANK;
            SceneNames = new string[3]
                {"PlayScene", "","CardScene"};

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
            //-----------------------------
            // Background
            //-----------------------------
            Entity entBack = CreateGameEntity(new Vector2(0, 0));
            entBack.name = "background";
            Sprite sp = new Sprite(Content.backGround);
            sp.FitWindow = true;
            entBack.Add(sp);

            entPanel = CreateSceneEntity(new Vector2(150, 200));                //position of panel
            entPanel.name = "panel";
            position = entPanel.Get<Transform>().Position;
            //-----------------------------
            // Panel (menu type)
            //-----------------------------
            Panel menuPanel = new Panel(position, 300, 250, Color.BLANK);
            //
            // add a label on top
            //
            Label lbl = new Label(new Vector2(80, 10), "This is a Menu");
            menuPanel.AddComponent(lbl);
            //
            // play button (position is with respect to the panel)
            //
            Button playBt = new Button(new Vector2(110, 40), 74, 35, "Play", -2, +5);
            playBt.Tag = 1;
            playBt.Image = Content.buttonEmpty;
            playBt.Click += ActionButton;
            menuPanel.AddComponent(playBt);
            //
            // Map button
            //
            Button setupBt = new Button(new Vector2(110, 85), 74, 35, "Options", -27, +5);
            setupBt.Tag = 2;
            setupBt.Image = Content.buttonEmpty;
            setupBt.Click += ActionButton;
            menuPanel.AddComponent(setupBt);
            //
            // Cards button
            //
            Button cardBt = new Button(new Vector2(110, 130), 74, 35, "Cards", -12, +5);
            cardBt.Tag = 3;
            cardBt.Image = Content.buttonEmpty;
            cardBt.Click += ActionButton;
            menuPanel.AddComponent(cardBt);
            //
            // Exit button
            //
            Button exitBt = new Button(new Vector2(110, 175), 74, 35, "Exit", -2, +5);
            exitBt.Tag = 4;
            exitBt.Image = Content.buttonEmpty;
            exitBt.Click += ExitButton;
            menuPanel.AddComponent(exitBt);

            entPanel.Add(menuPanel);

        }
        public void ExitButton(object btn)
        {
            ForceEndScene = true;
        }
        public void ActionButton(object btn)
        {
            Button bt = (Button)btn;
            int SceneNum = bt.Tag - 1;
            Global.NextScene = assName + "." + SceneNames[SceneNum];
            //
            // Using reflection to invoke the Scene
            //
            Type scene2play = assmbly.GetType(Global.NextScene);
            ConstructorInfo sceneInfo = scene2play.GetConstructor(Type.EmptyTypes);
            object sceneObj = sceneInfo.Invoke(new object[] { });
            mmGame.Scene = (Scene)sceneObj;

            //Global.NextScene = "TestmmGame.CardScene";
            //Scene otherScene = new CardScene();
            //mmGame.Scene = otherScene;
        }
    }

}
