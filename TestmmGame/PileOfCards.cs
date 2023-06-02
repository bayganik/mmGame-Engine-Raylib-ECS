
using System.Linq;
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
    public class PileOfCards
    {
        public int Tag { get; set; }                            //tag value of stack entity
        public CardPileComponent StackComp { get; set; }
        public Entity LastCardonStack { get; set; }             //last card in stack
        public List<Entity> CardsInThisPile { get; set; }          //bring cards up one level
        public int FannedDirection { get; set; }                //fanning direction
        public Vector2 FanOutDistannce { get; set; }            //distance of cards from each other
        public int TotalCards { get; set; }
        public PileOfCards(Entity _cardStack)
        {
            //
            // Stack entity holding cards
            //
            Tag = _cardStack.Tag;
            StackComp = new CardPileComponent();
            LastCardonStack = new Entity();

            StackComp = _cardStack.Get<CardPileComponent>();
            if (StackComp == null)
                return;

            if (StackComp.CardsInPile.Count == 0)
                return;
            else
                TotalCards = StackComp.CardsInPile.Count;

            LastCardonStack = StackComp.CardsInPile.LastOrDefault();
            CardsInThisPile = StackComp.CardsInPile;
            FannedDirection = StackComp.FannedDirection;
            switch (FannedDirection)
            {
                case 0:
                    FanOutDistannce = Vector2.Zero;
                    break;
                case 1:
                    FanOutDistannce = new Vector2(30f, 0);
                    break;
                case 2:
                    FanOutDistannce = new Vector2(-30f, 0);
                    break;
                case 3:
                    FanOutDistannce = new Vector2(0, -30f);
                    break;
                case 4:
                    FanOutDistannce = new Vector2(0, 30f);
                    break;
            }
        }
        public Entity GetCard(int _no)
        {
            if ((StackComp.CardsInPile.Count < _no) || (_no > StackComp.CardsInPile.Count))
                return null;

            return StackComp.CardsInPile[_no];
        }
    }
}

