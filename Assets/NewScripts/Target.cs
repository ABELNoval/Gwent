using System;
using System.Collections.Generic;

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
    public class Target<T>
    {
        public List<Font> source;
        public bool single;
        public Predicate<T> predicate;
        public bool IsAPosActivation()
        {
            return source.Contains(Font.parent);
        }
    }
}