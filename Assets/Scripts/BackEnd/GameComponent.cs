using System;
using System.Collections.Generic;
using UnityEngine;

namespace Console
{
    public abstract class GameComponent
    {
        public System.Random random = new System.Random();
        public Player owner;
        public List<Cards> cards = new List<Cards>();
        public abstract void Push(Cards card);
        public abstract Cards Pop();
        public abstract void SendBottom(Cards card);
        public abstract void Remove(Cards card);
        public abstract List<Cards> Find(Predicate<Cards> predicate);
        public void Shuffle()
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