using System;
using System.Collections.Generic;

namespace Console
{
    [Serializable]
    public class Cards
    {
        public Guid owner { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string faction { get; set; }
        public List<string> range { get; set; }
        public int attack { get; set; }
        public string img { get; set; }
        public List<OnActivation> onActivation { get; set; }

        public Cards(string name, string type, string description, string faction, List<string> range, int attack, string img, List<OnActivation> onActivation)
        {
            this.name = name;
            this.type = type;
            this.description = description;
            this.range = range;
            this.faction = faction;
            this.attack = attack;
            this.img = img;
            this.onActivation = onActivation;
        }

    }

    public class OnActivation
    {
        public string effect;
        public Selector selector;
        public OnActivation posAction;

        public OnActivation(string effect, Selector selector, OnActivation posAction)
        {
            this.effect = effect;
            this.selector = selector;
            this.posAction = posAction;
        }
    }
}