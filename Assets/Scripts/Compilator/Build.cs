using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Console
{
    public class BuildNode
    {
        public Cards BuildCard(CardNode node)
        {
            string name = (string)node.Name.Evaluate(null, null, null);
            string faction = (string)node.Faction.Evaluate(null, null, null);
            string type = (string)node.Type.Evaluate(null, null, null);
            int power = (int)node.Power.Evaluate(null, null, null);
            List<string> range = new();
            foreach (var expression in node.Range)
            {
                range.Add((string)expression.Evaluate(null, null, null));
            }
            List<OnActivation> onActivation = new();
            foreach (var onActValue in node.OnActivation.OnActValues)
            {
                onActivation.Add(BuildOnActivation(onActValue));
            }
            return new Cards(name, type, "...", faction, range, power, "Art/Images/Nanami", onActivation);
        }

        private OnActivation BuildOnActivation(OnActValueNode onActValue)
        {
            Selector selector = null;
            if (onActValue.Selector != null)
            {
                selector = BuildSelector(onActValue.Selector);
            }
            EffectData effectData;
            if (onActValue.EffectData.Params != null)
            {
                effectData = BuildEffectData(onActValue.EffectData);
            }
            else
            {
                effectData = new EffectData((string)onActValue.EffectData.Name.Evaluate(null, null, null));
            }

            if (onActValue.PosAction != null)
            {
                PosAction posAction = BuildPosAction(onActValue.PosAction);
                return new OnActivation(effectData, selector, posAction);
            }
            return new OnActivation(effectData, selector, null);
        }

        private Selector BuildSelector(SelectorNode selector)
        {
            string source = (string)selector.Source.Evaluate(null, null, null);
            bool single = (bool)selector.Single.Evaluate(null, null, null);
            return new Selector(source, selector.Predicate as PredicateNode, single);
        }

        private EffectData BuildEffectData(EffectDataNode effectData)
        {
            string name = (string)effectData.Name.Evaluate(null, null, null);
            List<(string, object)> parameters = new();
            foreach (var param in effectData.Params)
            {
                parameters.Add((param.Item1, param.Item2));
            }
            return new EffectData(name, parameters);
        }

        private PosAction BuildPosAction(PosActionNode posAction)
        {
            string type = (string)posAction.Type.Evaluate(null, null, null);
            List<(string, object)> parameters = new();
            if (posAction.Params != null)
            {
                foreach (var param in posAction.Params)
                {
                    parameters.Add((param.Item1, param.Item2));
                }
            }
            Selector selector = BuildSelector(posAction.Selector);
            return new PosAction(type, selector, parameters);
        }
    }
}