using System;
using System.Collections.Generic;
using UnityEngine;

namespace Console
{
    [Serializable]
    public class Deck : GameComponent
    {
        // public event PopObj popObj;
        // public event ShuffleObj shuffleObj;
        // public event SendBottomObj sendBottomObj;
        // public event PushObj pushObj;
        // public event RemoveObj removeObj;
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
            //sendBottomObj("deck", owner);
        }

        public override void Remove(Cards card)
        {
            cards.Remove(card);
            //removeObj("deck", owner, card.owner);
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

        public override List<Cards> Find(Predicate<Cards> predicate)
        {
            return cards.FindAll(predicate);
        }

        public override Cards Pop()
        {
            Cards card = cards[0];
            cards.RemoveAt(0);
            //popObj("deck", owner);
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
            //pushObj("deck", owner);
        }
        public override void Shuffle()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                int a = random.Next(0, cards.Count);
                int b = random.Next(0, cards.Count);
                Swap(a, b);
            }
            //shuffleObj("deck", owner);
        }

        private void Swap(int a, int b)
        {
            Cards aux = cards[a];
            cards[a] = cards[b];
            cards[b] = aux;
        }
    }
}