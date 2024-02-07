using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using Raylib_cs;
using mmGameEngine;

using Transform = mmGameEngine.TransformComponent;
using Entitas;

namespace TestmmGame
{
    /*
     * Mouse double click will take the first face up card in Play Pile and tries to find a proper 
     *  Ace pile to deliver it
     */
    public class CardScene : Scene
    {
        //
        // location of the piles on screen
        //
        int cardBackNum = 6;
        Vector2 drawStack = new Vector2(90, 100);
        Vector2 aceStack1 = new Vector2(390, 100);
        Vector2 drawStackDisp = new Vector2(190, 100);
        Vector2 playStack1 = new Vector2(90, 250);
        //
        // play pile collider size
        //
        float colliderHeight = 500f;
        float colliderWidth = 72f;
        float colliderX = -36f;
        float colliderY = -50f;
        int possibleScoreValue = 0;
        //
        // collection of card piles
        //
        List<Entity> PlayStacks = new List<Entity>();
        List<Entity> AceStacks = new List<Entity>();

        CardDeckManager CardDeck;
        Entity tmpEnt;

        Entity PlayBtn;
        Entity DealDeck;
        Entity DrawDisp;
        Entity DragDisp;
        Entity GameStatus;

        Entity entMap;              //TmxMap
        Entity msgEnt;              //msg box to inform
        Entity entPanel;
        Entity entUI;
        Texture2D textureImage = new Texture2D();
        Sprite Spr;
        int dispY;
        int dispX;
        Vector2 position;
        public CardScene() 
        {
            Global.SceneHeight = 800;
            Global.SceneWidth = 960;
            Global.SceneTitle = "Card Scene";
            Global.SceneClearColor = new Color(255, 255, 255, 255);
            Global.SceneClearColor = new Color(0, 117, 44, 255);

        }
        public override void Initialize()
        {
            Content.LoadMenu();
            //
            // we will show an index finger hand
            //
            Global.HideCursor = true;
            Global.CurrentScene = this;
            Global.DebugRenderEnabled = true;
        }
        public override void Play()
        {

            dispY = (int)Global.WindowCenter.Y - 100;
            dispX = 40;

            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            // Hand cursor mouse + boxcollider
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            PrefabEntity.CreateCursorEntity("Assets/Img/hand.png");

            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            // Game status (Score + Game over)
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            GameStatus = Global.CreateGameEntity(Global.WindowCenter - new Vector2(150, 0));
            GameStatus.Name = "Game Status";
            Text scoreTxt = new Text("Score: ", TextFontTypes.Arial);
            GameStatus.Add(scoreTxt);
            MsgBox mb = new MsgBox( 350, 150, Color.Brown);
            //mb.Visiable = false;
            GameStatus.Add(mb);
            GameStatus.Add<GameStatComponent>();

            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            // Create a deck of cards
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            CardDeck = new CardDeckManager(this);
            CardDeck.CardsHaveCollider = false;
            CardDeck.CreateDeckOfCards(false);

            CreateDealPile();                           // deal stack (falce down)
            CreateDrawPile();                           // face up dealt one card at a time
            CreateEmptyAcePiles();                      // 4 of them
            CreateEmptyPlayPiles();                     // 7 of them
            CreateEmptyDragPile();                      // used to disp cards being dragged around
            DealCardsToPiles();                         // draw cards & place in piles above
            //
            // Play Button (game entity so the cursor can hover over it)
            //
            Global.StateOfGame = GameState.Over;
            PlayBtn = Global.CreateGameEntity(new Vector2(850, 50));
            PlayBtn.Name = "playBtn";
            position = PlayBtn.Get<Transform>().Position;
            Button bt = new Button(75, 30, "Play", -6, 4);
            PlayBtn.Add(bt);
            bt.Click += PlayButton;
            //
            // End Button
            //
            Global.StateOfGame = GameState.Over;
            entUI = Global.CreateGameEntity(new Vector2(850, 100));
            entUI.Name = "endBtn";
            position = entUI.Get<Transform>().Position;
            bt = new Button(75, 30, "End", -6, 4);
            entUI.Add(bt);
            bt.Click += EndButton;
            //
            // Menu Button
            //
            Global.StateOfGame = GameState.Over;
            entUI = Global.CreateGameEntity(new Vector2(850, 150));
            entUI.Name = "menuBtn";
            position = entUI.Get<Transform>().Position;
            bt = new Button(75, 30, "Menu", -6, 4);
            entUI.Add(bt);
            bt.Click += MenuButton;
            //
            //znznznznznznznznznznznznznzn
            //   Scene Starts Playing
            //znznznznznznznznznznznznznzn

            AddSystem(new PileDispSystem());
            AddSystem(new MouseClickSystem());
            AddSystem(new DragDispPileSystem());
            AddSystem(new GameStateSystem());
        }
        public void MenuButton(object btn)
        {
            //
            // show normal cursor
            //
            Global.HideCursor = false;
            //
            // change the scene to Menu
            //
            Scene otherScene = new MenuScene();
            mmGame.Scene = otherScene;
        }
        public void PlayButton(object btn)
        {

            //PlayBtn.Get<Transform>().Visiable = false;
            PlayBtn.IsVisible = false;
            Global.StateOfGame = GameState.Playing;
            GameStatComponent sc = GameStatus.Get<GameStatComponent>();
            sc.Score = 0;
            MsgBox mb = GameStatus.Get<MsgBox>();
            GameStatus.IsVisible = false;
            //mb.Visible = false;

            //CardDeck.CreateDeckOfCards(false);

            //CreateDealPile();                           // deal stack (falce down)
            //CreateDrawPile();                           // face up dealt one card at a time
            //CreateEmptyAcePiles();                      // 4 of them
            //CreateEmptyPlayPiles();                     // 7 of them
            //CreateEmptyDragPile();                      // used to disp cards being dragged around
            //DealCardsToPiles();                         // draw cards & place in piles above
        }
        public void EndButton(object btn)
        {
            //PlayBtn.Get<Transform>().Visiable = true;
            PlayBtn.IsVisible = true;
            Global.StateOfGame = GameState.Over;
            MsgBox mb = GameStatus.Get<MsgBox>();
            GameStatus.IsVisible = true;
            //mb.Visiable = true;
        }
        public void UpdateScore(int _value)
        {
            /*
             *  Waste to 7 play pile	    5   points
                Waste to Aces pile	        10  points
                play pile to aces pile	    10  points
                Turn over play pile card	5   points
                Aces pile to play pile	    −15 points
                Recycle waste when playing by ones	−100 (minimum score is 0)
             */

            GameStatComponent sc = GameStatus.Get<GameStatComponent>();
            sc.Score += _value;
            if (sc.Score < 0)
                sc.Score = 0;
            possibleScoreValue = 0;
        }
        public void DealCard2Disp(Entity _entity)
        {
            //
            // take last card from the deal deck
            //
            CardPileComponent scDisp = DrawDisp.GetComponent<CardPileComponent>();
            CardPileComponent scTemp = DealDeck.GetComponent<CardPileComponent>();
            if (scTemp == null)
                return;         //not a Pile entity
            //
            // if deal deck is empty, put all face up cards back in deal deck
            //
            if (scTemp.CardsInPile.Count <= 0)
            {
                // 
                // We are recycling the waste cards back in the deal deal deck 
                //
                UpdateScore(-20);

                for (int i = scDisp.CardsInPile.Count - 1; i >= 0; i--)
                {
                    Card _card = scDisp.CardsInPile[i].GetComponent<Card>();
                    _card.IsFaceUp = false;
                    scTemp.CardsInPile.Add(scDisp.CardsInPile[i]);
                }
                scDisp.CardsInPile.Clear();
                return;
            }

            Entity lastCardonPile = scTemp.CardsInPile.LastOrDefault();               //get last card
            scTemp.CardsInPile.Remove(lastCardonPile);

            Card lastCard = lastCardonPile.GetComponent<Card>();
            lastCard.IsFaceUp = true;


            scDisp.CardsInPile.Add(lastCardonPile);

        }
        public void AcePileCard2Drag(Entity _entity)
        {
            //
            // Get the top card from Ace piles
            //
            possibleScoreValue = -15;
            Card2Drag(_entity);
        }
        public void DealtCard2Drag(Entity _entity)
        {
            //
            // Get the top card from DrawDisp and add to DragDisp stack
            //
            possibleScoreValue = 5;
            Card2Drag(_entity);
        }
        public void Card2Drag(Entity _entity)
        {
            //
            // We have a single Card entity to drag
            //
            CardPileComponent scDrag = DragDisp.GetComponent<CardPileComponent>();
            CardPileComponent scTemp = _entity.GetComponent<CardPileComponent>();
            if (scTemp == null)
                return;         //not a Pile entity
            if (scTemp.CardsInPile.Count == 0)
                return;

            Entity lastCardonPile = scTemp.CardsInPile.LastOrDefault();               //get last card
            scTemp.CardsInPile.Remove(lastCardonPile);

            Card lastCard = lastCardonPile.GetComponent<Card>();
            lastCard.IsFaceUp = true;


            scDrag.CardsInPile.Add(lastCardonPile);
            DragComponent sdc = new DragComponent() { EntityOrig = _entity };
            DragDisp.AddComponent<DragComponent>(sdc);
        }
        private bool RectangleContains(Rectangle _rec, Vector2 _point)
        {
            if ((_point.X > _rec.X && _point.X < (_rec.Width + _rec.X)) &&
                (_point.Y > _rec.Y && _point.Y < (_rec.Height + _rec.Y)))
                return true;

            return false;
        }
        public void TakeCards2Drag(Entity _entity)
        {
            DragDisp.Get<Transform>().Visiable = true;
            //
            // Get the top card from 1-7 play stacks (_entity is a card)
            //
            Card _cc = _entity.GetComponent<Card>();
            if (_cc == null)
                return;

            CardPileComponent scTemp = _cc.HoldingPile;
            Entity fromEntity = scTemp.OwnerEntity;
            //
            // Find the card in the pile, then get all cards from that on down
            //
            int cInd = scTemp.CardsInPile.FindIndex(x => x.Tag == _entity.Tag);
            CardPileComponent scDrag = DragDisp.GetComponent<CardPileComponent>();
            //
            // if cInd is less than zero, then something is wrong
            //
            if (cInd < 0)
                return;

            for (int i = cInd; i <= scTemp.CardsInPile.Count - 1; i++)
            {
                Entity lastCard = scTemp.CardsInPile[i];
                Card cc = lastCard.GetComponent<Card>();
                //cc.IsFaceUp = true;
                scDrag.CardsInPile.Add(lastCard);
            }
            //
            // remove cards from original play Pile
            //
            for (int i = 0; i < scDrag.CardsInPile.Count; i++)
            {
                Entity lastCard = scDrag.CardsInPile[i];
                scTemp.CardsInPile.Remove(lastCard);

            }
            //
            // add cards to DispDrag
            //
            DragComponent sdc = new DragComponent() { EntityOrig = fromEntity };
            DragDisp.AddComponent<DragComponent>(sdc);
        }
        public void DropCardFromDrag2PlayStack(Entity _playStack)
        {
            //
            // Dragging cards are being dropped on play stack
            //
            CardPileComponent scDrag = DragDisp.GetComponent<CardPileComponent>();            //cards being dragged
            CardPileComponent scPlay = _playStack.GetComponent<CardPileComponent>();            //cards being dropped
            if (scDrag.CardsInPile.Count == 0)
                return;
            //
            // Test first card of drag to match last card of drop
            //
            Entity firstCardonStack = scDrag.CardsInPile[0];               //get first card
            Card firstCard = firstCardonStack.GetComponent<Card>();
            //
            // Make sure this stack is not empty
            //
            if (scPlay.CardsInPile.Count == 0)
            {
                if (firstCard.FaceImage < 13)          //only a king will sit on empty stack
                {
                    ReturnCardFromDrag2Stack();
                    return;
                }
                //
                //  first card of drop is king, drop all of them
                //
                for (int i = 0; i < scDrag.CardsInPile.Count; i++)
                {
                    scPlay.CardsInPile.Add(scDrag.CardsInPile[i]);
                }
                //
                // set the holding stack for this card pile
                //
                for (int i = 0; i < scPlay.CardsInPile.Count; i++)
                {
                    Card _cc = scPlay.CardsInPile[i].GetComponent<Card>();
                    _cc.HoldingPile = scPlay;
                }
                UpdateScore(possibleScoreValue);
                DragStackClear();
                return;
            }
            //
            // play stack is NOT empty, test cards
            //
            Entity lastCardonStack = scPlay.CardsInPile.LastOrDefault();               //get last card
            Card lastCard = lastCardonStack.GetComponent<Card>();
            if (TestCardsForPlayStack(firstCard, lastCard))
            {
                for (int i = 0; i < scDrag.CardsInPile.Count; i++)
                {
                    scPlay.CardsInPile.Add(scDrag.CardsInPile[i]);
                }
                //
                // set the holding stack for this card pile
                //
                for (int i = 0; i < scPlay.CardsInPile.Count; i++)
                {
                    Card _cc = scPlay.CardsInPile[i].GetComponent<Card>();
                    _cc.HoldingPile = scPlay;
                }
                UpdateScore(possibleScoreValue);
                DragStackClear();
            }
            else
            {
                //
                // -1000 means we are flipping a card in play stack
                //
                if (possibleScoreValue == -1000)
                    UpdateScore(5);

                ReturnCardFromDrag2Stack();
            }

        }
        public bool TestCardsForPlayStack(Card dragCard, Card dropCard)
        {
            bool result = false;

            if ((dragCard == null) || (dropCard == null))
                return false;

            //
            // Test colors
            //
            if ((dropCard.IsRed) && (dragCard.IsRed))               //both red
                return false;
            if ((!dropCard.IsRed) && (!dragCard.IsRed))             //both black
                return false;
            //
            // Face value test
            //
            if (dropCard.FaceImage == dragCard.FaceImage + 1)
                return true;

            return result;
        }
        public void ReturnCardFromDrag2Stack()
        {
            //
            // Drag cards are NOT dropped on any ligit pile, return them to where they came from
            //
            DragComponent scDragComp = DragDisp.GetComponent<DragComponent>();
            if (scDragComp == null)
                return;

            Entity fromEntity = scDragComp.EntityOrig;
            CardPileComponent scDrag = DragDisp.GetComponent<CardPileComponent>();            //cards being dragged
            CardPileComponent scFrom = fromEntity.GetComponent<CardPileComponent>();            //cards to give back
            for (int i = 0; i < scDrag.CardsInPile.Count; i++)
            {
                scFrom.CardsInPile.Add(scDrag.CardsInPile[i]);
            }
            DragStackClear();

        }
        public void DragStackClear()
        {
            //
            // Clear drag stack (cards have been dropped)
            //
            DragDisp.RemoveComponent<DragComponent>();
            CardPileComponent sc = DragDisp.GetComponent<CardPileComponent>();
            sc.CardsInPile.Clear();
        }
        public void DropCardFromDrag2AceStack(Entity _playStack)
        {
            //
            // Ace statck is known as "foundation" pile
            //

            CardPileComponent scDrag = DragDisp.GetComponent<CardPileComponent>();            //cards being dragged

            if (scDrag.CardsInPile.Count != 1)
            {
                ReturnCardFromDrag2Stack();
                return;
            }
            DragComponent scDragComp = DragDisp.GetComponent<DragComponent>();
            if (scDragComp.EntityOrig == _playStack)
            {
                ReturnCardFromDrag2Stack();
                return;
            }

            CardPileComponent scPlay = _playStack.GetComponent<CardPileComponent>();            //cards being dropped
            //
            // first card of drag needs to match last card of drop
            //
            Entity firstCardonStack = scDrag.CardsInPile[0];               //get first card of drag
            Card firstCard = firstCardonStack.GetComponent<Card>();
            //
            // Make sure this stack is not empty
            //
            if (scPlay.CardsInPile.Count == 0)
            {
                if (firstCard.FaceImage != 1)          //only an ACE will sit on empty stack
                {
                    ReturnCardFromDrag2Stack();
                    return;
                }
                //
                //  first card of drap is ACE, drop it
                //
                for (int i = 0; i < scDrag.CardsInPile.Count; i++)
                {
                    scPlay.CardsInPile.Add(scDrag.CardsInPile[i]);
                    CardDeck.Score += scDrag.GetCardFaceImageValue(i);       //card added, calc score
                }
                UpdateScore(10);
                DragStackClear();
                return;
            }
            //
            // play stack is NOT empty, test cards
            //
            Entity lastCardonStack = scPlay.CardsInPile.LastOrDefault();               //get last card
            Card lastCard = lastCardonStack.GetComponent<Card>();
            if (TestCardsForAceStack(firstCard, lastCard))
            {
                for (int i = 0; i < scDrag.CardsInPile.Count; i++)
                {
                    scPlay.CardsInPile.Add(scDrag.CardsInPile[i]);
                    CardDeck.Score += scDrag.GetCardFaceImageValue(i);       //card added, calc score
                }
                UpdateScore(10);
                DragStackClear();
            }
            else
            {
                ReturnCardFromDrag2Stack();
            }
        }
        public bool TestCardsForAceStack(Card dragCard, Card dropCard)
        {
            bool result = false;
            if ((dragCard == null) || (dropCard == null))
                return result;


            if (dropCard.Suit != dragCard.Suit)
                return result;
            //
            // Face value test
            //
            if (dropCard.FaceImage == dragCard.FaceImage - 1)
                result = true;

            return result;
        }
        public Entity FindCardInPlayPile(Entity _playPile, Vector2 _mousePos)
        {
            possibleScoreValue = 0;
            //
            // entity = 1 of the 7 PlayPiles
            //
            PileOfCards Cards = new PileOfCards(_playPile);
            Vector2 startPos = _playPile.Get<Transform>().Position;
            int imax = Cards.CardsInThisPile.Count - 1;
            int ind = imax;

            for (int i = imax; i >= 0; i--)
            {
                Entity cardEntity = Cards.CardsInThisPile[i];
                Card cc = cardEntity.GetComponent<Card>();
                if (cc.IsFaceUp)
                {
                    //
                    // Card we are testing is face up
                    // Find its location, draw a rectangle and see if mouse click is in area
                    // 
                    Vector2 cardPos = startPos + Cards.FanOutDistannce * ind;
                    Rectangle cardRect = new Rectangle((int)cardPos.X - CardDeck.CardWidth / 2, (int)cardPos.Y - CardDeck.CardHeight / 2,
                                                       CardDeck.CardWidth, CardDeck.CardHeight);
                    if (RectangleContains(cardRect, _mousePos))
                    {
                        possibleScoreValue = 0;
                        return cardEntity;
                    }
                }
                else
                {
                    if (cardEntity == Cards.LastCardonStack)
                    {
                        //
                        // We are turning last card face up
                        //
                        cc.IsFaceUp = true;
                        possibleScoreValue = -1000;
                        return cardEntity;
                    }
                }
                ind -= 1;
            }
            return null;
        }
        private void DealCardsToPiles()
        {
            //
            // Card deck is created, shuffle it
            //
            CardDeck.Shuffle();
            CardPileComponent scDisp = DrawDisp.Get<CardPileComponent>();
            scDisp.CardsInPile.Clear();
            //
            // Play Stack 0 gets 1 card
            // Play Stack 1 gets 2 cards
            // .....
            // Play Stack 6 gets 7 cards
            //
            for (int i = 0; i < PlayStacks.Count; i++)
            {
                CardPileComponent sc = PlayStacks[i].GetComponent<CardPileComponent>();
                sc.CardsInPile.Clear();
                for (int j = 0; i >= j; j++)
                {
                    //
                    // Create a card Enitty (face down)
                    //
                    Entity card = CardDeck.DealACard(false, false);
                    var ccomp = card.GetComponent<Card>();
                    ccomp.CardStack = sc.PileID;
                    ccomp.HoldingPile = sc;
                    sc.CardsInPile.Add(card);
                }
                //
                // Find last card in this stack and flip it face up
                //
                CardPileComponent scTemp = PlayStacks[i].GetComponent<CardPileComponent>();
                Entity lastCardonStack = scTemp.CardsInPile.LastOrDefault();       //get last card
                var ccompTemp = lastCardonStack.GetComponent<Card>();              //turn it face up
                ccompTemp.IsFaceUp = true;
            }
            //
            // Deal Pile gets rest of cards (24 of them)
            //
            CardPileComponent scDeal = DealDeck.GetComponent<CardPileComponent>();
            scDeal.CardsInPile.Clear();
            for (int i = 0; i < 52; i++)
            {
                var card = CardDeck.DealACard(false, false);
                if (card == null)
                    break;
                var ccomp = card.GetComponent<Card>();
                ccomp.CardStack = scDeal.PileID;
                ccomp.HoldingPile = scDeal;
                scDeal.CardsInPile.Add(card);
            }
        }
        private void CreateEmptyDragPile()
        {
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            // Drag stack (all cards are faceup and are being moved)
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            //DragDisp = CardDeck.DealEmptyHolder();
            DragDisp = Global.CreateGameEntity("DragStack");
            DragDisp.Tag = 85;
            DragDisp.Name = "DragStack";
            DragDisp.Get<Transform>().Visiable = false;
            DragDisp.Get<Transform>().Position = Vector2.Zero;
            DragDisp.Add(new CardPileComponent() { PileID = 0, CName = "DragStack", FannedDirection = 4 });
            //DragDisp.Add(new PileDispComponent());
            BoxCollider bx = new BoxCollider(CardDeck.CardWidth, CardDeck.CardHeight);
            bx.RenderLayer = Global.BOXCOLLIDER_LAYER ;
            DragDisp.Add(bx);
        }
        private void CreateDealPile()
        {
            if (DealDeck != null)
                Global.DestroyGameEntity(DealDeck);
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            // Deal Deck (image to click on for next card)
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            DealDeck = CardDeck.DealEmptyHolder();
            DealDeck.Tag = 90;
            DealDeck.Name = "DealStack";
            DealDeck.Get<Transform>().Position = drawStack;
            DealDeck.Add(new CardPileComponent() { PileID = 0 });
            DealDeck.Add(new PileDispComponent());
            possibleScoreValue = 5;
        }
        private void CreateDrawPile()
        {
            if (DrawDisp != null)
                Global.DestroyGameEntity(DrawDisp);
            //znznznznznznznznznznznznznznznznznznznznznzn
            // Draw stack (all cards are faceup)
            //znznznznznznznznznznznznznznznznznznznznznzn
            DrawDisp = CardDeck.DealEmptyHolder();
            DrawDisp.Tag = 70;
            DrawDisp.Name = "DrawStack";
            DrawDisp.Get<Transform>().Position = drawStack + new Vector2(100, 0);
            DrawDisp.Add(new CardPileComponent() { PileID = 0, CName = "DrawStack", FannedDirection = 0 });
            DrawDisp.Add(new PileDispComponent());
        }
        private void CreateEmptyAcePiles()
        {
            if (PlayStacks.Count > 0)
            {
                for (int i = 0; i < AceStacks.Count; i++)
                    Global.DestroyGameEntity(AceStacks[i]);
            }
            AceStacks = new List<Entity>();
            //znznznznznznznznznznznznznznznznznznznznznzn
            // 4 Ace stacks to collect cards
            //znznznznznznznznznznznznznznznznznznznznznzn
            Entity aceEnt = CardDeck.DealEmptyHolder();
            aceEnt.Tag = 80;               //special tag for Ace pile
            aceEnt.Name = "AceStack1";
            aceEnt.Get<Transform>().Position = aceStack1;

            aceEnt.Add(new CardPileComponent() { PileID = 1 });
            aceEnt.Add(new PileDispComponent());
            AceStacks.Add(aceEnt);

            aceEnt = CardDeck.DealEmptyHolder();
            aceEnt.Tag = 80;               //special tag for Ace pile
            aceEnt.Name = "AceStack2";
            aceEnt.Get<Transform>().Position = aceStack1 + new Vector2(100, 0);

            aceEnt.Add(new CardPileComponent() { PileID = 2 });
            aceEnt.Add(new PileDispComponent());
            AceStacks.Add(aceEnt);

            aceEnt = CardDeck.DealEmptyHolder();
            aceEnt.Tag = 80;               //special tag for Ace pile
            aceEnt.Name = "AceStack3";
            aceEnt.Get<Transform>().Position = aceStack1 + new Vector2(200, 0);

            aceEnt.Add(new CardPileComponent() { PileID = 3 });
            aceEnt.Add(new PileDispComponent());
            AceStacks.Add(aceEnt);

            aceEnt = CardDeck.DealEmptyHolder();
            aceEnt.Tag = 80;               //special tag for Ace pile
            aceEnt.Name = "AceStack4";
            aceEnt.Get<Transform>().Position = aceStack1 + new Vector2(300, 0);

            aceEnt.Add(new CardPileComponent() { PileID = 4 });
            aceEnt.Add(new PileDispComponent());
            AceStacks.Add(aceEnt);
        }
        private void CreateEmptyPlayPiles()
        {
            if (PlayStacks.Count > 0)
            {
                for (int i = 0; i < PlayStacks.Count; i++)
                    Global.DestroyGameEntity(PlayStacks[i]);
            }
            PlayStacks = new List<Entity>();
            //znznznznznznznznznznznznznznznznznznznznznzn
            // 7 playing stacks
            //znznznznznznznznznznznznznznznznznznznznznzn
            Entity playEnt = Global.CreateGameEntity("PlayStack1", playStack1);
            playEnt.Tag = 1;               //special tag for Ace pile
            playEnt.Add(new Sprite(CardDeck.GetEmptyHolderTexture()));
            playEnt.Add(new BoxCollider(playStack1.X, playStack1.Y, colliderWidth, colliderHeight));
            playEnt.Add(new CardPileComponent() { PileID = 5, FannedDirection = 4 });
            playEnt.Add(new PileDispComponent());
            PlayStacks.Add(playEnt);

            playEnt = Global.CreateGameEntity("PlayStack2", playStack1 + new Vector2(100, 0));
            playEnt.Tag = 2;               //special tag for Ace pile
            playEnt.Add(new Sprite(CardDeck.GetEmptyHolderTexture()));
            playEnt.Add(new BoxCollider(playStack1.X + 200, playStack1.Y, colliderWidth, colliderHeight));
            playEnt.Add(new CardPileComponent() { PileID = 6, FannedDirection = 4 });
            playEnt.Add(new PileDispComponent());
            PlayStacks.Add(playEnt);

            playEnt = Global.CreateGameEntity("PlayStack3", playStack1 + new Vector2(200, 0));
            playEnt.Tag = 3;               //special tag for Ace pile
            playEnt.Add(new Sprite(CardDeck.GetEmptyHolderTexture()));
            playEnt.Add(new BoxCollider(playStack1.X + 400, playStack1.Y, colliderWidth, colliderHeight));
            playEnt.Add(new CardPileComponent() { PileID = 7, FannedDirection = 4 });
            playEnt.Add(new PileDispComponent());
            PlayStacks.Add(playEnt);

            playEnt = Global.CreateGameEntity("PlayStack4", playStack1 + new Vector2(300, 0));
            playEnt.Tag = 4;               //special tag for Ace pile
            playEnt.Add(new Sprite(CardDeck.GetEmptyHolderTexture()));
            playEnt.Add(new BoxCollider(playStack1.X + 600, playStack1.Y, colliderWidth, colliderHeight));
            playEnt.Add(new CardPileComponent() { PileID = 8, FannedDirection = 4 });
            playEnt.Add(new PileDispComponent());
            PlayStacks.Add(playEnt);

            playEnt = Global.CreateGameEntity("PlayStack5", playStack1 + new Vector2(400, 0));
            playEnt.Tag = 5;               //special tag for Ace pile
            playEnt.Add(new Sprite(CardDeck.GetEmptyHolderTexture()));
            playEnt.Add(new BoxCollider(playStack1.X + 800, playStack1.Y, colliderWidth, colliderHeight));
            playEnt.Add(new CardPileComponent() { PileID = 9, FannedDirection = 4 });
            playEnt.Add(new PileDispComponent());
            PlayStacks.Add(playEnt);

            playEnt = Global.CreateGameEntity("PlayStack6", playStack1 + new Vector2(500, 0));
            playEnt.Tag = 6;               //special tag for Ace pile
            playEnt.Add(new Sprite(CardDeck.GetEmptyHolderTexture()));
            playEnt.Add(new BoxCollider(playStack1.X + 1000, playStack1.Y, colliderWidth, colliderHeight));
            playEnt.Add(new CardPileComponent() { PileID = 10, FannedDirection = 4 });
            playEnt.Add(new PileDispComponent());
            PlayStacks.Add(playEnt);

            playEnt = Global.CreateGameEntity("PlayStack7", playStack1 + new Vector2(600, 0));
            playEnt.Tag = 7;               //special tag for Ace pile
            playEnt.Add(new Sprite(CardDeck.GetEmptyHolderTexture()));
            playEnt.Add(new BoxCollider(playStack1.X + 1200, playStack1.Y, colliderWidth, colliderHeight));
            playEnt.Add(new CardPileComponent() { PileID = 11, FannedDirection = 4 });
            playEnt.Add(new PileDispComponent());
            PlayStacks.Add(playEnt);
        }
        private void CreateCardPile(int _dispX, int _dispY)
        {
            Entity oneCardPile = Global.CreateGameEntity(new Vector2(_dispX, _dispY));
            oneCardPile.Tag = 80;
            oneCardPile.Add<PileDispComponent>();
            //BoxCollider bx = new BoxCollider(new Rectangle(0, 0, 75, 100));
            //oneCardPile.Add(bx);

            CardPileComponent cp = new CardPileComponent(){ PileID = 0, CName = "DrawDisp", FannedDirection = 1};
            cp.CardsInPile.Clear();
            //
            // deal out 13 cards
            //
            for (int i=0; i < 13; i++)
            {
                Entity card = CardDeck.DealACard(_enabled:true);     //enabled & faceup
                cp.CardsInPile.Add(card);
            }


            oneCardPile.Add(cp);
        }
    }
}
