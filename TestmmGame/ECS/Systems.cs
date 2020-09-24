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

    // if no feature-set declaration, it comes into UnnamedFeature
    public class TextMovementSystem : IExecuteSystem
    {
        public void Execute()
        {
            // new API for getting group with all matched entities from context
            var entities = Context<Default>.AllOf<Text>().GetEntities();

            foreach (var e in entities)
            {
                Text txt = e.Get<Text>();
                //txt.FontSize = 50;
                //txt.Content = "I changed, do you like?";
                //Vector2 pos = e.Modify<Transform>().Position; // new API for trigger Monitor/ReactiveSystem

                //pos.X += .05f;

                //e.Get<Transform>().Position = pos;
            }
        }
    }

    //
    // Sample view just display Entity's Position if changed
    // if Transform of any entity has changed, then this will execute
    //
    public class ViewSystem : ReactiveSystem
    {
        public ViewSystem()
        {
            // 
            // Register the Transform component with changes
            //
            Monitors += Context<Default>.AllOf<SpriteAnimation>().OnAdded(Process);
        }
        //
        // This get caught during update (DO NOT DRAY ANYTHING HERE)
        //
        protected void Process(List<Entity> entities)
        {
            foreach (var e in entities)
            {
                Vector2 pos = e.Get<Transform>().Position;
                //Raylib.DrawText(
                //	"Entity" + e.creationIndex + ": x=" + pos.X + " y=" + pos.Y, 300, 400, 13, Color.BLACK);
            }
        }
    }
}
