using System.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Diagnostics;

namespace Console
{
    public interface ASTNode
    {
    }

    public class ProgramNode : ASTNode
    {
        public virtual void SetName(string value) { }
        public virtual void SetType(string value) { }
        public virtual void SetFaction(string value) { }
        public virtual void AddRange(string value) { }
        public virtual void SetInt(int value) { }
        public virtual void SetOnActivation(OnActivationNode value) { }
        public virtual void SetSelector(SelectorNode value) { }
        public virtual void SetPosAction(PosActionNode value) { }
        public virtual void SetEffectDataNode(EffectDataNode value) { }
        public virtual void SetSource(string value) { }
        public virtual void SetSingle(bool value) { }
        public virtual void SetPredicate(string value) { }
        public virtual void AddOnActValue(OnActValueNode value) { }

        public virtual void Validate() { }
    }

    public class CardNode : ProgramNode
    {
        public string name { get; private set; }
        public string type { get; private set; }
        public string faction { get; private set; }
        public List<string> range { get; private set; } = new List<string>();
        public int power { get; private set; }
        public OnActivationNode onActivation { get; private set; }


        public override void SetName(string name)
        {
            if (this.name != null)
            {
                throw new Exception("El nombre ya está definido.");
            }
            this.name = name;
        }

        public override void SetType(string type)
        {
            if (this.type != null)
            {
                throw new Exception("El tipo ya está definido.");
            }
            this.type = type;
        }

        public override void SetFaction(string faction)
        {
            if (this.faction != null)
            {
                throw new Exception("La facción ya está definida.");
            }
            this.faction = faction;
        }

        public override void SetInt(int power)
        {
            if (this.power != 0)
            {
                throw new Exception("El poder ya está definido.");
            }
            this.power = power;
        }

        public override void AddRange(string range)
        {
            this.range.Add(range);
        }

        public override void SetOnActivation(OnActivationNode onActivation)
        {
            if (this.onActivation != null)
            {
                throw new Exception("El nodo de activación ya está definido.");
            }
            this.onActivation = onActivation;
        }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Falta el nombre de la carta.");
            }
            if (string.IsNullOrEmpty(type))
            {
                throw new Exception("Falta el tipo de carta.");
            }
            if (string.IsNullOrEmpty(faction))
            {
                throw new Exception("Falta la facción de la carta.");
            }
            if (onActivation == null)
            {
                throw new Exception("Falta el nodo de activación.");
            }
        }
    }

    public class EffectNode : ProgramNode
    {
        public string name { get; private set; }
        protected List<Type> parameters { get; private set; }
        public void Action(List<Cards> targets, Context context) { }

        public override void Validate()
        {
            base.Validate();
        }

    }

    public class OnActivationNode : ProgramNode
    {
        public List<OnActValueNode> values { get; private set; }

        public OnActivationNode()
        {
            values = new List<OnActValueNode>();
        }

        public override void AddOnActValue(OnActValueNode value)
        {
            values.Add(value);
        }

        public override void Validate()
        {
            foreach (OnActValueNode value in values)
            {
                value.Validate();
            }
        }
    }

    public class SelectorNode : ProgramNode
    {
        public string source { get; private set; }
        public bool single { get; private set; }
        public string predicate { get; private set; }

        public override void SetSource(string value)
        {
            if (source != null)
            {
                throw new Exception("El tipo ya está definido.");
            }
            source = value;
        }

        public override void SetSingle(bool value)
        {
            single = value;
        }

        public override void SetPredicate(string value)
        {
            if (predicate != null)
            {
                throw new Exception("El tipo ya está definido.");
            }
            predicate = value;
        }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new Exception("Falta el source");
            }
            if (string.IsNullOrEmpty(predicate))
            {
                throw new Exception("Falta el predicado");
            }
        }
    }

    public class PosActionNode : ProgramNode
    {
        public string name { get; set; }
        public SelectorNode selectorNode { get; set; }

        public override void SetName(string value)
        {
            if (name != null)
            {
                throw new Exception("El nombre ya esta definido");
            }
            name = value;
        }

        public override void SetSelector(SelectorNode value)
        {
            if (selectorNode != null)
            {
                throw new Exception("El nodo del seleccionador ya esta definido");
            }
            selectorNode = value;
        }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Falta el nombre de la carta.");
            }
        }
    }

    public class EffectDataNode : ProgramNode
    {
        public string name { get; private set; }
        public int amount { get; private set; }

        public override void SetName(string value)
        {
            if (name != null)
            {
                throw new Exception("El nombre ya esta definido");
            }
            name = value;
        }

        public override void SetInt(int value)
        {
            amount = value;
        }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Falta el nombre");
            }
        }
    }

    public class OnActValueNode : ProgramNode
    {
        public SelectorNode selectorNode { get; private set; }
        public PosActionNode posActionNode { get; private set; }
        public EffectDataNode effectDataNode { get; private set; }

        public override void SetSelector(SelectorNode value)
        {
            if (selectorNode != null)
            {
                throw new Exception("El nodo del seleccionador ya esta definido");
            }
            selectorNode = value;
        }

        public override void SetPosAction(PosActionNode value)
        {
            if (posActionNode != null)
            {
                throw new Exception("El nodo de la accion posterior ya esta definido");
            }
            posActionNode = value;
        }

        public override void SetEffectDataNode(EffectDataNode value)
        {
            UnityEngine.Debug.Log("Paso");
            if (effectDataNode != null)
            {
                throw new Exception("El nodo de la informacion del efecto ya esta denfinido");
            }
            effectDataNode = value;
        }

        public override void Validate()
        {
            UnityEngine.Debug.Log("Valida");
            selectorNode?.Validate();
            effectDataNode.Validate();
            posActionNode?.Validate();
        }
    }
}