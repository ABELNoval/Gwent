using System;

namespace Console
{
    public enum Font
    {
        hand,
        otherHand,
        deck,
        otherDeck,
        field,
        otherField,
        parent
    }
    public class Selector
    {
        public Font source;
        public bool single;
        public Predicate<Cards> predicate;

        public Selector(Font source, Predicate<Cards> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }
    }
}