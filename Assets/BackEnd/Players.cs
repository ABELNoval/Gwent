using System;
using System.Collections.Generic;

namespace Jujutsu_Kaisen_Game_Proyect.Assets.BackEnd
{
    [Serializable]
    public class Player
    {
        public bool isPlaying;
        public List<Card> hand;
        public List<Card> cementary;
        public Player(List<Card> hand)
        {
            this.hand = hand;
        }
    }
}