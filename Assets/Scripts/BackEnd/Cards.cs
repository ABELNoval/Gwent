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
        public int power { get; set; }
        public string img { get; set; }
        public List<OnActivation> onActivation { get; set; }

        public Cards(string name, string type, string description, string faction, List<string> range, int power, string img, List<OnActivation> onActivation)
        {
            this.name = name;
            this.type = type;
            this.description = description;
            this.range = range;
            this.faction = faction;
            this.power = power;
            this.img = img;
            this.onActivation = onActivation;
        }

    }

    public class OnActivation
    {
        public EffectData effect;
        public Selector selector;
        public PosAction posAction;

        public OnActivation(EffectData effect, Selector selector = null, PosAction posAction = null)
        {
            this.effect = effect;
            this.selector = selector;
            this.posAction = posAction;
        }

        public void GenerateEffect()
        {
            (EffectNode, GlobalContext) effectNode = effect.GetEffect();
            if (selector != null)
            {
                List<Cards> targets = selector.GetTargets(null);
                ActiveEffect(effectNode.Item1, effectNode.Item2, targets);
            }
            else
            {
                ActiveEffect(effectNode.Item1, effectNode.Item2);
            }
            if (posAction != null)
                posAction.GenerateEffect(selector);
        }

        private void ActiveEffect(EffectNode effectNode, GlobalContext globalContext, List<Cards> targets = null)
        {
            foreach (var expression in effectNode.Action.expressions)
            {
                expression.Evaluate(globalContext, targets, null);
            }
        }
    }

    public class EffectData
    {
        public string name { get; }
        public List<(string, object)> parameters { get; }
        public EffectData(string name, List<(string, object)> parameters = null)
        {
            this.name = name;
            this.parameters = parameters;
        }

        public (EffectNode, GlobalContext) GetEffect()
        {
            EffectNode effectNode = Store.GetEffectNode(name);
            GlobalContext globalContext = new GlobalContext();
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    globalContext.DefineVariable(parameter.Item1.ToLower(), parameter.Item2);
                }
            }
            return (effectNode, globalContext);
        }
    }

    public class Selector
    {
        public string source;
        public bool single;
        public PredicateNode predicate;

        public Selector(string source, PredicateNode predicate, bool single = false)
        {
            this.single = single;
            this.source = source;
            this.predicate = predicate;
        }

        public List<Cards> GetTargets(Selector parent)
        {
            List<Cards> cards = new();
            switch (source)
            {
                case "board":
                    cards = Context.board.cards;
                    break;
                case "hand":
                    cards = Context.Hand.cards;
                    break;
                case "deck":
                    cards = Context.Deck.cards;
                    break;
                case "field":
                    cards = Context.Field.cards;
                    break;
                case "graveyard":
                    cards = Context.Graveyard.cards;
                    break;
                case "otherhand":
                    cards = Context.HandOfPlayer(Context.secondPlayer).cards;
                    break;
                case "otherdeck":
                    cards = Context.DeckOfPlayer(Context.secondPlayer).cards;
                    break;
                case "otherfield":
                    cards = Context.FieldOfPlayer(Context.secondPlayer).cards;
                    break;
                case "othergraveyard":
                    cards = Context.GraveyardOfPlayer(Context.secondPlayer).cards;
                    break;
                case "parent":
                    cards = parent.GetTargets(null);
                    break;
            }
            cards = (List<Cards>)predicate.Evaluate(new GlobalContext(), cards, null);
            if (single)
            {
                return new List<Cards>() { cards[0] };
            }
            return cards;
        }
    }

    public class PosAction
    {
        public string type { get; }
        public Selector selector { get; }
        public List<(string, object)> parameters { get; }

        public PosAction(string type, Selector selector = null, List<(string, object)> parameters = null)
        {
            this.type = type;
            this.selector = selector;
            this.parameters = parameters;
        }

        public void GenerateEffect(Selector parent)
        {
            (EffectNode, GlobalContext) effectNode = GetEffect();
            if (selector != null)
            {
                List<Cards> targets = selector.GetTargets(parent);
                ActiveEffect(effectNode.Item1, effectNode.Item2, targets);
            }
            else
            {
                ActiveEffect(effectNode.Item1, effectNode.Item2, parent.GetTargets(null));
            }
        }

        public (EffectNode, GlobalContext) GetEffect()
        {
            EffectNode effectNode = Store.GetEffectNode(type);
            GlobalContext globalContext = new GlobalContext();
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    globalContext.DefineVariable(parameter.Item1.ToLower(), parameter.Item2);
                }
            }
            return (effectNode, globalContext);
        }

        private void ActiveEffect(EffectNode effectNode, GlobalContext globalContext, List<Cards> targets = null)
        {
            foreach (var expression in effectNode.Action.expressions)
            {
                expression.Evaluate(globalContext, targets, null);
            }
        }
    }
}