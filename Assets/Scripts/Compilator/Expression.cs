using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Unity.Properties;
using Unity.VisualScripting;

namespace Console
{
    public abstract class ExpressionNode
    {
        public delegate Player FindPlayer(string name);

        public abstract void SetProperty(ExpressionNode property);
        public abstract object Evaluate(GlobalContext context, List<Cards> target, object value);
    }

    [Serializable]
    //BinaryExpression
    public class BinaryExpressionNode : ExpressionNode
    {
        [JsonProperty]
        public Token Operator { get; }
        [JsonProperty]
        public ExpressionNode left { get; set; }
        [JsonProperty]
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
                TokenType.Plus => Convert.ToInt32(left.Evaluate(context, target, value)) + Convert.ToInt32(right.Evaluate(context, target, value)),
                TokenType.Minus => Convert.ToInt32(left.Evaluate(context, target, value)) - Convert.ToInt32(right.Evaluate(context, target, value)),
                TokenType.Multiply => Convert.ToInt32(left.Evaluate(context, target, value)) * Convert.ToInt32(right.Evaluate(context, target, value)),
                TokenType.Divide => Convert.ToInt32(left.Evaluate(context, target, value)) / Convert.ToInt32(right.Evaluate(context, target, value)),
                TokenType.Exponent => Math.Pow(Convert.ToInt32(left.Evaluate(context, target, value)), Convert.ToInt32(right.Evaluate(context, target, value))),
                TokenType.Equals => left.Evaluate(context, target, value).Equals(right.Evaluate(context, target, value)),
                TokenType.NotEquals => left.Evaluate(context, target, value) != right.Evaluate(context, target, value),
                TokenType.LessThan => Convert.ToInt32(left.Evaluate(context, target, value)) < Convert.ToInt32(right.Evaluate(context, target, value)),
                TokenType.GreaterThan => Convert.ToInt32(left.Evaluate(context, target, value)) > Convert.ToInt32(right.Evaluate(context, target, value)),
                TokenType.LessThanOrEqual => Convert.ToInt32(left.Evaluate(context, target, value)) <= Convert.ToInt32(right.Evaluate(context, target, value)),
                TokenType.GreaterThanOrEqual => Convert.ToInt32(left.Evaluate(context, target, value)) >= Convert.ToInt32(right.Evaluate(context, target, value)),
                TokenType.LogicalAnd => (bool)left.Evaluate(context, target, value) && (bool)right.Evaluate(context, target, value),
                TokenType.LogicalOr => (bool)left.Evaluate(context, target, value) || (bool)right.Evaluate(context, target, value),

                _ => throw new Exception("Operador no valido"),
            };

        }
    }

    [Serializable]
    public class MethodListNode : ExpressionNode
    {
        public event FindPlayer findPlayer;
        [JsonProperty]
        public string name { get; }
        [JsonProperty]
        ExpressionNode player { get; }
        [JsonProperty]
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
                    "DeckOfPlayer" => Context.DeckOfPlayer(player.Evaluate(context, target, value)),
                    "HandOfPlayer" => Context.HandOfPlayer(player.Evaluate(context, target, value)),
                    "FieldOfPlayer" => Context.FieldOfPlayer(player.Evaluate(context, target, value)),
                    "GraveyardOfPlayer" => Context.GraveyardOfPlayer(player.Evaluate(context, target, value)),

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
        [JsonProperty]
        public string name { get; }
        [JsonProperty]
        public GameComponent list { get; set; }
        [JsonProperty]
        public ExpressionNode card { get; }
        [JsonProperty]
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
        [JsonProperty]
        public Cards card { get; private set; }
        [JsonProperty]
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
                        card.power = Convert.ToInt32(value);
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
        [JsonProperty]
        public ExpressionNode list { get; }
        [JsonProperty]
        public ExpressionNode arg { get; }
        [JsonProperty]
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
                return ((GameComponent)list.Evaluate(context, target, value)).cards[(int)arg.Evaluate(context, target, value)];
            ((PropertyNode)property).SetCard(((GameComponent)list.Evaluate(context, target, value)).cards[Convert.ToInt32(arg.Evaluate(context, target, value))]);
            return property.Evaluate(context, target, value);
        }
    }

    [Serializable]
    public class IdentifierNode : ExpressionNode
    {
        [JsonProperty]
        public string Name { get; }
        [JsonProperty]
        public Type type { get; }
        [JsonProperty]
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
                    ((PropertyNode)property).SetCard((Cards)context.LookupVariable(Name));
                return property.Evaluate(context, target, value);
            }
            if (value != null)
            {
                context.DefineVariable(Name, value);
            }
            return context.LookupVariable(Name);
        }
    }

    [Serializable]
    public class AssignamentNode : ExpressionNode
    {
        [JsonProperty]
        public ExpressionNode identifier { get; }
        [JsonProperty]
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
        [JsonProperty]
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
    public class PredicateNode : ExpressionNode
    {
        [JsonProperty]
        public ExpressionNode condition { get; }
        public PredicateNode(ExpressionNode condition)
        {
            this.condition = condition;
        }
        public override void SetProperty(ExpressionNode property) { }

        public override object Evaluate(GlobalContext context, List<Cards> target, object value)
        {
            List<Cards> cards = new();
            foreach (var card in target)
            {
                context.DefineVariable("unit", card);
                if ((bool)condition.Evaluate(context, target, value))
                {
                    cards.Add(card);
                }
            }
            return cards;
        }
    }


    [Serializable]
    public class ForNode : ExpressionNode
    {
        [JsonProperty]
        public List<ExpressionNode> body { get; }

        public ForNode(List<ExpressionNode> body)
        {
            this.body = body;
        }

        public override void SetProperty(ExpressionNode property) { }

        public override object Evaluate(GlobalContext context, List<Cards> target, object value)
        {
            foreach (var card in target)
            {
                context.DefineVariable("target", card);
                foreach (var expression in body)
                {
                    expression.Evaluate(context, target, value);
                }
            }
            return null;
        }
    }

    [Serializable]
    public class WhileNode : ExpressionNode
    {
        [JsonProperty]
        public ExpressionNode condition { get; }
        [JsonProperty]
        public List<ExpressionNode> body { get; }

        public WhileNode(ExpressionNode condition, List<ExpressionNode> body)
        {
            this.condition = condition;
            this.body = body;
        }

        public override void SetProperty(ExpressionNode property) { }

        public override object Evaluate(GlobalContext context, List<Cards> target, object value)
        {
            while ((bool)condition.Evaluate(context, target, value))
            {
                foreach (var expression in body)
                {
                    return expression.Evaluate(context, target, value);
                }
            }
            return null;
        }
    }

}