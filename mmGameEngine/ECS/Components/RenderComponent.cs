using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;
using Entitas;

namespace mmGameEngine
{

    /*
	 * Same as component, with additional fields, a single delegate (used for buttons)
	 * and ability to invoke Render() method in our Scene class
	 */
    public delegate void ClickEventHandler(object obj);
    public class RenderComponent : Component, IRenderable 
	{
        public event ClickEventHandler Click;

        public int RenderLayer;                     //render from low to high
        /// <summary>
        /// sprite image
        /// </summary>
        public Texture2D Texture;
        /// <summary>
        /// center of the sourceRect if it had a 0,0 origin. This is basically the center in sourceRect-space.
        /// </summary>
        /// <value>The center.</value>
        public Vector2 TextureCenter;
        /// <summary>
        /// the origin that a RenderableComponent should use when using this Sprite. Defaults to the center.
        /// </summary>
        public Vector2 Origin;
        public Vector2 OriginLocal;				//pre Determined origin to override Origin

        public RenderComponent()
        {
            Tag = 0;
            Enabled = true;
        }
        public virtual void Render()
        { }

        public void OnClick(object obj) { Click?.Invoke(obj); }
    }
}
