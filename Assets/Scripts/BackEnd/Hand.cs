using System;
using System.Collections.Generic;

namespace Console
{
    public class Hand : GameComponent
    {
        public event UpdateInterface updateInterface;
        public Hand(List<Cards> cards)
        {
            this.cards = cards;
        }

        public override void SendBottom(Cards card)
        {
            cards.Add(card);
            updateInterface(owner);
        }

        public override void Push(Cards card)
        {
            List<Cards> listResult = new List<Cards>
            {
                card
            };
            foreach (Cards c in cards)
            {
                listResult.Add(c);
            }
            cards = listResult;
            updateInterface(owner);
        }

        public override void Remove(Cards card)
        {
            cards.Remove(card);
            updateInterface(owner);
        }

        public override List<Cards> Find(Predicate<Cards> predicate)
        {
            return cards.FindAll(predicate);
        }

        public override Cards Pop()
        {
            Cards card = cards[0];
            cards.RemoveAt(0);
            updateInterface(owner);
            return card;
        }

        public override void Shuffle()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                int a = random.Next(0, cards.Count);
                int b = random.Next(0, cards.Count);
                Swap(a, b);
            }
            updateInterface(owner);
        }

        private void Swap(int a, int b)
        {
            Cards aux = cards[a];
            cards[a] = cards[b];
            cards[b] = aux;
        }
    }
}