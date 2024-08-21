using System.Runtime.InteropServices;
using System.Globalization;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Console
{
    public class ProgramNode
    {
        public Dictionary<string, object> properties = new Dictionary<string, object>();

        public void SetProperty(string key, object value)
        {
            if (HasProperty(key))
            {
                throw new Exception("La propiedad ya existe");
            }
            properties[key] = value;
        }

        public T GetProperty<T>(string key)
        {
            if (properties.TryGetValue(key, out var value))
            {
                if (value is T)
                {
                    return (T)value;
                }
                else
                {
                    throw new InvalidCastException($"El valor para la clave \"{key}\" no es del tipo esperado. Se esperaba {typeof(T)}, pero se encontró {value.GetType()}.");
                }
            }
            throw new Exception($"La propiedad \"{key}\" no está definida.");
        }


        public void AddProperty<T>(string key, T value)
        {
            if (properties.TryGetValue(key, out var listValue))
            {
                if (listValue is List<T> list)
                {
                    list.Add(value);
                }
                else
                {
                    throw new Exception($"La propiedad \"{key}\" no es una lista.");
                }
            }
            else
            {
                SetProperty(key, new List<T> { value });
            }
        }

        private bool HasProperty(string key)
        {
            return properties.ContainsKey(key);
        }

        public virtual void Validate() { }
    }

    public class CardNode : ProgramNode
    {

        public ExpressionNode Name
        {
            get => GetProperty<ExpressionNode>("Name");
            private set => SetProperty("Name", value);
        }

        public ExpressionNode Type
        {
            get => GetProperty<ExpressionNode>("Type");
            private set => SetProperty("Type", value);
        }

        public ExpressionNode Faction
        {
            get => GetProperty<ExpressionNode>("Faction");
            private set => SetProperty("Faction", value);
        }

        public List<ExpressionNode> Range
        {
            get => GetProperty<List<ExpressionNode>>("Range");
            private set => SetProperty("Range", value);
        }

        public ExpressionNode Power
        {
            get => GetProperty<ExpressionNode>("Power");
            private set => SetProperty("Power", value);
        }

        public OnActivationNode OnActivation
        {
            get => GetProperty<OnActivationNode>("OnActivation");
            private set => SetProperty("OnActivation", value);
        }


        public void SetName(ExpressionNode name) => Name = name;
        public void SetType(ExpressionNode type) => Type = type;
        public void SetFaction(ExpressionNode faction) => Faction = faction;
        public void SetPower(ExpressionNode power) => Power = power;
        public void AddRange(ExpressionNode range) => AddProperty("Range", range);
        public void SetOnActivation(OnActivationNode onActivation) => OnActivation = onActivation;


        public override void Validate()
        {
            if (Name == null)
            {
                throw new Exception("Falta el nombre de la carta.");
            }
            if (Type == null)
            {
                throw new Exception("Falta el tipo de carta.");
            }
            if (Faction == null)
            {
                throw new Exception("Falta la facción de la carta.");
            }
            if (OnActivation == null)
            {
                throw new Exception("Falta el nodo de activación.");
            }
        }
    }

    public class EffectNode : ProgramNode
    {

        public ExpressionNode Name
        {
            get => GetProperty<ExpressionNode>("Name");
            private set => SetProperty("Name", value);
        }

        public List<(string, ExpressionNode)> Parameters
        {
            get => GetProperty<List<(string, ExpressionNode)>>("Parameters");
            set => SetProperty("Parameters", value);
        }

        public ActionNode Action
        {
            get => GetProperty<ActionNode>("Action");
            set => SetProperty("Action", value);
        }

        public void SetName(ExpressionNode name) => Name = name;
        public void AddParam((string, ExpressionNode) param) => AddProperty("Parameters", param);
        public void SetAction(ActionNode actionNode) => Action = actionNode;

        public override void Validate()
        {
            if (Name == null)
            {
                throw new Exception("Falta el nombre de la carta.");
            }
            if (Action == null)
            {
                throw new Exception("Falta el nodo de acción");
            }
            else
            {
                Action.Validate();
            }
        }
    }

    public class OnActivationNode : ProgramNode
    {

        public List<OnActValueNode> OnActValues
        {
            get => GetProperty<List<OnActValueNode>>("OnActValues");
            private set => SetProperty("OnActValues", value);
        }

        public void AddOnActValue(OnActValueNode onActValue) => AddProperty("OnActValues", onActValue);

        public override void Validate()
        {
            foreach (OnActValueNode onActValue in OnActValues)
            {
                onActValue.Validate();
            }
        }
    }

    public class SelectorNode : ProgramNode
    {

        public ExpressionNode Source
        {
            get => GetProperty<ExpressionNode>("Source");
            private set => SetProperty("Source", value);
        }

        public ExpressionNode Single
        {
            get => GetProperty<ExpressionNode>("Single");
            private set => SetProperty("Single", value);
        }

        public ExpressionNode Predicate
        {
            get => GetProperty<ExpressionNode>("Predicate");
            private set => SetProperty("Predicate", value);
        }

        public void SetSource(ExpressionNode source) => Source = source;
        public void SetSingle(ExpressionNode single) => Single = single;
        public void SetPredicate(ExpressionNode predicate) => Predicate = predicate;

        public override void Validate()
        {
            UnityEngine.Debug.Log("Valida");
            if (Source == null)
            {
                throw new Exception("Falta el source");
            }
            if (Predicate == null)
            {
                throw new Exception("Falta el predicado");
            }
        }
    }

    public class PosActionNode : ProgramNode
    {

        public ExpressionNode Name
        {
            get => GetProperty<ExpressionNode>("Name");
            private set => SetProperty("Name", value);
        }
        public SelectorNode Selector
        {
            get => GetProperty<SelectorNode>("Selector");
            private set => SetProperty("Selector", value);
        }

        public void SetName(ExpressionNode name) => Name = name;
        public void SetSelector(SelectorNode selector) => Selector = selector;

        public override void Validate()
        {
            if (Name == null)
            {
                throw new Exception("Falta el nombre de la carta.");
            }
            Selector?.Validate();
        }
    }

    public class EffectDataNode : ProgramNode
    {

        public ExpressionNode Name
        {
            get => GetProperty<ExpressionNode>("Name");
            private set => SetProperty("Name", value);
        }

        public List<(string, ExpressionNode)> Params
        {
            get => GetProperty<List<(string, ExpressionNode)>>("Params");
            private set => SetProperty("Params", value);
        }

        public override void Validate()
        {
            if (Name == null)
            {
                throw new Exception("Falta el nombre");
            }
        }
    }

    public class OnActValueNode : ProgramNode
    {
        public SelectorNode Selector
        {
            get => GetProperty<SelectorNode>("Selector");
            private set => SetProperty("Selector", value);
        }

        public PosActionNode PosAction
        {
            get => GetProperty<PosActionNode>("PosAction");
            private set => SetProperty("PosAction", value);
        }

        public EffectDataNode EffectData
        {
            get => GetProperty<EffectDataNode>("EffectData");
            private set => SetProperty("EffectData", value);
        }

        public void SetEffectData(EffectDataNode effectData) => EffectData = effectData;
        public void SetPosAction(PosActionNode posAction) => PosAction = posAction;
        public void SetSelector(SelectorNode selector) => Selector = selector;

        public override void Validate()
        {
            Selector?.Validate();
            EffectData.Validate();
            PosAction?.Validate();
        }

    }

    public class ActionNode : ProgramNode
    {
        public List<ExpressionNode> expressions { get; }

        public ActionNode(List<ExpressionNode> expressions)
        {
            this.expressions = expressions;
        }

        public override void Validate()
        {
            if (expressions.Count == 0)
            {
                throw new Exception("Faltan las expresiones");
            }
        }
    }

}