using System;
using System.Collections.Generic;
using System.Diagnostics;
using Gwent_Proyect.Assets.Scripts.Compilator;
using Unity.Properties;
using Unity.VisualScripting;

namespace Console
{
    public abstract class ExpressionNode
    {
        public abstract void SetProperty(ExpressionNode property);
        public abstract object Evaluate(GlobalContext context, List<Cards> target, object value);
    }

    [Serializable]
    //BinaryExpression
    public class BinaryExpressionNode : ExpressionNode
    {
        public Token Operator { get; }
        public ExpressionNode left { get; set; }
        public ExpressionNode right { get; set; }
        public override void SetProperty(ExpressionNode property) { }

        public BinaryExpressionNode(ExpressionNode left, Token Operator, ExpressionNode right)
        {
            this.Operator = Operator;
            this.left = left;
            this.right = right;
        }

        public override object Evaluate(GlobalContext context, List<Cards> target, object value)
        {
            return Operator.type switch
            {
                TokenType.Plus => (int)left.Evaluate(context, target, value) + (int)right.Evaluate(context, target, value),
                TokenType.Minus => (int)left.Evaluate(context, target, value) - (int)right.Evaluate(context, target, value),
                TokenType.Multiply => (int)left.Evaluate(context, target, value) * (int)right.Evaluate(context, target, value),
                TokenType.Divide => (int)left.Evaluate(context, target, value) / (int)right.Evaluate(context, target, value),
                TokenType.Exponent => (int)Math.Pow((int)left.Evaluate(context, target, value), (int)right.Evaluate(context, target, value)),
                TokenType.Equals => left.Evaluate(context, target, value) == right.Evaluate(context, target, value),
                TokenType.NotEquals => left.Evaluate(context, target, value) != right.Evaluate(context, target, value),
                TokenType.LessThan => (int)left.Evaluate(context, target, value) < (int)right.Evaluate(context, target, value),
                TokenType.GreaterThan => (int)left.Evaluate(context, target, value) > (int)right.Evaluate(context, target, value),
                TokenType.LessThanOrEqual => (int)left.Evaluate(context, target, value) <= (int)right.Evaluate(context, target, value),
                TokenType.GreaterThanOrEqual => (int)left.Evaluate(context, target, value) >= (int)right.Evaluate(context, target, value),
                TokenType.LogicalAnd => (bool)left.Evaluate(context, target, value) && (bool)right.Evaluate(context, target, value),
                TokenType.LogicalOr => (bool)left.Evaluate(context, target, value) || (bool)right.Evaluate(context, target, value),

                _ => throw new Exception("Operador no valido"),
            };

        }
    }

    [Serializable]
    public class MethodListNode : ExpressionNode
    {
        public string name { get; }
        ExpressionNode player { get; }
        public ExpressionNode property { get; private set; }

        public MethodListNode(string name, ExpressionNode player = null)
        {
            this.name = name;
            this.player = player;
        }

        public override void SetProperty(ExpressionNode property)
        {
            if (this.property != null)
            {
                this.property.SetProperty(property);
                return;
            }
            this.property = property;
        }

        public override object Evaluate(GlobalContext context, List<Cards> target, object value)
        {
            if (property != null)
            {
                ExpressionNode newProperty = property;
                property = null;
                ((MethodCardNode)newProperty).SetList((GameComponent)Evaluate(context, target, value));
                return newProperty.Evaluate(context, target, value);
            }
            else if (player != null)
            {
                return name switch
                {
                    "DeckOfPlayer" => Context.DeckOfPlayer((Player)player.Evaluate(context, target, value)),
                    "HandOfPlayer" => Context.HandOfPlayer((Player)player.Evaluate(context, target, value)),
                    "FieldOfPlayer" => Context.FieldOfPlayer((Player)player.Evaluate(context, target, value)),
                    "GraveyardOfPlayer" => Context.GraveyardOfPlayer((Player)player.Evaluate(context, target, value)),

                    _ => throw new Exception("Metodo no encontrado"),
                };
            }
            else
            {
                return name switch
                {
                    "Deck" => Context.Deck,
                    "Hand" => Context.Hand,
                    "Field" => Context.Field,
                    "Graveyard" => Context.Graveyard,
                    "Board" => Context.board,

                    _ => throw new Exception("Metodo no encontrado"),
                };
            }
        }
    }

    [Serializable]
    public class MethodCardNode : ExpressionNode
    {
        public string name { get; }
        public GameComponent list { get; set; }
        public ExpressionNode card { get; }
        public ExpressionNode property { get; private set; }
        public MethodCardNode(string name, ExpressionNode card = null)
        {
            this.name = name;
            this.card = card;
        }

        public void SetList(GameComponent list)
        {
            this.list = list;
        }

        public override void SetProperty(ExpressionNode property)
        {
            if (this.property != null)
            {
                this.property.SetProperty(property);
                return;
            }
            this.property = property;
        }

        public override object Evaluate(GlobalContext context, List<Cards> target, object value)
        {
            if (property != null)
            {
                ExpressionNode newProperty = property;
                property = null;
                ((PropertyNode)newProperty).SetCard((Cards)Evaluate(context, target, value));
                return newProperty.Evaluate(context, target, value);
            }
            switch (name)
            {
                case "Pop":
                    return list.Pop();
                case "Push":
                    list.Push((Cards)card.Evaluate(context, target, value));
                    return null;
                case "SendBottom":
                    list.SendBottom((Cards)card.Evaluate(context, target, value));
                    return null;
                case "Remove":
                    list.Remove((Cards)card.Evaluate(context, target, value));
                    return null;
                case "Find":
                    return list.Find((Predicate<Cards>)card.Evaluate(context, target, value));
                case "Shuffle":
                    list.Shuffle();
                    return null;

                default:
                    throw new Exception("Metodo no encontrado");
            };
        }
    }

    [Serializable]
    public class PropertyNode : ExpressionNode
    {
        public Cards card { get; private set; }
        public string Name { get; }

        public PropertyNode(string name)
        {
            Name = name;
        }

        public void SetCard(Cards card)
        {
            this.card = card;
        }

        public override void SetProperty(ExpressionNode property) { }

        public override object Evaluate(GlobalContext context, List<Cards> target, object value)
        {
            switch (Name)
            {
                case "Power":
                    if (value != null)
                        card.power = (int)value;
                    return card.power;
                case "Faction":
                    if (value != null)
                        card.faction = (string)value;
                    return card.faction;
                case "Type":
                    if (value != null)
                        card.type = (string)value;
                    return card.type;
                case "Range":
                    if (value != null)
                        card.range = (List<string>)value;
                    return card.range;
                case "Name":
                    if (value != null)
                        card.name = (string)value;
                    return card.name;
                case "Owner":
                    if (value != null)
                        card.owner = (Guid)value;
                    return card.owner;
                default:
                    throw new Exception("Propiedad no encontrado");
            };
        }
    }

    [Serializable]
    public class ListNode : ExpressionNode
    {
        public ExpressionNode list { get; }
        public ExpressionNode arg { get; }
        public ExpressionNode property { get; private set; }

        public ListNode(ExpressionNode list, ExpressionNode arg)
        {
            this.arg = arg;
            this.list = list;
        }

        public override void SetProperty(ExpressionNode property)
        {
            if (this.property != null)
            {
                this.property.SetProperty(property);
                return;
            }
            this.property = property;
        }

        public override object Evaluate(GlobalContext context, List<Cards> target, object value)
        {
            if (property == null)
                return ((List<Cards>)list.Evaluate(context, target, value))[(int)arg.Evaluate(context, target, value)];
            ((PropertyNode)property).SetCard(((List<Cards>)list.Evaluate(context, target, value))[(int)arg.Evaluate(context, target, value)]);
            return property.Evaluate(context, target, value);
        }
    }

    [Serializable]
    public class IdentifierNode : ExpressionNode
    {
        public string Name { get; }
        public Type type { get; }
        public ExpressionNode property { get; private set; }

        public IdentifierNode(string name, Type type = null)
        {
            this.type = type;
            Name = name;
        }

        public override void SetProperty(ExpressionNode property)
        {
            if (this.property != null)
            {
                this.property.SetProperty(property);
                return;
            }
            this.property = property;
        }

        public override object Evaluate(GlobalContext context, List<Cards> target, object value)
        {
            if (property != null)
            {
                if (property is PropertyNode)
                    ((PropertyNode)property).SetCard((Cards)context.LookupSymbol(Name).Item2);
                return property.Evaluate(context, target, value);
            }
            return context.LookupSymbol(Name);
        }
    }

    [Serializable]
    public class AssignamentNode : ExpressionNode
    {
        public ExpressionNode identifier { get; }
        public ExpressionNode value { get; private set; }

        public AssignamentNode(ExpressionNode identifier, ExpressionNode value)
        {
            this.identifier = identifier;
            this.value = value;
        }

        public override void SetProperty(ExpressionNode property) { }

        public override object Evaluate(GlobalContext context, List<Cards> target, object value)
        {
            return identifier.Evaluate(context, target, this.value.Evaluate(context, target, value));
        }
    }

    [Serializable]
    public class LiteralNode : ExpressionNode
    {
        public object value { get; }
        public LiteralNode(object value)
        {
            this.value = value;
        }

        public override void SetProperty(ExpressionNode property) { }
        public override object Evaluate(GlobalContext context, List<Cards> target, object value)
        {
            return this.value;
        }
    }

    [Serializable]
    public class ForNode : ExpressionNode
    {
        public List<ExpressionNode> body { get; }

        public ForNode(List<ExpressionNode> body)
        {
            this.body = body;
        }

        public override void SetProperty(ExpressionNode property) { }

        public override object Evaluate(GlobalContext context, List<Cards> target, object value)
        {
            foreach (var expression in body)
            {
                return expression.Evaluate(context, target, value);
            }
            return null;
        }
    }

    [Serializable]
    public class WhileNode : ExpressionNode
    {
        public ExpressionNode condition { get; }
        public List<ExpressionNode> body { get; }

        public WhileNode(ExpressionNode condition, List<ExpressionNode> body)
        {
            this.condition = condition;
            this.body = body;
        }

        public override void SetProperty(ExpressionNode property) { }

        public override object Evaluate(GlobalContext context, List<Cards> target, object value)
        {
            foreach (var expression in body)
            {
                return expression.Evaluate(context, target, value);
            }
            return null;
        }
    }

}