using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Entitas;
using System.Numerics;
using Raylib_cs;

namespace mmGameEngine
{
    public class CardPileComponent : Component
    {
        //
        // id the card Pile eg. Dealer, Player, Discard, etc
        //
        public int PileID;                 //0 - 99 
        public string CName = "Pile of Cards";
        public Vector2 Location;
        public int InitDealCnt = 0;         //initial num of cards dealt
        public int MaxDealCnt = 0;          //Max num of cards dealt

        public float xOffset = 0.35f;       //disp offset on x-axis
        public float yOffset;               //disp offset on y-axis
        public float xPan;                  //disp panning on x-axis
        public float yPan;                  //disp panning on y-axis
        public bool FaceUp = true;         //are cards face up for this Pile?
        //
        // Tracking a Pile of cards 
        //
        public List<Entity> CardsInPile;
        public List<int> BlockedPiles;     // Other Piles this one is blocking, if empty then none.
        //
        // are cards in this Pile fanned out?
        //
        public int FannedDirection = 0;     // 0=Pile on top eachother, 1=right, 2=left, 3=up, 4=down
        public float FannedOffset = 0.35f;  // how far to separate the cards from eachother
        //
        // entity moving on its own
        //
        public bool IsMoving;

        public CardPileComponent()
        {
            CardsInPile = new List<Entity>();
            BlockedPiles = new List<int>();
        }
        public Entity GetLastCard()
        {
            if (CardsInPile.Count <= 0)
                return null;

            return CardsInPile.LastOrDefault();
        }
        public Entity GetFirstCard()
        {
            if (CardsInPile.Count <= 0)
                return null;

            return CardsInPile[0];
        }
        public Entity GetCard(int _cardSeq)
        {
            if (_cardSeq > CardsInPile.Count - 1)
                return null;

            return CardsInPile[_cardSeq];
        }
        public int GetCardFaceImageValue(int _cardSeq)
        {
            Entity card = GetCard(_cardSeq);
            if (card == null)
                return 0;
            Card ccomp = card.GetComponent<Card>();
            return ccomp.FaceImage;
        }
    }
}
