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
        public List<Cards> hand;
        public Guid id = new Guid();
        //public Field field;
        public Player(Deck deck)
        {
            this.deck = deck;
        }

        public void GenerateHand()
        {
            hand = deck.GenerateHand();
        }

    }
}