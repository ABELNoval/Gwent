using System;
using System.Collections.Generic;

namespace Console
{
    public class Graveyard
    {

        System.Random random = new Random();
        public List<Cards> cards = new List<Cards>();

        public void SendButtom(Cards card)
        {
            cards.Add(card);
        }

        public void Push(Cards card)
        {

            /*List<Cards> listResult = new List<Cards>();
            listResult.Add(card);
            foreach (Cards c in cards)
            {
                listResult.Add(c);
            }
            cards = listResult;*/
        }

        public void Remove(Cards card)
        {
            cards.Remove(card);
        }

        public List<Cards> Find(Predicate<Cards> predicate)
        {
            return cards.FindAll(predicate);
        }

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

        public Cards Pop()
        {
            Cards card = cards[0];
            cards.RemoveAt(0);
            return card;
        }
    }
}