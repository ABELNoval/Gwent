using System.Runtime.InteropServices.ComTypes;
using System;
using System.Collections.Generic;

namespace Console
{
    public static class Context
    {
        public delegate Player FindPlayer(Guid id);
        public static event FindPlayer findPlayer;
        public static Guid triggerPlayer { get; set; }
        public static Board board { get; set; }

        public static Hand HandOfPlayer(Player player)
        {
            return player.hand;
        }

        public static Deck DeckOfPlayer(Player player)
        {
            return player.deck;
        }

        public static Field FieldOfPlayer(Player player)
        {
            return player.field;
        }

        public static Graveyard GraveyardOfPlayer(Player player)
        {
            return player.graveyard;
        }

        public static Deck Deck
        {
            get { return DeckOfPlayer(findPlayer(triggerPlayer)); }
        }

        public static Hand Hand
        {
            get { return HandOfPlayer(findPlayer(triggerPlayer)); }
        }

        public static Field Field
        {
            get { return FieldOfPlayer(findPlayer(triggerPlayer)); }
        }

        public static Graveyard Graveyard
        {
            get { return GraveyardOfPlayer(findPlayer(triggerPlayer)); }
        }
    }
}