using System;
using System.Collections.Generic;

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

        public string Name
        {
            get => GetProperty<string>("Name");
            private set => SetProperty("Name", value);
        }

        public string Type
        {
            get => GetProperty<string>("Type");
            private set => SetProperty("Type", value);
        }

        public string Faction
        {
            get => GetProperty<string>("Faction");
            private set => SetProperty("Faction", value);
        }

        public List<string> Range
        {
            get => GetProperty<List<string>>("Range");
            private set => SetProperty("Range", value);
        }

        public int Power
        {
            get => GetProperty<int>("Power");
            private set => SetProperty("Power", value);
        }

        public OnActivationNode OnActivation
        {
            get => GetProperty<OnActivationNode>("OnActivation");
            private set => SetProperty("OnActivation", value);
        }


        public void SetName(string name) => Name = name;
        public void SetType(string type) => Type = type;
        public void SetFaction(string faction) => Faction = faction;
        public void SetPower(int power) => Power = power;
        public void AddRange(string range) => AddProperty("Range", range);
        public void SetOnActivation(OnActivationNode onActivation) => OnActivation = onActivation;


        public override void Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new Exception("Falta el nombre de la carta.");
            }
            if (string.IsNullOrEmpty(Type))
            {
                throw new Exception("Falta el tipo de carta.");
            }
            if (string.IsNullOrEmpty(Faction))
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

        public string Name
        {
            get => GetProperty<string>("Name");
            private set => SetProperty("Name", value);
        }

        public ParameterNode Parameters
        {
            get => GetProperty<ParameterNode>("Parameters");
            set => SetProperty("Parameters", value);
        }

        public ActionNode Action
        {
            get => GetProperty<ActionNode>("Action");
            set => SetProperty("Action", value);
        }

        public void SetName(string name) => Name = name;
        public void AddParam(Type param) => AddProperty("Parameters", param);
        public void SetAction(ActionNode actionNode) => Action = actionNode;

        public override void Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new Exception("Falta el nombre de la carta.");
            }
            if (Parameters != null)
            {
                Parameters.Validate();
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

        private void AddOnActValue(OnActValueNode onActValue) => AddProperty("OnActValues", onActValue);

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

        public string Source
        {
            get => GetProperty<string>("Source");
            private set => SetProperty("Source", value);
        }

        public bool Single
        {
            get => GetProperty<bool>("Single");
            private set => SetProperty("Single", value);
        }

        public string Predicate
        {
            get => GetProperty<string>("Predicate");
            private set => SetProperty("Predicate", value);
        }

        public void SetSource(string source) => Source = source;
        public void SetSingle(bool single) => Single = single;
        public void SetPredicate(string predicate) => Predicate = predicate;

        public override void Validate()
        {
            UnityEngine.Debug.Log("Valida");
            if (string.IsNullOrEmpty(Source))
            {
                throw new Exception("Falta el source");
            }
            if (Predicate == null)
            {
                throw new Exception("Falta el predicado");
            }
        }
    }

    public class ParameterNode : ProgramNode
    {
        public object amount
        {
            get => GetProperty<object>("Amount");
            set => SetProperty("Amount", value);
        }

        public override void Validate()
        {
            if (amount == null)
            {
                throw new Exception("Falta el amount");
            }
        }
    }

    public class PosActionNode : ProgramNode
    {

        public string Name
        {
            get => GetProperty<string>("Name");
            private set => SetProperty("Name", value);
        }
        public SelectorNode Selector
        {
            get => GetProperty<SelectorNode>("Selector");
            private set => SetProperty("Selector", value);
        }

        public void SetName(string name) => Name = name;
        public void SetSelector(SelectorNode selector) => Selector = selector;

        public override void Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new Exception("Falta el nombre de la carta.");
            }
            Selector?.Validate();
        }
    }

    public class EffectDataNode : ProgramNode
    {

        public string Name
        {
            get => GetProperty<string>("Name");
            private set => SetProperty("Name", value);
        }

        public object Amount
        {
            get => GetProperty<object>("Amount");
            private set => SetProperty("Amount", value);
        }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(Name))
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


        public override void Validate()
        {
            base.Validate();
        }
    }
}