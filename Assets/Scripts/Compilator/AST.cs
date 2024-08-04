using System;
using System.Collections.Generic;

namespace Console
{
    public interface ASTNode
    {

    }

    public class CardNode : ASTNode
    {
        public string name { get; set; }
        public string type { get; set; }
        public string faction { get; set; }
        public List<string> range { get; set; }
        public int power { get; set; }
        public OnActivation onActivation { get; set; }
    }

    public class EffectNode : ASTNode
    {
        public string name { get; private set; }
        protected List<Type> parameters { get; private set; }
        public void Action(List<Cards> targets, Context context) { }
    }
}