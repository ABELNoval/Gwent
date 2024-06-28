using System;
using System.Collections.Generic;
using Jujutsu_Kaisen_Game_Proyect.Assets.BackEnd;
using UnityEngine;

namespace Console
{
    [Serializable]
    public class Deck
    {
        public Guid id;
        private System.Random random = new System.Random();
        public List<Cards> cards = new List<Cards>();
        public string name { get; set; }

        public Deck(Guid id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public void AddCard(Cards card)
        {
            cards.Add(card);
        }
        public void RemoveCard(Cards card)
        {
            cards.Remove(card);
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
        public Cards DrawCard()
        {
            Cards card = cards[0];
            cards.RemoveAt(0);
            return card;
        }

        public List<Cards> GenerateHand()
        {
            List<Cards> hand = new List<Cards>();
            for (int i = 0; i < 10; i++)
            {
                Cards card = cards[random.Next(0, cards.Count)];
                hand.Add(card);
                cards.Remove(card);
            }
            return hand;
        }
    }
}