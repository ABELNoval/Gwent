using System.Runtime.InteropServices.ComTypes;
using System;
using System.Collections.Generic;

namespace Console
{
    public static class Context
    {
        public delegate Player FindPlayer(Guid id);
        public static event FindPlayer findPlayer;
        public delegate Player FindPlayerWithString(string name);
        public static event FindPlayerWithString findPlayerWithString;
        public static Guid triggerPlayer { get; set; }
        public static Player secondPlayer { get; set; }
        public static Board board { get; set; }

        public static Hand HandOfPlayer(object player)
        {
            if (player is string)
            {
                return findPlayerWithString(player.ToString()).hand;
            }
            if (player is Guid)
            {
                return findPlayer((Guid)player).hand;
            }
            return ((Player)player).hand;
        }

        public static Deck DeckOfPlayer(object player)
        {
            if (player is string)
            {
                return findPlayerWithString(player.ToString()).deck;
            }
            if (player is Guid)
            {
                return findPlayer((Guid)player).deck;
            }
            return ((Player)player).deck;
        }

        public static Field FieldOfPlayer(object player)
        {
            if (player is string)
            {
                return findPlayerWithString(player.ToString()).field;
            }
            if (player is Guid)
            {
                return findPlayer((Guid)player).field;
            }
            return ((Player)player).field;
        }

        public static Graveyard GraveyardOfPlayer(object player)
        {
            if (player is string)
            {
                return findPlayerWithString(player.ToString()).graveyard;
            }
            if (player is Guid)
            {
                return findPlayer((Guid)player).graveyard;
            }
            return ((Player)player).graveyard;
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