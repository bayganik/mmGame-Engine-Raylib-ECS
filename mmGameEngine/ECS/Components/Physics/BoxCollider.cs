using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;

namespace mmGameEngine
{
    public class BoxCollider : RenderComponent
    {
		public List<Vector2> BoxPoints;
		public BoxAABB CollisionBox;

		Rectangle boxContainer;

		bool setSceneColliders;
		bool fixedCollider;
		/// <summary>
		/// Box to check for collisions. Box goes around the image (using scales)
		/// </summary>
		/// <param name="boxToCollide"></param>
		//public BoxCollider(Rectangle _boxToCollide)
  //      {
		//	theCenter = new Vector2(_boxToCollide.width * 0.5f, _boxToCollide.height * 0.5f);
		//	boxContainer = new Rectangle(_boxToCollide.x - theCenter.X,
		//								 _boxToCollide.y - theCenter.Y,
		//								 _boxToCollide.width,
		//								 _boxToCollide.height);


		//	CollisionBox = new BoxAABB();
		//	setSceneColliders = false;
		//	RenderLayer = Global.BOXCOLLIDER_LAYER;				//make sure this is drawn first
		//}
		public BoxCollider(float _width, float _height)
        {
			Rectangle _boxToCollide = new Rectangle(0,0, _width, _height);
			OriginLocal = new Vector2(_boxToCollide.width * 0.5f, _boxToCollide.height * 0.5f);

			boxContainer = new Rectangle(_boxToCollide.x - OriginLocal.X,
										 _boxToCollide.y - OriginLocal.Y,
										 _boxToCollide.width,
										 _boxToCollide.height);


			CollisionBox = new BoxAABB();
			RenderLayer = Global.BOXCOLLIDER_LAYER;             //make sure this is drawn first
			fixedCollider = false;
		}
		public BoxCollider(float _x, float _y, float _width, float _height)
		{
			//
			// Special BoxCollider that is stationary and will not move
			// likke a building or card pile
			//
			Rectangle _boxToCollide = new Rectangle(_x , _y , _width, _height);
			OriginLocal = new Vector2(_x/2 , _y/2);
			boxContainer = new Rectangle(_boxToCollide.x,
										 _boxToCollide.y,
										 _boxToCollide.width,
										 _boxToCollide.height);


			CollisionBox = new BoxAABB();
            boxContainer.x = (_boxToCollide.x - OriginLocal.X) + 10;
            boxContainer.y = (_boxToCollide.y - OriginLocal.Y / 2) + 10;
            BoxPoints = new List<Vector2>();
			Vector2 topL = new Vector2(boxContainer.x, boxContainer.y);
			Vector2 topR = new Vector2(topL.X + boxContainer.width, topL.Y);
			Vector2 botL = new Vector2(topL.X, topL.Y + boxContainer.height);
			Vector2 botR = new Vector2(topR.X, topR.Y + boxContainer.height);
			BoxPoints.Add(topL);
			BoxPoints.Add(botL);
			BoxPoints.Add(botR);
			BoxPoints.Add(topR);
			//
			// Find the min & max vectors for collision
			//
			CollisionBox.Fit(BoxPoints);                //updates the position of this collider
			RenderLayer = Global.BOXCOLLIDER_LAYER;             //make sure this is drawn first
			fixedCollider = true;
		}
		public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
			//
			// Has Entity been assigned yet?
			//
			if (CompEntity == null)
				return;
			if (!Enabled)
				return;
			//
			// update location of box containing the collider
			//
			if (!fixedCollider)
			{
				boxContainer.x = Transform.Position.X - OriginLocal.X;
				boxContainer.y = Transform.Position.Y - OriginLocal.Y;
				BoxPoints = new List<Vector2>();
				Vector2 topL = new Vector2(boxContainer.x, boxContainer.y);
				Vector2 topR = new Vector2(topL.X + boxContainer.width, topL.Y);
				Vector2 botL = new Vector2(topL.X, topL.Y + boxContainer.height);
				Vector2 botR = new Vector2(topR.X, topR.Y + boxContainer.height);
				BoxPoints.Add(topL);
				BoxPoints.Add(botL);
				BoxPoints.Add(botR);
				BoxPoints.Add(topR);
				//
				// Find the min & max vectors for collision
				//
				CollisionBox.Fit(BoxPoints);                //updates the position of this collider
			}
			if (!setSceneColliders)
			{
				//
				// update the database of colliders in this scene (happens only once)
				//
				SceneColliderDatabase.SetCollider(CompEntity, CollidreShape.Box);
				setSceneColliders = true;
			}
		}
        public override void Render()
		{
			if (CompEntity == null)
				return;
			if (!Enabled)
				return;

			if (Global.DebugRenderEnabled)
                RenderDebug();
        }
		public void RenderDebug()
		{
			//Raylib.DrawLine((int)CollisionBox.min.X, (int)CollisionBox.min.Y, (int)CollisionBox.min.X, (int)CollisionBox.min.Y + (int)CollisionBox.max.Y, Color.RED);
			//Raylib.DrawLine((int)CollisionBox.min.X, (int)CollisionBox.min.Y + (int)CollisionBox.max.Y, (int)CollisionBox.min.X + (int)CollisionBox.max.X, (int)CollisionBox.min.Y + (int)CollisionBox.max.Y, Color.RED);
			//Raylib.DrawLine((int)CollisionBox.min.X + (int)CollisionBox.max.X, (int)CollisionBox.min.Y + (int)CollisionBox.max.Y, (int)CollisionBox.min.X + (int)CollisionBox.max.X, (int)CollisionBox.min.Y, Color.RED);
			//Raylib.DrawLine((int)CollisionBox.min.X + (int)CollisionBox.max.X, (int)CollisionBox.min.Y, (int)CollisionBox.min.X, (int)CollisionBox.min.Y, Color.RED);

			//
			// draw full rectangle
			//
			//Rectangle rt = new Rectangle(CollisionBox.min.X, CollisionBox.min.Y, boxContainer.width, boxContainer.height);
			Rectangle rt = new Rectangle(boxContainer.x, boxContainer.y, boxContainer.width, boxContainer.height);
			//Raylib.DrawRectangle((int)rt.x, (int)rt.y, 
			//					 (int)rt.width, (int)rt.height,
			//					 Raylib.Fade(Color.RED, 0.5f));
			Raylib.DrawRectangleLines((int)rt.x, (int)rt.y,
								      (int)rt.width, (int)rt.height,
								       Color.YELLOW);
			//Raylib.DrawCircle((int)CollisionBox.min.X, (int)CollisionBox.min.X, 5, Color.GRAY);
			//Raylib.DrawCircle(Convert.ToInt32(CollisionBox.max.X), Convert.ToInt32(CollisionBox.max.Y), 5, Color.BLACK);
		}
	}
}
