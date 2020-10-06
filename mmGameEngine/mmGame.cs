using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;

namespace mmGameEngine
{
    /*
     * The core of our game.  
     * 
     * The game class MUST inhirit from mmGame (then you can change scenes)
     *
     *public class TestGame : mmGame
     *{
     *   public TestGame() : base()
     *   {
     *       Scene = new StartScene();          //could be Splash Or Menu Or a game
     *   }
     *}
     */
    public class mmGame
    {
        public Color WindowClearColor;                                  //Can be changed
        public KeyboardKey ExitKey = KeyboardKey.KEY_ESCAPE;            //Can be changed by user
        public bool ForceEndScene = false;
        /// <summary>
        /// provides access to the singlton mmGame/Scene instance
        /// </summary>
        public static mmGame Instance => _instance;
        /// <summary>
        /// facilitates easy access to the global Content instance for internal classes
        /// </summary>
        internal static mmGame _instance;
        //
        // Scene variables for internal use
        //
        Scene _scene;
        Scene _nextScene;
        /// <summary>
        /// The currently active Scene. Note that if set, the Scene will not actually change 
        /// until the end of the Update
        /// </summary>
        public static Scene Scene
        {
            get => _instance._scene;
            set
            {
                if (_instance._scene == null)                       //first Scene comes here
                {
                    _instance._nextScene = null;
                    _instance._scene = value;                       //scene object
                    _instance._scene.Game = _instance;              //tell the scene where Game is 
                    Global.CurrentScene = value;

                    _instance._scene.Begin();                       //setup of entity context, lists, etc

                    _instance.SetSceneWindow();                     //create a new Raylib window
                    _instance._scene.Play();                        //do game logic - load assets, entities
                    _instance.RunGameLoop();                        //run the loop
                }
                else
                {
                    _instance._nextScene = value;
                }
            }
        }

        protected mmGame()
        {
            _instance = this;
            Global.GameState = GameState.Playing;
            Global.GameOver = false;
        }
        internal void RunGameLoop()
        {
            /*
             * Game loop to allow scenes to change.  Typically first scene can be a menu 
             * Calling other scenes
             * or
             * In case of RPG games, the scene may change frequently
             */

            while (true)
            {
                //-----------------------------------
                //update and render game loop
                //-----------------------------------
                Run();                                              
                //
                // We are here because game loop was intrupted
                // scene changed or user forced out by pressing ESCAPE
                //
                // EACH SCENE SHUTS DOWN PREVIOUS WINDOW & STARTS A NEW WINDOW
                //
                if (_nextScene != null)
                {
                    _scene.End();
                    _scene = _nextScene;
                    Global.CurrentScene = _scene;

                    _nextScene = null;

                    _scene.Game = this;
                    OnSceneChanged();                               // clean up, start the new Scene/Window
                    _scene.Begin();                                 //setup of entity context, lists, etc

                    _instance.SetSceneWindow();                     //create a new Raylib window
                    _instance._scene.Play();                        //do game logic - load assets, entities

                }
                else
                    break;                                          //game is done
            }
        }
        /// <summary>
        /// Called after a Scene ends, before the next Scene begins
        /// </summary>
        void OnSceneChanged()
        {
            GC.Collect();
        }
        public void SetSceneWindow()
        {
            //
            // setup game window
            //
            Raylib.SetConfigFlags(ConfigFlag.FLAG_WINDOW_RESIZABLE);
            //
            // This allows all other operations for RayLib 
            //
            Raylib.InitWindow(Global.SceneWidth, Global.SceneHeight, Global.SceneTitle);
            Raylib.SetWindowMinSize(600, 4000);
            Raylib.SetWindowPosition(200, 100);
            Raylib.SetTargetFPS(Global.TARGET_FPS);
            if (!Raylib.IsAudioDeviceReady())
                Raylib.InitAudioDevice();

            Global.WindowCenter = new Vector2(Global.SceneWidth / 2, Global.SceneHeight / 2);
            Global.LoadFonts();

            Raylib.SetExitKey(ExitKey);
        }
        //
        //  We are here because a new scene was activated.  We will be running this until
        //  a ESCAPE is pressed or Scene changes.
        //
        public void Run()
        {
            //
            // actual scene loop
            //
            Global.FrameCount = 0;                      //Scene may affect this counter
            while (!Raylib.WindowShouldClose())
            {
                Global.FrameCount += 1;
                //
                // Test for debug key F9 (F12 is used for screenshot by RayLib)
                //
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_F9))
                {
                    Global.DebugRenderEnabled = !Global.DebugRenderEnabled;
                }

                Update();

                if (ForceEndScene)
                    break;
                //
                // remove deleted/destroyed entities
                //
                _scene.RemoveDeletedEntities();

                Render();


                //------------------------------------
                // New scene is given
                //------------------------------------
                if (_nextScene != null)
                    break;
            }
            //
            // Exit key or Scene change
            //
            Raylib.CloseWindow();

        }
        public void Update()
        {

            if (_scene == null)
                return;

            _scene.Update();                        // Update current scene

        }
        public void Render()
        {
            if (_scene == null)
                return;

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Global.SceneClearColor);

            _scene.Render();              //give the scene the "RenderTarget"

            Raylib.EndDrawing();

        }

    }
}
