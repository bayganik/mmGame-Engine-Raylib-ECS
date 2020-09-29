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

                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    CollisionResult cr;
                    if (SceneColliderDatabase.CollidedWithBox(e, out cr))
                    {
                        e.Get<CrossHairComponent>().CurrentEntityChosen = cr.CompEntity;
                        cr.CompEntity.Add<EntityCapturedComponent>();
                    }
                }
            }
        }
    }
}
