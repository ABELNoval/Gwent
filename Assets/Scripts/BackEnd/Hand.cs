using System;
using System.Collections.Generic;

namespace Console
{
    public class Hand : GameComponent
    {
        public Hand(List<Cards> cards)
        {
            this.cards = cards;
        }

        public override void SendBottom(Cards card)
        {
            cards.Add(card);
        }

        public override void Push(Cards card)
        {
            List<Cards> listResult = new List<Cards>();
            listResult.Add(card);
            foreach (Cards c in cards)
            {
                listResult.Add(c);
            }
            cards = listResult;
        }

        public override void Remove(Cards card)
        {
            cards.Remove(card);
        }

        public override List<Cards> Find(Predicate<Cards> predicate)
        {
            return cards.FindAll(predicate);
        }

        public override Cards Pop()
        {
            Cards card = cards[0];
            cards.RemoveAt(0);
            return card;
        }
    }
}