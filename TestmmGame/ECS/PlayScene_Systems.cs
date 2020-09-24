using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using Entitas;
using mmGameEngine;
using Transform = mmGameEngine.Transform;
using Raylib_cs;
namespace TestmmGame
{
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
                //if (SceneColliders.CollidedWith(ent, spComp.BoxContainer, out cr))
                //{

                //    Global.AddEntityDestroy(ent);
                //}
                //else
                //    ent.Get<Transform>().Position = pos;
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

                FireComponent tc = e.Get<FireComponent>();
                Transform tr = e.Get<Transform>();
                Vector2 pos = e.Modify<Transform>().Position; // new API for trigger Monitor/ReactiveSystem
                pos.Y += Raylib.GetFrameTime() * tc.Speed;

                CollisionResult cr;
                //BoxCollider bx = e.Get<SpriteAnimation>().BoxCollider;
                //
                // e is entity moving
                // cr.CompEntity.tag is entity we colided with
                //
                if (SceneColliders.CollidedWithBox(e, out cr))
                {
                    if (cr.CompEntity.tag == 1000)          //if cursor then do nothing
                    {
                        e.Get<Transform>().Position = pos;
                        e.Get<Transform>().Enabled = true;
                        return;
                    }
                    //Global.AddEntityToDestroy(e);
                    // cr.CompEntity is the entity we cloided with
                    //
                    e.Get<Transform>().Enabled = false;         //else this code is executed every frame
                    Global.DestroyGameEntity(e);
                    //ActiveScene.ChangeSprite(cr.CompEntity);
                    return;
                }
                //
                // if we leave the world, then destroy the entity
                //
                if ((pos.X >= Global.WorldWidth || pos.X <= 0) || (pos.Y >= Global.WorldHeight || pos.Y <= 0))
                {
                    e.Get<Transform>().Enabled = false;         //else this code is executed every frame
                    Global.DestroyGameEntity(e);
                    return;
                }

                e.Get<Transform>().Position = pos;
                //e.Get<Transform>().Enabled = true;


            }
        }
    }
    public class TurretMovementSystem : IExecuteSystem
    {

        public void Execute()
        {
            var MyScene = (Scene)Global.CurrentScene;
            if (MyScene.GetType().Name != "PlayScene")
                return;

            var ActiveScene = (PlayScene)Global.CurrentScene;
            // new API for getting group with all matched entities from context
            var entities = Context<Default>.AllOf<TurretComponent>().GetEntities();


            foreach (var entity in entities)
            {
                Transform tr = entity.Get<Transform>();

                //if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                //{
                //    tr.LookAt(Raylib.GetMousePosition());
                //}
                float rot = Global.LookAt(Raylib.GetMousePosition(), entity.Get<Transform>().Position);
                entity.Get<Transform>().Rotation = rot;
                //entity.Modify<Transform>().LookAt(Global.GetMousePosition()) ;

                //
                // shot fired
                //
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                {
                    var tComp = entity.Get<TurretComponent>();
                    ActiveScene.FireMissile(Raylib.GetMousePosition(),
                        entity.Get<Transform>().Position,
                    tr.Rotation);
                    //ActiveScene.FireMissile(Raylib.GetMousePosition(),
                    //                        tComp.BulletPlaceHolder.Get<Transform>().Position,
                    //                        tr.Rotation);


                }

            }
        }
    }
    public class TankMovementSystem : IExecuteSystem
    {
        public void Execute()
        {
            var MyScene = (Scene)Global.CurrentScene;
            if (MyScene.GetType().Name != "PlayScene")
                return;

            // new API for getting group with all matched entities from context
            var entities = Context<Default>.AllOf<TankComponent>().GetEntities();

            foreach (var e in entities)
            {
                TankComponent tc = e.Get<TankComponent>();
                //EntityCapturedComponent ECC = e.Get<EntityCapturedComponent>();
                //if (ECC == null)
                //    return;
                //
                // We move tank if it was clicked on
                //
                Transform tr = e.Get<Transform>();
                Vector2 pos = e.Get<Transform>().Position; 
                //
                // Move the tank using keyboard
                //
                if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT))
                {
                    pos.X += Raylib.GetFrameTime() * tc.Speed;
                    //ActiveScene.Camera.offset.X -= Raylib.GetFrameTime() * tc.Speed;
                }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))
                {
                    pos.X -= Raylib.GetFrameTime() * tc.Speed;
                    //ActiveScene.Camera.offset.X += Raylib.GetFrameTime() * tc.Speed;
                }


                if (Raylib.IsKeyDown(KeyboardKey.KEY_UP))
                {
                    pos.Y -= Raylib.GetFrameTime() * tc.Speed;
                    //ActiveScene.Camera.offset.Y += Raylib.GetFrameTime() * tc.Speed;
                }


                if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN))
                {
                    pos.Y += Raylib.GetFrameTime() * tc.Speed;
                    //ActiveScene.Camera.offset.Y -= Raylib.GetFrameTime() * tc.Speed;
                }


                if (Raylib.IsKeyDown(KeyboardKey.KEY_Q))
                    e.Get<Transform>().Rotation -= .05f;

                if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
                    e.Get<Transform>().Rotation += .05f;
                //
                // Test to make sure we don't leave the world
                //
                if ((pos.X >= Global.WorldWidth || pos.X <= 0) || (pos.Y >= Global.WorldHeight || pos.Y <= 0))
                    pos = e.Get<Transform>().Position;
                else
                    e.Get<Transform>().Position = pos;
            }
        }
    }
}
