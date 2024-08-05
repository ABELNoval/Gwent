using System;
using System.Collections.Generic;

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
        public virtual void SetPower(int value) { }
        public virtual void SetOnActivation(OnActivationNode value) { }
        public virtual void SetSelector(string value) { }
        public virtual void SetSource(string value) { }
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

        public override void SetPower(int power)
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
            /*if (onActivation == null)
            {
                throw new Exception("Falta el nodo de activación.");
            }*/
        }
    }

    public class EffectNode : ProgramNode
    {
        public string name { get; private set; }
        protected List<Type> parameters { get; private set; }
        public void Action(List<Cards> targets, Context context) { }

    }

    public class OnActivationNode : ProgramNode
    {
        public SelectorNode selector { get; set; }
        public EffectNode effect { get; set; }
        public PosActionNode posAction { get; set; }
    }

    public class SelectorNode : ProgramNode
    {

    }

    public class PosActionNode : ProgramNode
    {

    }
}