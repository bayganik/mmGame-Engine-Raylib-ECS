﻿using System;
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

		public Vector2 MoveFrom;						//From/To are place holders they do not work here
		public Vector2 MoveTo;
		public bool IsMoving;							//entity still is moving (true/false)
		public CollisionResult MoveCollisionResult;		//if we hit any other collider
		//
		// rotate entity during movement if false, no rotation e.g. arrows are pressed
		//
		public bool DoesRotate = false;             //rotating like a wheel
		public float RotationSpeed = 15f;            //how fast rotate

		public EntityMover()
		{
			IsMoving = true;
			Speed = 20f;
			DoesRotate = false;
			RotationSpeed = 15f;

		}
		public EntityMover(Vector2 _from, Vector2 _to, float _speed = 200f)
		{
			MoveFrom = _from;
			MoveTo = _to;
			IsMoving = true;
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
			if (CompEntity == null)
				return;
			if (!Enabled)
				return;
			//
			// Moving from - to on its own
			//
			if (!this.IsMoving)
				return;                     //not moving anymore (did we hit a collider or reached the end)

			Vector2 start = this.MoveFrom;
			Vector2 end = this.MoveTo;

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
				return ;
			}
			//
			// set a new starting location for the next update cycle
			//
			this.MoveFrom = Transform.Position;
			//
			// If we hit anything, then no entity does not move
			// Its up to the caller to investigate MoveCollisionResult
			//
			this.IsMoving = Move(moveDir);
			//if (Move(moveDir))
			//{
			//	MoveCollisionResult = new CollisionResult();
			//	this.IsMoving = true;
			//}
			//else
			//	this.IsMoving = false;
		}
		private bool Move(Vector2 motion)
        {
			
			Transform.Position += motion * this.Speed * Global.DeltaTime;

			if (SceneColliderDatabase.CollidedWithBox(CompEntity, out MoveCollisionResult))
			{
				if (CompEntity.isEnemy == MoveCollisionResult.CompEntity.isEnemy)
					return true;

				return false;
			}
			return true;
		}
    }
}