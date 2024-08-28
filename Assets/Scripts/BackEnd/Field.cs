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
    public class Field : GameComponent
    {

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
                points += card.attack;
            }
            return points;
        }
    }
}