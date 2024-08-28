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
        public abstract void SetValue(GlobalContext context, List<Cards> target, object value, object arg);
        public abstract object Evaluate(GlobalContext context, List<Cards> target);
    }

    //BinaryExpression
    public class BinaryExpressionNode : ExpressionNode
    {
        public Token Operator { get; }
        public ExpressionNode left { get; set; }
        public ExpressionNode right { get; set; }
        public override void SetValue(GlobalContext context, List<Cards> target, object value, object arg) { }
        public override void SetProperty(ExpressionNode property) { }

        public BinaryExpressionNode(ExpressionNode left, Token Operator, ExpressionNode right)
        {
            this.Operator = Operator;
            this.left = left;
            this.right = right;
        }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            return Operator.type switch
            {
                TokenType.Plus => (int)left.Evaluate(context, target) + (int)right.Evaluate(context, target),
                TokenType.Minus => (int)left.Evaluate(context, target) - (int)right.Evaluate(context, target),
                TokenType.Multiply => (int)left.Evaluate(context, target) * (int)right.Evaluate(context, target),
                TokenType.Divide => (int)left.Evaluate(context, target) / (int)right.Evaluate(context, target),
                TokenType.Exponent => (int)Math.Pow((int)left.Evaluate(context, target), (int)right.Evaluate(context, target)),
                TokenType.Equals => left.Evaluate(context, target) == right.Evaluate(context, target),
                TokenType.NotEquals => left.Evaluate(context, target) != right.Evaluate(context, target),
                TokenType.LessThan => (int)left.Evaluate(context, target) < (int)right.Evaluate(context, target),
                TokenType.GreaterThan => (int)left.Evaluate(context, target) > (int)right.Evaluate(context, target),
                TokenType.LessThanOrEqual => (int)left.Evaluate(context, target) <= (int)right.Evaluate(context, target),
                TokenType.GreaterThanOrEqual => (int)left.Evaluate(context, target) >= (int)right.Evaluate(context, target),
                TokenType.LogicalAnd => (bool)left.Evaluate(context, target) && (bool)right.Evaluate(context, target),
                TokenType.LogicalOr => (bool)left.Evaluate(context, target) || (bool)right.Evaluate(context, target),
                TokenType.Dot => (IdentifierNode)left.Evaluate(context, target),
                _ => throw new Exception("Operador no valido"),
            };

        }
    }

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
            if (property != null)
            {
                this.property.SetProperty(property);
                return;
            }
            this.property = property;
        }

        public override void SetValue(GlobalContext context, List<Cards> target, object value, object arg)
        {
            if (arg != null)
            {
                Cards card = ((List<Cards>)Evaluate(context, target))[(int)arg];
                if (property == null)
                {
                    card = (Cards)value;
                }
                else
                {
                    ((PropertyNode)property).SetCard(card);
                    property.SetValue(context, target, value, arg);
                }
            }
            else
            {
                throw new Exception("");
            }
        }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            if (player != null)
            {
                return name switch
                {
                    "DeckOfPlayer" => Context.DeckOfPlayer((Player)player.Evaluate(context, target)),
                    "HandOfPlayer" => Context.HandOfPlayer((Player)player.Evaluate(context, target)),
                    "FieldOfPlayer" => Context.FieldOfPlayer((Player)player.Evaluate(context, target)),
                    "GraveyardOfPlayer" => Context.GraveyardOfPlayer((Player)player.Evaluate(context, target)),

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

        public override void SetValue(GlobalContext context, List<Cards> target, object value, object arg)
        {
            if (arg != null)
            {
                Cards card = ((GameComponent)Evaluate(context, target)).cards[(int)arg];
                if (property == null)
                {
                    card = (Cards)value;
                }
                else
                {
                    ((PropertyNode)property).SetCard(card);
                    property.SetValue(context, target, value, arg);
                }
            }
            else
            {
                GameComponent list = (GameComponent)Evaluate(context, target);
                ((MethodCardNode)property).SetList(list);
            }
        }

        public override void SetProperty(ExpressionNode property)
        {
            if (property != null)
            {
                this.property.SetProperty(property);
                return;
            }
            this.property = property;
        }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            if (property != null)
            {
                ExpressionNode newProperty = property;
                property = null;
                ((PropertyNode)newProperty).SetCard((Cards)Evaluate(context, target));
            }
            switch (name)
            {
                case "Pop":
                    return list.Pop();
                case "Push":
                    list.Push((Cards)card.Evaluate(context, target));
                    return null;
                case "SendBottom":
                    list.SendBottom((Cards)card.Evaluate(context, target));
                    return null;
                case "Remove":
                    list.Remove((Cards)card.Evaluate(context, target));
                    return null;
                case "Find":
                    return list.Find((Predicate<Cards>)card.Evaluate(context, target));
                case "Shuffle":
                    list.Shuffle();
                    return null;

                default:
                    throw new Exception("Metodo no encontrado");
            };
        }
    }

    public class PropertyNode : ExpressionNode
    {
        public void SetCard(Cards cards)
        {

        }

        public override void SetProperty(ExpressionNode property) { }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(GlobalContext context, List<Cards> target, object value, object arg)
        {
            throw new NotImplementedException();
        }
    }

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
            if (property != null)
            {
                this.property.SetProperty(property);
                return;
            }
            this.property = property;
        }

        public override void SetValue(GlobalContext context, List<Cards> target, object value, object arg)
        {
            list.SetValue(context, target, value, (int)this.arg.Evaluate(context, target));
        }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            if (property == null)
                return ((List<Cards>)list.Evaluate(context, target))[(int)arg.Evaluate(context, target)];
            ((PropertyNode)property).SetCard(((List<Cards>)list.Evaluate(context, target))[(int)arg.Evaluate(context, target)]);
            return property.Evaluate(context, target);
        }
    }

    public class IdentifierNode : ExpressionNode
    {
        public string Name { get; }
        public Type type { get; }
        public ExpressionNode property { get; private set; } // Puede ser un MethodExpression, ListExpression, etc.

        public IdentifierNode(string name, Type type = null, ExpressionNode property = null)
        {
            this.type = type;
            Name = name;
            this.property = property;
        }

        public override void SetProperty(ExpressionNode property)
        {
            if (property != null)
            {
                this.property.SetProperty(property);
                return;
            }
            this.property = property;
        }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            // Evaluar el contexto base, p. ej., context o cualquier objeto base
            var baseValue = context.LookupSymbol(Name);

            // Evaluar la propiedad si existe
            if (property != null)
            {
                return property.Evaluate(context, target); // Esto recursivamente evalúa Hand(), [1], Power
            }

            return baseValue;
        }

        public override void SetValue(GlobalContext context, List<Cards> target, object value, object arg)
        {
            if (property != null)
            {
                property.SetValue(context, target, value, arg); // Propaga la asignación hacia la propiedad correcta
            }
            else
            {
                // Asignar directamente si es el valor final
                context.DefineSymbol(Name, value.GetType(), value);
            }
        }
    }

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
        public override void SetValue(GlobalContext context, List<Cards> target, object value, object arg) { }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            identifier.SetValue(context, target, value, null);
            return identifier.Evaluate(context, target);
        }
    }

    public class LiteralNode : ExpressionNode
    {
        public object value { get; }
        public LiteralNode(object value)
        {
            this.value = value;
        }

        public override void SetProperty(ExpressionNode property) { }
        public override void SetValue(GlobalContext context, List<Cards> target, object value, object arg) { }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            return value;
        }
    }

    public class ForNode : ExpressionNode
    {
        public List<ExpressionNode> body { get; }

        public ForNode(List<ExpressionNode> body)
        {
            this.body = body;
        }

        public override void SetProperty(ExpressionNode property) { }
        public override void SetValue(GlobalContext context, List<Cards> target, object value, object arg) { }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            foreach (var expression in body)
            {
                return expression.Evaluate(context, target);
            }
            return null;
        }
    }

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
        public override void SetValue(GlobalContext context, List<Cards> target, object value, object arg) { }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            foreach (var expression in body)
            {
                return expression.Evaluate(context, target);
            }
            return null;
        }
    }

}