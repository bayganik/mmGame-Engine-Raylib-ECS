using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Numerics;

namespace mmGameEngine
{
	/*
	 * Sprite animation using a sprite sheet with eaqually spaced frames
	 */
	public class SpriteAnimation : RenderComponent 
    {
		/// <summary>
		/// rectangle in the Texture for this element
		/// </summary>
		public Rectangle DestRect;
		/// <summary>
		/// Each rectangle represent an image/sprite
		/// </summary>
		//public Rectangle DestRect;
		public Rectangle[] SourceFrames;

		/// <summary>
		/// the current state of the animation
		/// </summary>
		public AnimationState CurrentState { get; private set; } = AnimationState.None;
		public SpriteAnimationSet CurrentAnimation;
		public bool OriginReCalc = true;
		//
		// Animation sets 
		//
		Dictionary<string, SpriteAnimationSet> Animations = new Dictionary<string, SpriteAnimationSet>();
		//
		// play back
		//
		//int MAX_FRAME_SPEED = 15;
		//int MIN_FRAME_SPEED = 3;
		int currentFrame = 0;
		float timer = 0;

		//int framesCounter = 0;
		//int framesSpeed = 8;
		int FrameWidth = 0;
		int FrameHeight = 0;
		public SpriteAnimation(Texture2D sheetTexture, int cellWidth, int cellHeight, int cellOffset = 0)
		{
			FrameHeight = cellHeight;
			FrameWidth = cellWidth;

			Texture = sheetTexture;
			var cols = sheetTexture.width / cellWidth;
			var rows = sheetTexture.height / cellHeight;
			SourceFrames = new Rectangle[cols * rows];
			//
			// Find source rectangles for each frame on this spritesheet
			// I assume they are uniformly done
			//
			int i = 0;
			for (var y = 0; y < rows; y++)
			{
				for (var x = 0; x < cols; x++)
				{
					SourceFrames[i] = new Rectangle(x * cellWidth + cellOffset, y * cellHeight + cellOffset, cellWidth, cellHeight);
					i++;
				}
			}
		}
		public void AddAnimation(string name = "", string frameNumbers = "", float fps = 3 )
        {
			//
			// default is 3 fps or frames/second (3/60 = 0.05 seconds)
			//
			int numOfAnimFrames;

			//
			// frameNumbers  = "all" or "1,2,3,4"
			//
			if (string.IsNullOrEmpty(name))				//name assumed to be "all"
				name = "all";

			if (string.IsNullOrEmpty(frameNumbers))		//no frame numbers, then "all"
				frameNumbers = "all";

			string[] nums;
			if (frameNumbers.ToLower() == "all")
            {
				// if "all" then fine all frames
				numOfAnimFrames = SourceFrames.Count();
				nums = new string[numOfAnimFrames];
				for (int i = 0; i < numOfAnimFrames; i++)
					nums[i] = i.ToString();
			}
			else
            {
				// if numbers supplied for frames "11,12,13,11"
				nums = Regex.Split(frameNumbers, ",");
				numOfAnimFrames = nums.Count();
            }

			SpriteAnimationSet saSet = new SpriteAnimationSet();
			saSet.FrameRate = fps;
			saSet.SpriteFrames = new Rectangle[numOfAnimFrames]; 
			for (int i = 0; i < nums.Count(); i++)
            {
				int framNum = Convert.ToInt32(nums[i]);
				saSet.SpriteFrames[i] = SourceFrames[framNum];
            }
			Animations.Add(name, saSet);

        }
		public override void Update(float deltaTime)
        {
			base.Update(deltaTime);
			//
			// This component is not attached to Entity yet, cycle out
			//
			if (OwnerEntity == null)
				return;

			if (!Enabled)
				return;

			if (CurrentState != AnimationState.Running)
				return;
			//
			// add delta time (timer = .20 is quarter of a second but 0.05 is slower)
			//
			timer += deltaTime;			//time it takes to render ONE frame 60/1000

			//
			// Change the current frame if number of frames > CurrentAnimation.FrameRate
			//
			//framesCounter++;
			//if (framesCounter > CurrentAnimation.FrameRate)
			//{
			//	currentFrame++;
			//	framesCounter = 0;
			//}
			if (timer >= (CurrentAnimation.FrameRate/60))
            {
				timer = 0;
				currentFrame++;
            }
			//
			// Find out if current frame is more than the animation has
			//
			if (currentFrame >= CurrentAnimation.SpriteFrames.Count())
			{
				currentFrame = 0;
				//
				// If its ONCE only, then stop
				//
				if (!CurrentAnimation.Loop)
                {
					CurrentState = AnimationState.None;
                }				
					
			}
			//
			// Set the origin of the frame one time only
			//
            if (OriginReCalc)
            {
                TextureCenter = new Vector2(FrameWidth * 0.5f * Transform.Scale.X, FrameHeight * 0.5f * Transform.Scale.Y);
				Origin = TextureCenter;
				OriginReCalc = false;
			}
		}
		public override void Render()
		{
            if (OwnerEntity == null)
                return;
			if (!OwnerEntity.IsVisible)
				return;
			if (!Enabled)
                return;

            if (CurrentState != AnimationState.Running)
			{
				Transform.Enabled = false;
				return;
			}

			DestRect = new Rectangle(Transform.Position.X, Transform.Position.Y, 
									 FrameWidth * Transform.Scale.X, 
									 FrameHeight * Transform.Scale.Y);

			Raylib.DrawTexturePro(Texture,
									CurrentAnimation.SpriteFrames[currentFrame],
									DestRect,
									Origin,
									Transform.Rotation,
									Color.WHITE);
        }
		//public void RenderDebug()
		//{
		//	Rectangle rect = new Rectangle(BoxCollider.x, BoxCollider.y, 
		//								   BoxCollider.width, BoxCollider.height);
		//	Vector2 orig = new Vector2(rect.width * 0.25f, rect.height * 0.25f);

		//	Raylib.DrawRectangle((int)BoxCollider.x, (int)BoxCollider.y, (int)BoxCollider.width, (int)BoxCollider.height, Color.RED);
		//	//Raylib.DrawRectanglePro(rect, orig, Transform.Rotation, Color.RED);


		//}
		/// <summary>
		/// Play animation set once (loop = false) or continusly (loop = true)
		/// </summary>
		/// <param name="animName"></param>
		/// <param name="loop"></param>
		public void Play(string animName, bool loop = false)
        {

			if (!Animations.TryGetValue(animName, out CurrentAnimation))
				return;

			CurrentAnimation.Loop = loop;
			CurrentState = AnimationState.Running;
			//Transform.Enabled = true;
		}
		public void Stop()
        {
			CurrentState = AnimationState.None;
			currentFrame = 0;
        }
    }

	public struct SpriteAnimationSet
	{
		public Rectangle[] SpriteFrames;
		public float FrameRate;
		public bool Loop;
	}
	public enum AnimationState
	{
		None,
		Running,
		Paused,
		Completed
	}
}
