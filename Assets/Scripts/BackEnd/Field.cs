using System;
using System.Collections.Generic;
using UnityEngine;

namespace Console
{
    public class Field : GameComponent
    {
        public event RemoveCard removeCard;
        public override void SendBottom(Cards card)
        {
            cards.Add(card);
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
        }

        public override void Remove(Cards card)
        {
            cards.Remove(card);
            removeCard(owner, card);
        }

        public override List<Cards> Find(Predicate<Cards> predicate)
        {
            return cards.FindAll(predicate);
        }

        public override Cards Pop()
        {
            Cards card = cards[0];
            cards.Remove(card);
            return card;
        }

        public int GetPoints()
        {
            int points = 0;
            foreach (Cards card in cards)
            {
                points += card.power;
            }
            return points;
        }

        public override void Shuffle()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                int a = random.Next(0, cards.Count);
                int b = random.Next(0, cards.Count);
                Swap(a, b);
            }
        }

        private void Swap(int a, int b)
        {
            Cards aux = cards[a];
            cards[a] = cards[b];
            cards[b] = aux;
        }
    }
}