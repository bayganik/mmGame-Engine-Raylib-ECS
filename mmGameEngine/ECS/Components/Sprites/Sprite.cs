using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;


namespace mmGameEngine
{
	/*
	 * Main component to display a texture for an entity
	 */
    public class Sprite : RenderComponent
    {
		//
		// Draw line data (from init pos to current pos)
		//
		public bool EnableTracer = false;   
		public Vector2 InitialPosition;
		public Color TracerColor = Color.RED;
		public float TracerThick = 1.0f;

		public bool FitWindow = false;
		/// <summary>
		/// rectangle in the Texture2D for this element
		/// </summary>
		public Rectangle SourceRect;
		/// <summary>
		/// rectangle in the Texture2D for this element
		/// </summary>
		public Rectangle DestRect;

		public bool OriginReCalc = true;					//force the first update to get correct Origin
		public Sprite(string filePath)
        {
			Texture = Raylib.LoadTexture(filePath);
			//Position = Vector2.Zero;

			Initialize();
		}
		public Sprite(Texture2D initTexture)
		{
			Texture = initTexture;
			//Position = Vector2.Zero;

			Initialize();
		}
		private void Initialize()
        {
			SourceRect = new Rectangle(0, 0, Texture.width, Texture.height);
			TextureCenter = new Vector2(Texture.width * 0.5f, Texture.height * 0.5f);
			OriginLocal = new Vector2(Texture.width * 0.5f , Texture.height * 0.5f );
		}
		
        public override void Update(float deltaTime)
        {
			base.Update(deltaTime);
			//
			// If Entity has not been attached, then leave
			//
			if (CompEntity == null)
				return;
			if (!Enabled)
				return;
			//
			// At this time, we have the Entity's "Transform" component assigned by Scene or find Origin
			//		using the Scale
			//
			TextureCenter = new Vector2(Texture.width * 0.5f * Transform.Scale.X, Texture.height * 0.5f * Transform.Scale.Y);

			if (OriginReCalc)
            {
				InitialPosition = Transform.Position;
				Origin = new Vector2(OriginLocal.X * Transform.Scale.X, 
									 OriginLocal.Y * Transform.Scale.Y);
                OriginReCalc = false;
            }
        }
        public override void Render()
        {
			//
			// If Entity has not been attached, then leave
			//
			if (CompEntity == null)
				return;
			if (!Enabled)
				return;
			//
			// Destination Rect depends on position & scale
			//
			if (FitWindow)
			{
				DestRect = new Rectangle(Transform.Position.X, Transform.Position.Y,
						 Global.SceneWidth * Transform.Scale.X,
						 Global.SceneHeight * Transform.Scale.Y);
				Origin = Vector2.Zero;
			}
			else
			{
				DestRect = new Rectangle(Transform.Position.X, Transform.Position.Y,
										 Texture.width * Transform.Scale.X,
										 Texture.height * Transform.Scale.Y);
			}
			if (EnableTracer)
			{
				//
				// line ends at the Origin of the sprite
				//
				Raylib.DrawLineEx(InitialPosition, Transform.Position, TracerThick, TracerColor);
			}
			//
			// Draw actual texture image
			//
			Raylib.DrawTexturePro(Texture,
                                  SourceRect,
                                  DestRect,
                                  Origin,
                                  Transform.Rotation,
                                  Color.WHITE);

		}
    }
}
