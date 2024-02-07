using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

using Entitas;
using mmGameEngine;
using Transform = mmGameEngine.TransformComponent;
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
                CardPileComponent cp = entity.GetComponent<CardPileComponent>();
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

                    cardEntity.Get<Card>().RenderLayer = 100 + ind;
                    ind += 1;
                }
            }
        }
    }
    public class GameStateSystem : IExecuteSystem
    {
        public void Execute()
        {
            var MyScene = (Scene)Global.CurrentScene;
            if (MyScene.GetType().Name != "CardScene")
                return;

            var entities = Context<Default>.AllOf<GameStatComponent>().GetEntities();

            foreach (var entity in entities)
            {
                GameStatComponent sc = entity.Get<GameStatComponent>();
                Text txt = entity.Get<Text>();
                txt.Content = sc.ScoreWords + sc.Score.ToString();


                if (Global.StateOfGame == GameState.Over)
                {
                    MsgBox msb = entity.Get<MsgBox>();
                    msb.RenderLayer = 10000;

                    Label lbl = new Label("Press Play to continue the game.");
                    msb.AddMsg(lbl, new Vector2(15, 5));
                    lbl = new Label("Score : " + sc.Score.ToString("###"));
                    msb.AddMsg(lbl, new Vector2(15, 30));
                    //Button btn = new Button( 50, 25, "OK");
                    //msb.AddButton(btn, new Vector2(14, 50));
                    entity.IsVisible = true;
                }
            }
        }
    }
    public class MouseClickSystem : IExecuteSystem
    {
        bool Dragging = false;
        float timerDelay = 500;
        float clickTimer;
        CardScene MyScene;
        public void Execute()
        {
            var tmpScene = (Scene)Global.CurrentScene;
            if (tmpScene.GetType().Name != "CardScene")
                return;

            MyScene = (CardScene)tmpScene;
            var entities = Context<Default>.AllOf<MouseComponent>().GetEntities();

            foreach (var mouseEntity in entities)
            {
                mouseEntity.Get<Transform>().Position = Global.GetMousePosition();
                clickTimer += Global.DeltaTime * 1000;
                //if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                //{
                //    if (clickTimer < timerDelay)
                //    {
                //        CheckCollisionResults(mouseEntity);
                //        clickTimer = 0;
                //    }
                //}
                if (Raylib.IsMouseButtonReleased(MouseButton.Left))
                {
                    if (clickTimer < timerDelay)
                    {
                        clickTimer = 0;             //double click detected
                    }
                    clickTimer = 0;
                    if (Dragging)
                    {
                        Dragging = false;

                        CollisionResult cr;
                        if (!SceneColliderManager.CollidedWithBox(mouseEntity, out cr))
                            return;

                        Entity collidedEntity = cr.OwnerEntity;
                        //
                        // Dealt Card is released but was not put on a stack
                        //
                        if (collidedEntity.Tag == 80)
                        {
                            //
                            // Ace pile drop
                            //
                            MyScene.DropCardFromDrag2AceStack(collidedEntity);
                            return;                     //ace pile stack
                        }
                        if ((collidedEntity.Tag >= 1) && (collidedEntity.Tag <= 7))
                        {
                            //
                            // Play pile drop
                            //
                            MyScene.DropCardFromDrag2PlayStack(collidedEntity);
                            return;
                        }
                        //
                        // mouse released but not on Ace or Play area, return card to its place
                        //
                        MyScene.ReturnCardFromDrag2Stack();
                        return;                     //drap disp stack (release of mouse outside of play area)
                    }


                }
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    //
                    // if game over, don't allow movement
                    //
                    if (Global.StateOfGame == GameState.Over)
                        return;

                    CheckCollisionResults(mouseEntity);
                   
                }
            }
        }
        /*
         *
         * Pile with tag = 90 dealt pile displayed
         * Pile with tag = 80 are Ace piles
         * Pile with tag = 70 face down cards to deal
         * Pile with tags 1 - 7 are play stacks
         *       
         */
        private void CheckCollisionResults(Entity entity)
        {
            CollisionResult cr;
            if (!SceneColliderManager.CollidedWithBox(entity, out cr))
                return;
            //-----------------------------------------
            // we have clicked on a box collider
            //-----------------------------------------
            entity.Get<MouseComponent>().CurrentEntityClicked = cr.OwnerEntity;

            Dragging = false;
            switch (cr.OwnerEntity.Tag)
            {
                case 80:
                    MyScene.AcePileCard2Drag(cr.OwnerEntity);
                    Dragging = true;
                    break;
                case 70:
                    MyScene.DealtCard2Drag(cr.OwnerEntity);
                    Dragging = true;
                    break;
                case 90:
                    MyScene.DealCard2Disp(cr.OwnerEntity);           //from face down deal to face up
                    break;
                case int n when (n >=1 && n <= 7):
                    Entity cardEntity = MyScene.FindCardInPlayPile(cr.OwnerEntity, Global.GetMousePosition());
                    if (cardEntity == null)
                        return;
                    //
                    // We have card, drag it + all others under it
                    //
                    Dragging = true;
                    MyScene.TakeCards2Drag(cardEntity);
                    break;
            }
        }
    }
    public class DragDispPileSystem : IExecuteSystem
    {
        CardScene MyScene;
        Vector2 fanOutDistannce;
        Vector2 PrevMouse;
        Vector2 CurrentMouse = Vector2.Zero;
        public void Execute()
        {
            var tmpScene = (Scene)Global.CurrentScene;
            if (tmpScene.GetType().Name != "CardScene")
                return;

            MyScene = (CardScene)tmpScene;
            var entities = Context<Default>.AllOf<DragComponent>().GetEntities();

            foreach (var entity in entities)
            {
                //
                // We have a DragDisp entity (holds all cards entities)
                //
                CardPileComponent sc = entity.GetComponent<CardPileComponent>();
                if (sc != null)
                {
                    if (sc.CardsInPile.Count <= 0)
                        return;                         //no cards to drag
                }

                var _mouseCollider = entity.GetComponent<BoxCollider>();
                PrevMouse = CurrentMouse;
                CurrentMouse = Global.GetMousePosition() ;
                //
                // Current location of the mouse used for the hand icon
                //
                entity.Get<Transform>().Position = CurrentMouse;

                Entity lastCardonStack = sc.CardsInPile.LastOrDefault();
                //
                // Display of stack by fan out direction
                //
                switch (sc.FannedDirection)
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
                int ind = 0;                            //cars number in stack
                for (int i = 0; i < sc.CardsInPile.Count; i++)
                {
                    Entity cardEntity = sc.CardsInPile[i];
                    cardEntity.Get<Transform>().Enabled = true;
                    cardEntity.Get<Transform>().Position = entity.Get<Transform>().Position + fanOutDistannce * ind;
                    //
                    // Get the sprite (face/back)
                    //
                    //cardEntity.GetComponent<Card>().RenderLayer = (ind * 1000);
                    //cardEntity.GetComponent<Card>().RenderLayer = Global.CURSOR_LAYER - ind;
                    cardEntity.GetComponent<Card>().RenderLayer = Global.CURSOR_LAYER - ind;
                    ind += 1;
                }
            }
        }

    }
}
