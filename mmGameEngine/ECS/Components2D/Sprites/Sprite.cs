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
		// Draw tracer line data (from init pos to current pos)
		//
		public bool EnableTracer = false;   
		public Vector2 InitialPosition;
		public Color TracerColor = Color.Red;
		public float TracerThick = 1.0f;
		//
		// Fit entire sprite in a window
		//
		public bool FitWindow = false;
		public Rectangle SourceRect;                //source rectangle in the Texture for this element
        public Rectangle DestRect;					//rectangle (position & size) for display

		public bool OriginReCalc = true;            //force the first update to get correct Origin
		public Color DrawColor = Color.White;		//default color for drawing is White
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
			SourceRect = new Rectangle(0, 0, Texture.Width, Texture.Height);
			TextureCenter = new Vector2(Texture.Width * 0.5f, Texture.Height * 0.5f);
			Origin = new Vector2(Texture.Width * 0.5f , Texture.Height * 0.5f );
		}
		
        public override void Update(float deltaTime)
        {
			base.Update(deltaTime);

			if (OwnerEntity == null)
				return;
			if(!OwnerEntity.IsVisible)
				return;
			if (!Enabled)
				return;
            //
            // At this time, we have the Entity's "Transform" component assigned by Scene or find Origin
            //		using the Scale
            //
            TransformComponent Transform = OwnerEntity.Get<TransformComponent>();
            TextureCenter = new Vector2(Texture.Width * 0.5f * Transform.Scale.X, Texture.Height * 0.5f * Transform.Scale.Y);

			if (OriginReCalc)
            {
				InitialPosition = Transform.Position;
				Origin = new Vector2(Origin.X * Transform.Scale.X, 
									 Origin.Y * Transform.Scale.Y);
                OriginReCalc = false;
            }
        }
        public override void Render()
        {
			//
			// If Entity has not been attached, then leave
			//
			if (OwnerEntity == null)
				return;
			if (!Enabled)
				return;
			if (!OwnerEntity.IsVisible)
				return;
            //
            // Destination Rect depends on position & scale
            //
            TransformComponent Transform = OwnerEntity.Get<TransformComponent>();
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
										 Texture.Width * Transform.Scale.X,
										 Texture.Height * Transform.Scale.Y);
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
                                  DrawColor);

		}
    }
}
