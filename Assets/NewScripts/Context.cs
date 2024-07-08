using System;
using System.Collections.Generic;
using Jujutsu_Kaisen_Game_Proyect.Assets.BackEnd;

namespace Console
{
    public class Context
    {
        public Guid triggerPlayer { get; set; }
        public List<Cards> board { get; set; }

        public Context(List<Cards> board, Guid triggerPlayer)
        {
            this.board = board;
            this.triggerPlayer = triggerPlayer;
        }

        public Hand HandOfPlayer(Player player)
        {
            return player.hand;
        }

        public Deck DeckOfPlayer(Player player)
        {
            return player.deck;
        }

        public Field FieldOfPlayer(Player player)
        {
            return player.field;
        }

        public Graveyard GraveyardOfPlayer(Player player)
        {
            return player.graveyard;
        }
    }
}