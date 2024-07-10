using System;
using System.Collections.Generic;
using UnityEngine;

namespace Console
{
    public delegate void PlayCard(GameObject card);
    public enum FieldPosition
    {
        Melee,
        Range,
        Siege,
        Graveyard,
        BuffMelee,
        BuffRange,
        BuffSiege,
        Climate
    }
    public class Field
    {
        System.Random random = new System.Random();
        public List<Cards> cards = new List<Cards>();

        public void SendButtom(Cards card)
        {
            cards.Add(card);
        }

        public void Push(Cards card)
        {
            List<Cards> listResult = new List<Cards>();
            listResult.Add(card);
            foreach (Cards c in cards)
            {
                listResult.Add(c);
            }
            cards = listResult;
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
            cards.Remove(card);
            return card;
        }

        public int GetPoints()
        {
            int points = 0;
            foreach (Cards card in cards)
            {
                points += card.attack;
            }
            return points;
        }
    }
}