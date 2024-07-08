using System;
using System.Collections.Generic;
using Jujutsu_Kaisen_Game_Proyect.Assets.BackEnd;
using UnityEngine;
using UnityEngine.XR;

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

        public void SendButtom(Cards card)
        {
            cards.Add(card);
        }

        public void Remove(Cards card)
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

        public Hand GenerateHand()
        {
            Hand hand = new Hand(new List<Cards>());
            for (int i = 0; i < 10; i++)
            {
                Cards card = cards[random.Next(0, cards.Count)];
                hand.cards.Add(card);
                cards.Remove(card);
            }
            return hand;
        }

        public void SaveID(Guid id)
        {
            foreach (Cards card in cards)
            {
                card.owner = id;
            }
        }

        public List<Cards> Find(Predicate<Cards> predicate)
        {
            return cards.FindAll(predicate);
        }

        public Cards Pop()
        {
            Cards card = cards[0];
            cards.RemoveAt(0);
            return card;
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
    }
}