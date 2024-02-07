using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using Entitas;
using mmGameEngine;
using Transform = mmGameEngine.TransformComponent;
using Raylib_cs;

namespace TestmmGame
{
    public class CrossHairMoveSystem : IExecuteSystem
    {
        /*
         * All active scenes use the Cross Hair cursor
         */
        public void Execute()
        {
            var ActiveScene = (Scene)Global.CurrentScene;

            var entities = Context<Default>.AllOf<CrossHairComponent>().GetEntities();

            foreach (var e in entities)
            {

                e.Get<Transform>().Position = Global.GetMousePosition();

                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    CollisionResult cr;
                    if (SceneColliderManager.CollidedWithBox(e, out cr))
                    {
                        e.Get<CrossHairComponent>().CurrentEntityChosen = cr.OwnerEntity;
                        cr.OwnerEntity.Add<EntityCapturedComponent>();
                    }
                }
            }
        }
    }
    public class CollisionDetectSystem : IExecuteSystem
    {
        public void Execute()
        {
            var MyScene = (Scene)Global.CurrentScene;
            if (MyScene.GetType().Name != "PlayScene")
                return;

            var entities = Context<Default>.AllOf<TankComponent>().GetEntities();

            foreach (var ent in entities)
            {
                //var spComp = ent.Get<SpriteAnimation>();
                //Vector2 pos = ent.Modify<Transform>().Position; // new API for trigger Monitor/ReactiveSystem

                //pos.Y += .05f;
                //CollisionResult cr;
                //if (SceneColliderDatabase.CollidedWith(ent, spComp.BoxContainer, out cr))
                //{

                //    Global.AddEntityDestroy(ent);
                //}
                //else
                //    ent.Get<Transform>().Position = pos;
            }
        }
    }
    public class BulletMoveSystem : IExecuteSystem
    {
        public void Execute()
        {
            var MyScene = (PlayScene)Global.CurrentScene;
            if (MyScene.GetType().Name != "PlayScene")
                return;

            var entities = Context<Default>.AllOf<EntityMover>().GetEntities();

            foreach (var ent in entities)
            {
                if (!ent.Get<EntityMover>().Enabled)
                    continue;
                if (ent.Get<EntityMover>().IsMoving)
                    continue;
                //
                // We are Enabled & stopped moving
                //
                CollisionResult cres = ent.Get<EntityMover>().MoveCollisionResult;
                //
                // stopped, Did we hit something?
                //
                if (cres.Collided)
                {
                    MyScene.PlayExplosion(cres.OwnerEntity.Get<Transform>().Position);
                    if (cres.OwnerEntity.Tag != 1000)
                        Global.DestroyGameEntity(cres.OwnerEntity);
                    //cres.OwnerEntity.Get<Transform>().Enabled= false;

                    ent.Get<EntityMover>().Enabled = false;
                    ent.Get<Transform>().Enabled = false;
                    ent.IsVisible = false;
                }
                else
                {
                    //
                    //stopped but didn't hit anything
                    //
                    ent.Get<EntityMover>().Enabled = false;               
                    ent.IsVisible = false;              
                }
            }
        }
    }
    public class FireMoveSystem : IExecuteSystem
    {
        public void Execute()
        {
            var MyScene = (Scene)Global.CurrentScene;
            if (MyScene.GetType().Name != "PlayScene")
                return;

            // new API for getting group with all matched entities from context
            var entities = Context<Default>.AllOf<FireComponent>().GetEntities();

            foreach (var e in entities)
            {
                if (!e.Get<Transform>().Enabled)
                    continue;
                //
                // if cursor is on top of fire animation, then move it vertically
                //
                FireComponent tc = e.Get<FireComponent>();
                Transform tr = e.Get<Transform>();
                Vector2 pos = e.Modify<Transform>().Position; // new API for trigger Monitor/ReactiveSystem
                pos.Y += Raylib.GetFrameTime() * tc.Speed;


                CollisionResult cr;

                if (SceneColliderManager.CollidedWithBox(e, out cr))
                {
                    if (cr.OwnerEntity.Tag == 1000)          //cursor has collided with fire
                    {
                        e.Get<Transform>().Position = pos;
                        e.Get<Transform>().Enabled = true;
                        return;
                    }
                    //Global.AddEntityToDestroy(e);
                    // cr.OwnerEntity is the entity we cloided with
                    //
                    e.Get<Transform>().Enabled = false;         //else this code is executed
                    Global.DestroyGameEntity(e);
                    //ActiveScene.ChangeSprite(cr.OwnerEntity);
                    return;
                }
                //
                // if we left the world, then destroy the entity
                //
                if ((pos.X >= Global.WorldWidth || pos.X <= 0) || (pos.Y >= Global.WorldHeight || pos.Y <= 0))
                {
                    e.Get<Transform>().Enabled = false;         //else this code is executed every frame
                    Global.DestroyGameEntity(e);
                    return;
                }

                //e.Get<Transform>().Position = pos;



            }
        }
    }
    public class TurretMovementSystem : IExecuteSystem
    {
        PlayScene ActiveScene;
        public void Execute()
        {
            
            //
            // Make sure you are in the correct scene!
            //
            //var MyScene = (Scene)Global.CurrentScene;
            //if (MyScene.GetType().Name != "PlayScene")
            //    return;

            ActiveScene = (PlayScene)Global.CurrentScene;
            //
            // Get group with all matched entities from context
            //
            var entities = Context<Default>.AllOf<TurretComponent>().GetEntities();


            foreach (var entity in entities)
            {
                Transform tr = entity.Get<Transform>();

                float rot = Global.LookAt(Global.GetMousePosition(), tr.Position);
                tr.Rotation = rot;
                //
                // shot fired
                //
                if (Raylib.IsKeyPressed(KeyboardKey.Space))
                {
                    //var tComp = entity.Get<TurretComponent>();
                    //var _pos = tComp.BulletPlaceHolder.Get<Transform>().Position;
                    ActiveScene.FireMissile(Global.GetMousePosition(),
                                           tr.Position,
                                           tr.Rotation);
                }

            }
        }
    }
    public class TankMovementSystem : IExecuteSystem
    {
        public void Execute()
        {
            //var MyScene = (Scene)Global.CurrentScene;
            //if (MyScene.GetType().Name != "PlayScene")
            //    return;
            //
            // Get group with all matched entities from context
            //
            var entities = Context<Default>.AllOf<TankComponent>().GetEntities();

            foreach (var e in entities)
            {
                TankComponent tc = e.Get<TankComponent>();
                InputController inpCnt = e.Get<InputController>();
                //
                // We move tank if it was clicked on
                //
                Transform tr = e.Get<Transform>();
                Vector2 pos = e.Get<Transform>().Position; 
                //
                // Input controller gives us direction to go
                //
                if(inpCnt.Direction != Vector2.Zero)
                {
                    //if (e.Get<Transform>().Rotation != 0)
                    //    inpCnt.Direction = (inpCnt.Direction * e.Get<Transform>().Rotation);
                    pos += inpCnt.Direction * Raylib.GetFrameTime() * tc.Speed;
                    
                }

                if (Raylib.IsKeyDown(KeyboardKey.Q))
                    e.Get<Transform>().Rotation -= .05f;

                if (Raylib.IsKeyDown(KeyboardKey.E))
                    e.Get<Transform>().Rotation += .05f;
                //
                // Test to make sure we don't leave the world
                //
                if (Global.EntityOutOfBound(pos))
                    pos = e.Get<Transform>().Position;
                else
                    e.Get<Transform>().Position = pos;
            }
        }
    }
}
