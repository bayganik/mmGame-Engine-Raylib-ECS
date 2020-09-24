using System;
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
        public const int CURSOR_LAYER = 1000;               //last to draw
        //
        // Game state
        //
        public static bool EndOfGame = false;
        public static string GameState;
        //
        // Game Windows setup for mmGame
        //
        public static int SceneWidth;
        public static int SceneHeight;
        public static string SceneTitle;
        public static Color SceneClearColor;
        //
        // Windows size
        //
        public static int ViewPortWidth;
        public static int ViewPortHeight;
        public static Vector2 WindowCenter = new Vector2(ViewPortWidth / 2, ViewPortHeight / 2);
        public const int TARGET_FPS = 60;
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

        public static bool DebugRenderEnabled = false;
        /// <summary>
        /// use method AddEntityToDestroy() . Don't add directly
        /// </summary>
        public static Dictionary<Entity, bool> GameEntityToDestroy;
        public static Dictionary<Entity, bool> SceneEntityToDestroy;
        public static bool ActiveCurrentScene(ISystem _current)
        {
            if (_current.GetType().Name == CurrentScene.GetType().Name)
                return true;
            return false;
        }
        /// <summary>
        /// Add and entity/children to be destroyed when Update/Render cycle is done
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
                    SceneColliders.RemoveCollider(child.CompEntity);
                }
            }
            //
            // Add entity to be removed
            //
            GameEntityToDestroy.TryAdd(entity, true);
            SceneColliders.RemoveCollider(entity);
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

        public static void CreateWorld(int _width, int _height)
        {
            //
            // world starts at 0,0
            //
            WorldWidth = _width;
            WorldHeight = _height;
            WorldView = new RectangleF(0, 0, _width, _height);
        }
        public static void CreateViewport (int _x, int _y, int _width, int _height)
        {
            //
            // Viewport starts at a particular location in the world
            //
            SceneView = new RectangleF(0,0,SceneWidth, SceneHeight);
        }


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
}
