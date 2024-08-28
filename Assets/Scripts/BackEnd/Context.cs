using System.Runtime.InteropServices.ComTypes;
using System;
using System.Collections.Generic;

namespace Console
{
    public class Context
    {
        public delegate Player FindPlayer(Guid id);
        public event FindPlayer findPlayer;
        public Guid triggerPlayer { get; set; }
        public Board board { get; set; }

        public Context(List<Cards> cards, Guid triggerPlayer)
        {
            board.cards = cards;
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

        public Deck Deck
        {
            get { return DeckOfPlayer(findPlayer(triggerPlayer)); }
        }

        public Hand Hand
        {
            get { return HandOfPlayer(findPlayer(triggerPlayer)); }
        }

        public Field Field
        {
            get { return FieldOfPlayer(findPlayer(triggerPlayer)); }
        }

        public Graveyard Graveyard
        {
            get { return GraveyardOfPlayer(findPlayer(triggerPlayer)); }
        }
    }
}