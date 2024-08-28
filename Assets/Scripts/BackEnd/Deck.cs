using System;
using System.Collections.Generic;
using UnityEngine;

namespace Console
{
    [Serializable]
    public class Deck : GameComponent
    {
        public Guid id;
        public string Name { get; set; }

        public Deck(Guid id, string name)
        {
            this.id = id;
            this.Name = name;
        }

        public override void SendBottom(Cards card)
        {
            cards.Add(card);
        }

        public override void Remove(Cards card)
        {
            cards.Remove(card);
        }

        public Cards DrawCard()
        {
            Debug.Log($"{cards.Count}");
            Cards card = cards[0];
            cards.Remove(card);
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

        public override Cards Pop()
        {
            Cards card = cards[0];
            cards.RemoveAt(0);
            return card;
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
    }
}