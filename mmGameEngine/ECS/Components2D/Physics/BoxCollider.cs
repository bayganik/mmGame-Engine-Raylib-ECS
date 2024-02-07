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
		//	theCenter = new Vector2(_boxToCollide.Width * 0.5f, _boxToCollide.Height * 0.5f);
		//	boxContainer = new Rectangle(_boxToCollide.X - theCenter.X,
		//								 _boxToCollide.Y - theCenter.Y,
		//								 _boxToCollide.Width,
		//								 _boxToCollide.Height);


		//	CollisionBox = new BoxAABB();
		//	setSceneColliders = false;
		//	RenderLayer = Global.BOXCOLLIDER_LAYER;				//make sure this is drawn first
		//}
		public BoxCollider(float _width, float _height)
        {
			Rectangle _boxToCollide = new Rectangle(0,0, _width, _height);
			Origin = new Vector2(_boxToCollide.Width * 0.5f, _boxToCollide.Height * 0.5f);

			boxContainer = new Rectangle(_boxToCollide.X - Origin.X,
										 _boxToCollide.Y - Origin.Y,
										 _boxToCollide.Width,
										 _boxToCollide.Height);


			CollisionBox = new BoxAABB();
			RenderLayer = Global.BOXCOLLIDER_LAYER;             //make sure this is drawn first
			fixedCollider = false;
		}
		/// <summary>
		/// Special BoxCollider that is stationary and will not move e.g. a building or card pile
		/// </summary>
		/// <param name="_x"></param>
		/// <param name="_y"></param>
		/// <param name="_width"></param>
		/// <param name="_height"></param>
		public BoxCollider(float _x, float _y, float _width, float _height)
		{
			Rectangle _boxToCollide = new Rectangle(_x , _y , _width, _height);
			Origin = new Vector2(_x/2 , _y/2);
			boxContainer = new Rectangle(_boxToCollide.X,
										 _boxToCollide.Y,
										 _boxToCollide.Width,
										 _boxToCollide.Height);


			CollisionBox = new BoxAABB();
            boxContainer.X = (_boxToCollide.X - Origin.X) + 10;
            boxContainer.Y = (_boxToCollide.Y - Origin.Y / 2) + 10;
            BoxPoints = new List<Vector2>();
			Vector2 topL = new Vector2(boxContainer.X, boxContainer.Y);
			Vector2 topR = new Vector2(topL.X + boxContainer.Width, topL.Y);
			Vector2 botL = new Vector2(topL.X, topL.Y + boxContainer.Height);
			Vector2 botR = new Vector2(topR.X, topR.Y + boxContainer.Height);
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
			if (OwnerEntity == null)
				return;
			if (!Enabled)
				return;

            TransformComponent Transform = OwnerEntity.Get<TransformComponent>();
            //
            // update location of box containing the collider
            //
            if (!fixedCollider)
			{
				boxContainer.X = Transform.Position.X - Origin.X;
				boxContainer.Y = Transform.Position.Y - Origin.Y;
				BoxPoints = new List<Vector2>();
				Vector2 topL = new Vector2(boxContainer.X, boxContainer.Y);
				Vector2 topR = new Vector2(topL.X + boxContainer.Width, topL.Y);
				Vector2 botL = new Vector2(topL.X, topL.Y + boxContainer.Height);
				Vector2 botR = new Vector2(topR.X, topR.Y + boxContainer.Height);
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
				SceneColliderManager.SetCollider(OwnerEntity, CollidreShape.Box);
				setSceneColliders = true;
			}
		}
        public override void Render()
		{
			//if (OwnerEntity == null)
			//	return;
			//if (!OwnerEntity.IsVisible)
			//	return;
			//if (!Enabled)
			//	return;

			if (Global.DebugRenderEnabled)
                RenderDebug();
        }
		public void RenderDebug()
		{
			//
			// draw full rectangle
			//
			Rectangle rt = new Rectangle(boxContainer.X, boxContainer.Y, boxContainer.Width, boxContainer.Height);
			Raylib.DrawRectangleLines((int)rt.X, (int)rt.Y,
								      (int)rt.Width, (int)rt.Height,
								       Color.Red);
			//Raylib.DrawCircle((int)CollisionBox.min.X, (int)CollisionBox.min.X, 5, Color.Gray);
			//Raylib.DrawCircle(Convert.ToInt32(CollisionBox.max.X), Convert.ToInt32(CollisionBox.max.Y), 5, Color.Black);
		}
	}
}
