using System.Collections.Generic;
using Jujutsu_Kaisen_Game_Proyect.Assets.BackEnd;

namespace Console
{
    public class Context
    {
        public int playerTrigger { get; set; }
        public List<Cards> board { get; set; }
        public List<Cards> handOfPlayer { get; set; }
        public List<Cards> graveyardOfPlayer { get; set; }
        public List<Cards> deckOfPlayer { get; set; }
        public List<Cards> fieldOfPlayer { get; set; }

        public Context(List<Cards> board, int playerTrigger, List<Cards> handOfPlayer, List<Cards> graveyardOfPlayer, List<Cards> deckOfPplayer, List<Cards> fieldOfPlayer)
        {
            this.board = board;
            this.playerTrigger = playerTrigger;
            this.graveyardOfPlayer = graveyardOfPlayer;
            this.handOfPlayer = handOfPlayer;
            this.fieldOfPlayer = fieldOfPlayer;
            this.deckOfPlayer = deckOfPplayer;
        }


    }
}