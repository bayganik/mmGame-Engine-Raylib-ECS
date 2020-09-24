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
    public class PileDispSystem : IExecuteSystem
    {
        Vector2 fanOutDistannce;
        public void Execute()
        {
            var MyScene = (Scene)Global.CurrentScene;
            if (MyScene.GetType().Name != "CardScene")
                return;

            var entities = Context<Default>.AllOf<PileDispComponent>().GetEntities();

            foreach (var entity in entities)
            {
                CardPile cp = entity.GetComponent<CardPile>();
                //
                // 0=Pile on top eachother, 1=right, 2=left, 3=up, 4=down
                //
                switch (cp.FannedDirection)
                {
                    case 0:
                        fanOutDistannce = Vector2.Zero;
                        break;
                    case 1:
                        fanOutDistannce = new Vector2(30f, 0);
                        break;
                    case 2:
                        fanOutDistannce = new Vector2(-30f, 0);
                        break;
                    case 3:
                        fanOutDistannce = new Vector2(0, -30f);
                        break;
                    case 4:
                        fanOutDistannce = new Vector2(0, 30f);
                        break;

                }
                //
                // All cards are Entities in this stack
                //
                int ind = 0;                            //cards number in stack

                for (int i = 0; i < cp.CardsInPile.Count; i++)
                {
                    Entity cardEntity = cp.CardsInPile[i];
                    cardEntity.Get<Transform>().Enabled = true;
                    cardEntity.Get<Transform>().Position = entity.Get<Transform>().Position + fanOutDistannce * new Vector2(ind, ind);

                    ind += 1;
                }
            }
        }
    }
}
