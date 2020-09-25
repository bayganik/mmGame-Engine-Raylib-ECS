using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;


namespace mmGameEngine
{
	/*
	 * Scrolling background image with speed control
	 */
    public class ScrollingImage : RenderComponent
    {
		/// <summary>
		/// x speed of automatic scrolling in pixels/s
		/// </summary>
		public float ScrollSpeedX = 15;					//default is horizontal move

		/// <summary>
		/// y speed of automatic scrolling in pixels/s
		/// </summary>
		public float ScrollSpeedY = 0;					//vertial move
		/// <summary>
		/// x value of the texture scroll
		/// </summary>
		/// <value>The scroll x.</value>
		public int ScrollX
		{
			get => (int)_sourceRect.x;
			set => _sourceRect.x = value;
		}

		/// <summary>
		/// y value of the texture scroll
		/// </summary>
		/// <value>The scroll y.</value>
		public int ScrollY
		{
			get => (int)_sourceRect.y;
			set => _sourceRect.y = value;
		}
		/// <summary>
		/// scale of the texture
		/// </summary>
		/// <value>The texture scale.</value>
		public Vector2 TextureScale
		{
			get => _textureScale;
			set
			{
				_textureScale = value;

				// recalulcate our inverseTextureScale and the source rect size
				_inverseTexScale = new Vector2(1f / _textureScale.X, 1f / _textureScale.Y);
			}
		}
		public int Width
		{
			get => (int)_sourceRect.width;
			set
			{
				_areBoundsDirty = true;
				_sourceRect.width = value;
			}
		}
		public int Height
		{
			get => (int)_sourceRect.height;
			set
			{
				_areBoundsDirty = true;
				_sourceRect.height = value;
			}
		}
		/// <summary>
		/// we keep a copy of the sourceRect so that we dont change the Sprite in case it is used elsewhere
		/// </summary>
		protected Rectangle _sourceRect;

		protected Vector2 _textureScale = Vector2.One;
		protected Vector2 _inverseTexScale = Vector2.One;
		protected Vector2 _localOffset;
		protected bool _areBoundsDirty = true;

		// accumulate scroll in a separate float so that we can round it without losing precision for small scroll speeds
		float _scrollX, _scrollY;
		public ScrollingImage(Texture2D initTexture)
        {
			Texture = initTexture;
			ScrollSpeedX = 15;
			ScrollSpeedY = 0;
			_sourceRect = new Rectangle(0, 0, Texture.width, Texture.height);
			RenderLayer = Global.SCROLLINGBACK_LAYER;

		}
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
			if (CompEntity == null)
				return;

			_scrollX += ScrollSpeedX * deltaTime;
			_scrollY += ScrollSpeedY * deltaTime;
			_sourceRect.x = (int)_scrollX;
			_sourceRect.y = (int)_scrollY;
		}
        public override void Render()
        {
            base.Render();
			//
			// Origin is always 0,0
			//
			var topLeft = Transform.Position + _localOffset;

			Rectangle _destRect = new Rectangle(topLeft.X, topLeft.Y,
						Texture.width * Transform.Scale.X,
						Texture.height * Transform.Scale.Y);

			Raylib.DrawTexturePro(Texture,
					  _sourceRect,
					  _destRect,
					  Vector2.Zero,
					  Transform.Rotation,
					  Color.WHITE);
		}
    }
}
