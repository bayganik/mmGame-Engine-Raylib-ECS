using Entitas;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngine
{
    /// <summary>
    /// Prefabricated entities with certain components
    /// </summary>
    public class PrefabEntity
    {
        /// <summary>
        /// Cursor with/without image + box collider + MouseComponent
        /// </summary>
        /// <param name="textureImage"></param>
        /// <returns></returns>
        public static Entity CreateCursorEntity(string textureImage = null)
        {
            Entity CursorEnt = Global.CreateSceneEntity(Vector2.Zero);
            CursorEnt.Name = "cursor";
            CursorEnt.Tag = 1000;
            //
            // Image to move with mouse
            //
            if (textureImage != null) 
            {
                Texture2D txt = Raylib.LoadTexture(textureImage);
                Sprite Spr = new Sprite(textureImage);
                Spr.RenderLayer = Global.CURSOR_LAYER;      //on top of everything
                CursorEnt.Add(Spr);
                Raylib.HideCursor();
            }
            else
                Raylib.ShowCursor();
            //
            // Add small box collider if we click on anything
            //
            BoxCollider bxxx = new BoxCollider(8, 8);
            CursorEnt.Add(bxxx);
            CursorEnt.Add<MouseComponent>();

            return CursorEnt;
        }

    }
}
