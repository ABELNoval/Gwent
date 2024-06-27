using System;
using System.Collections.Generic;

namespace Console
{
    [Serializable]
    public class Store
    {
        public string text = "";
        public List<Deck> decks = new List<Deck>();
        public List<Effect> effects = new List<Effect>();
    }
}