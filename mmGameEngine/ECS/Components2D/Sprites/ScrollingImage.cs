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
		public float ScrollSpeedX = 15;                 //default is horizontal move
		/// <summary>
		/// fill entire screen with the image
		/// </summary>
        public bool FitWindow = false;
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
			get => (int)_sourceRect.X;
			set => _sourceRect.X = value;
		}

		/// <summary>
		/// y value of the texture scroll
		/// </summary>
		/// <value>The scroll y.</value>
		public int ScrollY
		{
			get => (int)_sourceRect.Y;
			set => _sourceRect.Y = value;
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
			get => (int)_sourceRect.Width;
			set
			{
				_areBoundsDirty = true;
				_sourceRect.Width = value;
			}
		}
		public int Height
		{
			get => (int)_sourceRect.Height;
			set
			{
				_areBoundsDirty = true;
				_sourceRect.Height = value;
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
			_sourceRect = new Rectangle(0, 0, Texture.Width, Texture.Height);
			RenderLayer = Global.SCROLLINGBACK_LAYER;

		}
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
			if (OwnerEntity == null)
				return;
			if (!Enabled)
				return;

			_scrollX += ScrollSpeedX * deltaTime;
			_scrollY += ScrollSpeedY * deltaTime;
			_sourceRect.X = (int)_scrollX;
			_sourceRect.Y = (int)_scrollY;
		}
        public override void Render()
        {
			if (OwnerEntity == null)
				return;
			if(!OwnerEntity.IsVisible)
				return;
			if (!Enabled)
				return;
            //
            // Origin is always 0,0
            //
            TransformComponent Transform = OwnerEntity.Get<TransformComponent>();
            var topLeft = Transform.Position + _localOffset;

			Rectangle _destRect = new Rectangle(topLeft.X, topLeft.Y,
						Texture.Width * Transform.Scale.X,
						Texture.Height * Transform.Scale.Y);
            if (FitWindow)
            {
                _destRect = new Rectangle(Transform.Position.X, Transform.Position.Y,
                         Global.SceneWidth * Transform.Scale.X,
                         Global.SceneHeight * Transform.Scale.Y);
                Origin = Vector2.Zero;
            }
            Raylib.DrawTexturePro(Texture,
					  _sourceRect,
					  _destRect,
					  Vector2.Zero,
					  Transform.Rotation,
					  Color.White);
		}
    }
}
