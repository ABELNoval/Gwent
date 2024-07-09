using System;
using System.Collections.Generic;
using Jujutsu_Kaisen_Game_Proyect.Assets.BackEnd;

namespace Console
{
    public class Context
    {
        public delegate Player FindPlayer(Guid id);
        public event FindPlayer findPlayer;
        public Guid triggerPlayer { get; set; }
        public List<Cards> board { get; set; }

        public Context(List<Cards> board, Guid triggerPlayer)
        {
            this.board = board;
            this.triggerPlayer = triggerPlayer;
        }

        public Hand HandOfPlayer(Guid id)
        {
            Player player = findPlayer(id);
            return player.hand;
        }

        public Deck DeckOfPlayer(Guid id)
        {
            Player player = findPlayer(id);
            return player.deck;
        }

        public Field FieldOfPlayer(Guid id)
        {
            Player player = findPlayer(id);
            return player.field;
        }

        public Graveyard GraveyardOfPlayer(Guid id)
        {
            Player player = findPlayer(id);
            return player.graveyard;
        }

        public Deck Deck
        {
            get { return DeckOfPlayer(triggerPlayer); }
        }

        public Hand Hand
        {
            get { return HandOfPlayer(triggerPlayer); }
        }

        public Field Field
        {
            get { return FieldOfPlayer(triggerPlayer); }
        }

        public Graveyard Graveyard
        {
            get { return GraveyardOfPlayer(triggerPlayer); }
        }
    }
}