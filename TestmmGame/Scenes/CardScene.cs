using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using mmGameEngine;
using Raylib_cs;
using Transform = mmGameEngine.Transform;
using Entitas;

namespace TestmmGame
{
    /*
     * Scene entities do not obey camera.  They are always within the screen space
     * Game  entities do obey camera.  Must call Global.GetMousePosition() or
     *                                           Global.GetMouseX()
     *                                           Global.GetMouseY()
     *                                           Global.WorldPosition(Vector2)
     */
    public class CardScene : Scene
    {
        CardDeckManager CardDeck;

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
            Global.SceneWidth = 800;
            Global.SceneTitle = "Card Scene";
            Global.SceneClearColor = Color.BLUE;
        }
        public override void Play()
        {
            Global.CurrentScene = this;
            Global.DebugRenderEnabled = true;

            dispY = (int)Global.WindowCenter.Y - 100;
            dispX = 40;
            //znznznznznznznznznznznznznznznznzn
            // All game logic goes at this point
            //znznznznznznznznznznznznznznznznzn
            //
            // Crosshair mouse
            //
            Entity CH = this.CreateGameEntity(Vector2.Zero);
            CH.name = "cursor";

            textureImage = Raylib.LoadTexture("Assets/Img/crosshair.png");
            Spr = new Sprite(textureImage);
            Spr.RenderLayer = Global.CURSOR_LAYER;              //on top of everything
            CH.Add(Spr);
            BoxCollider bxxx = new BoxCollider(8, 8);
            CH.Add(bxxx);
            CH.Add<CrossHairComponent>();
            //
            // Create a deck of cards
            //
            CardDeck = new CardDeckManager(this);
            CardDeck.CreateDeckOfCards(false);
            //
            // show joker and back
            //
            Entity joker = CardDeck.DealAJoker(true);                 //enabled & face up
            //bxxx = new BoxCollider(new Rectangle(0, 0, 79, 100));
            //joker.Add(bxxx);

            joker.name = "joker";
            joker.Get<Transform>().Position = new Vector2(dispX, 100);

            Entity cardBack = CardDeck.DealAJoker(true, false);       //enabled & face down
            cardBack.name = "JockerBack";
            cardBack.Get<Transform>().Position = new Vector2(dispX + 110, 100);

            CreateCardPile(dispX, dispY);                     //HEARTS
            CreateCardPile(dispX, dispY + 110);               //DIAMONDS
            CreateCardPile(dispX, dispY + 220);               //CLUBS
            CreateCardPile(dispX, dispY + 330);               //SPADES


            //znznznznznznznznznznznznznzn
            //   Scene Starts Playing
            //znznznznznznznznznznznznznzn

        }

        private void CreateCardPile(int _dispX, int _dispY)
        {
            Entity oneCardPile = CreateGameEntity(new Vector2(_dispX, _dispY));
            oneCardPile.tag = 80;
            oneCardPile.Add<PileDispComponent>();
            //BoxCollider bx = new BoxCollider(new Rectangle(0, 0, 75, 100));
            //oneCardPile.Add(bx);

            CardPile cp = new CardPile(){ PileID = 0, CName = "DrawDisp", FannedDirection = 1};
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
