using System;

namespace Console
{
    // public enum Font
    // {
    //     board,
    //     hand,
    //     otherHand,
    //     deck,
    //     otherDeck,
    //     field,
    //     otherField,
    //     parent
    // }

    public class Selector
    {
        public string source;
        public bool single;
        public Predicate<Cards> predicate;

        public Selector(string source, Predicate<Cards> predicate, bool single = false)
        {
            this.single = single;
            this.source = source;
            this.predicate = predicate;
        }
    }
}