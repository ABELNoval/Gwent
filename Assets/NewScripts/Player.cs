using System;
using System.Collections.Generic;
using System.Diagnostics;
using Jujutsu_Kaisen_Game_Proyect.Assets.BackEnd;
using UnityEngine.XR;

namespace Console
{
    public class Player
    {
        public Deck deck;
        public Hand hand;
        public Guid id;
        public Field field;
        public Player(Deck deck)
        {
            this.deck = deck;
            id = Guid.NewGuid();
            field = new Field();
            SaveIdInTheCards();
        }

        public void GenerateHand()
        {
            hand = deck.GenerateHand();
        }

        private void SaveIdInTheCards()
        {
            deck.SaveID(id);
        }
    }
}