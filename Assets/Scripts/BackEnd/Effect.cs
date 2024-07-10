using System;
using System.Collections.Generic;

namespace Console
{
    [Serializable]
    public class Effect
    {
        public string name { get; private set; }
        protected List<Type> parameters { get; private set; }
        public Effect(string name, List<Type> parameters)
        {
            this.name = name;
            this.parameters = parameters;
        }

        public void Action(List<Cards> targets, Context context) { }
    }
}