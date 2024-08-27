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
            string name = (string)node.Name.Evaluate(null, null);
            string faction = (string)node.Faction.Evaluate(null, null);
            string type = (string)node.Type.Evaluate(null, null);
            int power = (int)node.Power.Evaluate(null, null);
            List<string> range = new();
            foreach (var expression in node.Range)
            {
                range.Add((string)expression.Evaluate(null, null));
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
                effectData = new EffectData((string)onActValue.EffectData.Name.Evaluate(null, null));
            }
            PosAction posAction = null;
            if (onActValue.Selector != null)
            {
                posAction = BuildPosAction(onActValue.PosAction);
            }
            return new OnActivation(effectData, selector, posAction);
        }

        private Selector BuildSelector(SelectorNode selector)
        {
            string source = (string)selector.Source.Evaluate(null, null);
            bool single = (bool)selector.Single.Evaluate(null, null);
            string predicate = (string)selector.Predicate.Evaluate(null, null);
            return new Selector(source, predicate, single);
        }

        private EffectData BuildEffectData(EffectDataNode effectData)
        {
            string name = (string)effectData.Name.Evaluate(null, null);
            List<(string, object)> parameter = new();
            foreach (var param in effectData.Params)
            {
                parameter.Add((param.Item1, param.Item2.Evaluate(null, null)));
            }
            return new EffectData(name, parameter);
        }

        private PosAction BuildPosAction(PosActionNode posAction)
        {
            string type = (string)posAction.Type.Evaluate(null, null);
            Selector selector = BuildSelector(posAction.Selector);
            return new PosAction(type, selector);
        }
    }
}