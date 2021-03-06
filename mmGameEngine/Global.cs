﻿using System;
using System.Collections.Generic;
using System.Text;

using System.Numerics;
using Entitas;
using Raylib_cs;


namespace mmGameEngine
{
    public class Global
    {
        //
        // Special Render Layers
        //
        public const int BOXCOLLIDER_LAYER = -1000;         
        public const int TILEMAP_LAYER = -1500;
        public const int SCROLLINGBACK_LAYER = -2000;       //first to draw
        public const int CURSOR_LAYER = 100000;               //last to draw
        //
        // Game state
        //
        public static bool GameOver = false;
        public static GameState StateOfGame;
        public static int GameScore;
        public static Int64 FrameCount;

        //
        // Game Windows setup for mmGame
        //
        public static int SceneWidth;
        public static int SceneHeight;
        public static string SceneTitle;
        public static Color SceneClearColor;
        public static bool HideCursor = false;
        //
        // Windows size
        //
        //public static int ViewPortWidth;
        //public static int ViewPortHeight;
        public static Vector2 WindowCenter = new Vector2(SceneWidth / 2, SceneHeight / 2);
        public static int TARGET_FPS = 60;
        public static float DeltaTime;
        //
        // World size (using myown camera)
        //
        //
        // Camera
        //
        public static RectangleF SceneView;
        public static RectangleF WorldView;
        public static int WorldWidth;
        public static int WorldHeight;
        //
        // Entitas ECS context for Game/Scene UI
        //
        public static Entitas.Context EntityContext;
        public static Entitas.Context SceneContext;
        //
        // Current game scene
        //
        public static Scene CurrentScene;
        public static string NextScene;
        //
        // Default text fonts loaded to engine
        //
        public static Font[] EngineFonts;

        public static bool DebugRenderEnabled = false;
        /// <summary>
        /// use method AddEntityToDestroy() . Don't add directly
        /// </summary>
        public static Dictionary<Entity, bool> GameEntityToDestroy;
        public static Dictionary<Entity, bool> SceneEntityToDestroy;
        public static void LoadFonts()
        {
            EngineFonts = new Font[7];

            EngineFonts[0] = Raylib.LoadFont("AssetsEngine/Fonts/Default.ttf");
            EngineFonts[1] = Raylib.LoadFont("AssetsEngine/Fonts/Default2.ttf");
            EngineFonts[2] = Raylib.LoadFont("AssetsEngine/Fonts/arial.ttf");
            EngineFonts[3] = Raylib.LoadFont("AssetsEngine/Fonts/toon.ttf");
            EngineFonts[4] = Raylib.LoadFont("AssetsEngine/Fonts/VeraMono.ttf");
            EngineFonts[5] = Raylib.LoadFont("AssetsEngine/Fonts/Digitalt.ttf");
            EngineFonts[6] = Raylib.LoadFont("AssetsEngine/Fonts/OpenSans.ttf");
        }
        public static bool ActiveCurrentScene(ISystem _current)
        {
            if (_current.GetType().Name == CurrentScene.GetType().Name)
                return true;
            return false;
        }
        /// <summary>
        /// Add an entity/children to be destroyed after Scene Update/Render is done
        /// </summary>
        /// <param name="entity"></param>
        public static void DestroyGameEntity(Entity entity)
        {
            //
            // All children added first (to be removed)
            //
            if (entity.Get<Transform>().ChildCount > 0)
            {
                foreach (Component child in entity.Get<Transform>().Children)
                {
                    GameEntityToDestroy.TryAdd(child.CompEntity, true);
                    SceneColliderDatabase.RemoveCollider(child.CompEntity);
                }
            }
            //
            // Add entity to be removed
            //
            GameEntityToDestroy.TryAdd(entity, true);
            SceneColliderDatabase.RemoveCollider(entity);
        }
        public static void DestroySceneEntity(Entity entity)
        {
            //
            // All children added first (to be removed)
            //
            if (entity.Get<Transform>().ChildCount > 0)
            {
                foreach (Component child in entity.Get<Transform>().Children)
                {
                    SceneEntityToDestroy.TryAdd(child.CompEntity, true);
                }
            }
            //
            // Add entity to be removed
            //
            SceneEntityToDestroy.TryAdd(entity, true);
        }
        /// <summary>
        /// Rotate so the top of the sprite is facing <see cref="pos"/>
        /// </summary>
        /// <param name="pos">The position to look at</param>
        public static float LookAt(Vector2 _lookatposition, Vector2 _myPosition)
        {
            int sign = _myPosition.X > _lookatposition.X ? -1 : 1;
            System.Numerics.Vector2 vectorToAlignTo = System.Numerics.Vector2.Normalize(_myPosition - _lookatposition);
            float rotationInRadians = sign * Mathf.Acos(System.Numerics.Vector2.Dot(vectorToAlignTo, System.Numerics.Vector2.UnitY));

            float RotationDegrees = rotationInRadians * 57.2958f;            //convert to degrees for Raylib (1 rad * 180/pi)
            return RotationDegrees;
        }

        #region // Camera2D  \\
        //znznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
        // When camera is used, all coordinants must come here
        //znznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
        public static Vector2 WorldPosition(Vector2 _position)
        {
            if (!CurrentScene.CameraEnabled)
                return _position;

            Vector2 pos = Raylib.GetScreenToWorld2D(_position, CurrentScene.Camera);
            return pos;
        }
        public static Vector2 GetMousePosition()
        {
            if (!CurrentScene.CameraEnabled)
                return Raylib.GetMousePosition();

            Vector2 pos = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), CurrentScene.Camera);
            return pos;
        }
        public static int GetMouseX()
        {
            if (!CurrentScene.CameraEnabled)
                return Raylib.GetMouseX();

            Vector2 pos = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), CurrentScene.Camera);
            return (int)pos.X;
        }
        public static int GetMouseY()
        {
            if (!CurrentScene.CameraEnabled)
                return Raylib.GetMouseY();

            Vector2 pos = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), CurrentScene.Camera);
            return (int)pos.Y;
        }
        #endregion
        public static bool EntityOutOfBound(Vector2 _pos)
        {
            if ((_pos.X >= Global.WorldWidth || _pos.X <= 0) || (_pos.Y >= Global.WorldHeight || _pos.Y <= 0))
                return true;

            return false;
        }
        public static void CreateWorld(int _width, int _height)
        {
            //
            // world starts at 0,0
            //
            WorldWidth = _width;
            WorldHeight = _height;
            WorldView = new RectangleF(0, 0, _width, _height);
        }



    }
    public enum GameState
    {
        Playing,
        Paused,
        ForcedExit,
        Over
    }
    public enum Edge
    {
        Top,
        Bottom,
        Left,
        Right
    }
    public enum Camera2DType
    {
        FollowPlayer,
        FollowInsideMap,
        FollowCenterSmooth
    }
    public enum CollidreShape
    {
        Box,
        Circle
    }
}
