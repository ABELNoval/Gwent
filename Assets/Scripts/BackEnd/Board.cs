using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Console
{
    public class Board : GameComponent
    {
        public override void SendBottom(Cards card)
        {
            cards.Add(card);
        }

        public override void Remove(Cards card)
        {
            cards.Remove(card);
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