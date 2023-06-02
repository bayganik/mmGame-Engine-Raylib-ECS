using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;
using System.Net.NetworkInformation;


namespace mmGameEngine
{
    public class EntityMover : Component
    {
		//
		// Following items are data to be used by calling processing system
		//      they are NOT used by this component automatically
		//
		public float Speed = 200f;

		public Vector2 MoveStart;						//From/To are place holders they do not work here
		public Vector2 MoveEnd;
		public bool IsMoving;                           //entity still is moving (true/false)
		public bool IsAtEnd;					//didn't hit anything but reached MoveEnd

		public CollisionResult MoveCollisionResult;		//if we hit any other collider
		//
		// rotate entity during movement if false, no rotation e.g. arrows are pressed
		//
		public bool DoesRotate = false;             //rotating like a wheel
		public float RotationSpeed = 15f;            //how fast rotate

		public EntityMover()
		{
			IsMoving = true;
			Speed = 200f;
			DoesRotate = false;
			RotationSpeed = 200f;

		}
		public EntityMover(Vector2 _start, Vector2 _end, float _speed = 200f)
		{
			MoveStart = _start;
			MoveEnd = _end;
			IsMoving = true;
			IsAtEnd = false;
			Speed = _speed;
			DoesRotate = false;
			RotationSpeed = 15f;
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

			if (!this.IsMoving)
				return;                     //not moving anymore (did we hit a collider or reached the end)
			//
			// Moving from start - to end on its own
			//
			Vector2 start = this.MoveStart;
			Vector2 end = this.MoveEnd;

			float distance = Vector2.Distance(start, end);
			Vector2 moveDir = Vector2.Normalize(end - start);
			//
			// put entity at the start location, move him according to his speed and direction
			//
			Transform.Position = start;
			Transform.Position += moveDir * this.Speed * Global.DeltaTime;

			if (DoesRotate)
				Transform.Rotation += (RotationSpeed * Global.DeltaTime) * 1;
			//
			// has Enity reached the end position
			//
			if (Vector2.Distance(start, Transform.Position) >= distance)
			{
				Transform.Position = end;
				this.IsMoving = false;
				this.IsAtEnd = true;
				return ;
			}
			//
			// set a new starting location for the next update cycle
			//
			this.MoveStart = Transform.Position;
			//
			// If we hit anything, then entity does not move
			// Its up to the caller to investigate MoveCollisionResult
			//
			Move(moveDir);
		}
		private void Move(Vector2 motion)
        {
			MoveCollisionResult = new CollisionResult();
			Transform.Position += motion * this.Speed * Global.DeltaTime;

			if (SceneColliderManager.CollidedWithBox(OwnerEntity, out MoveCollisionResult))
			{
				//
				// continue moving if both entities that hit eachother
				// are either enemies or friends
				//
				if (OwnerEntity.Tag == MoveCollisionResult.OwnerEntity.Tag)
					return;


    //            if (OwnerEntity.IsEnemy && MoveCollisionResult.OwnerEntity.IsEnemy)
				//	return;             //both enemy
				//if (!OwnerEntity.IsEnemy && !MoveCollisionResult.OwnerEntity.IsEnemy)
				//	return;             //both friends

				this.IsMoving = false;          // stop moving(enemy hit a friendly OR friendly hit an enemy
			}
			return;
		}
    }
}
