using System;
using System.Collections.Generic;
using System.Diagnostics;
using Gwent_Proyect.Assets.Scripts.Compilator;
using Unity.VisualScripting;

namespace Console
{

    public abstract class ExpressionNode
    {
        public abstract void SetValue(GlobalContext context, List<Cards> target, object value, int arg);
        public abstract object Evaluate(GlobalContext context, List<Cards> target);
    }

    //BinaryExpression
    public class BinaryExpressionNode : ExpressionNode
    {
        public Token Operator { get; }
        public ExpressionNode left { get; set; }
        public ExpressionNode right { get; set; }
        public override void SetValue(GlobalContext context, List<Cards> target, object value, int arg) { }

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


    public class BoardList : ExpressionNode
    {
        public override void SetValue(GlobalContext context, List<Cards> target, object value, int arg) { }
        public override object Evaluate(GlobalContext context, List<Cards> targets)
        {
            return Context.board;
        }
    }

    public class DeckList : ExpressionNode
    {
        ExpressionNode player { get; }
        public DeckList(ExpressionNode player = null)
        {
            this.player = player;
        }
        public override void SetValue(GlobalContext context, List<Cards> target, object value, int arg) { }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            if (player != null)
            {
                return Context.DeckOfPlayer((Player)player.Evaluate(context, target));
            }
            return Context.Deck;
        }
    }

    public class HandList : ExpressionNode
    {
        ExpressionNode player { get; }
        public HandList(ExpressionNode player = null)
        {
            this.player = player;
        }
        public override void SetValue(GlobalContext context, List<Cards> target, object value, int arg) { }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            if (player != null)
            {
                return Context.HandOfPlayer((Player)player.Evaluate(context, target));
            }
            return Context.Hand;
        }
    }

    public class FieldList : ExpressionNode
    {
        ExpressionNode player { get; }
        public FieldList(ExpressionNode player = null)
        {
            this.player = player;
        }
        public override void SetValue(GlobalContext context, List<Cards> target, object value, int arg) { }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            if (player != null)
            {
                return Context.FieldOfPlayer((Player)player.Evaluate(context, target));
            }
            return Context.Field;
        }
    }

    public class GraveyardList : ExpressionNode
    {
        ExpressionNode player { get; }
        public GraveyardList(ExpressionNode player = null)
        {
            this.player = player;
        }
        public override void SetValue(GlobalContext context, List<Cards> target, object value, int arg) { }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            if (player != null)
            {
                return Context.GraveyardOfPlayer((Player)player.Evaluate(context, target));
            }
            return Context.Graveyard;
        }
    }

    public class ListNode : ExpressionNode
    {
        public ExpressionNode list { get; }
        public ExpressionNode arg { get; }

        public ListNode(ExpressionNode list, ExpressionNode arg)
        {
            this.arg = arg;
            this.list = list;
        }

        public override void SetValue(GlobalContext context, List<Cards> target, object value, int arg)
        {
            list.SetValue(context, target, value, (int)this.arg.Evaluate(context, target));
        }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            return ((List<Cards>)list.Evaluate(context, target))[(int)arg.Evaluate(context, target)];
        }
    }

    public class LiteralNode : ExpressionNode
    {
        public object value { get; }
        public LiteralNode(object value)
        {
            this.value = value;
        }
        public override void SetValue(GlobalContext context, List<Cards> target, object value, int arg) { }

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
        public override void SetValue(GlobalContext context, List<Cards> target, object value, int arg) { }

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
        public override void SetValue(GlobalContext context, List<Cards> target, object value, int arg) { }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            foreach (var expression in body)
            {
                return expression.Evaluate(context, target);
            }
            return null;
        }
    }

    public class IdentifierNode : ExpressionNode
    {
        public string Name { get; }
        public ExpressionNode Property { get; private set; } // Puede ser un MethodExpression, ListExpression, etc.

        public IdentifierNode(string name, ExpressionNode property = null)
        {
            Name = name;
            Property = property;
        }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            // Evaluar el contexto base, p. ej., context o cualquier objeto base
            var baseValue = context.LookupSymbol(Name);

            // Evaluar la propiedad si existe
            if (Property != null)
            {
                return Property.Evaluate(context, target); // Esto recursivamente evalúa Hand(), [1], Power
            }

            return baseValue;
        }

        public override void SetValue(GlobalContext context, List<Cards> target, object value, int arg)
        {
            if (Property != null)
            {
                Property.SetValue(context, target, value, arg); // Propaga la asignación hacia la propiedad correcta
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
        public override void SetValue(GlobalContext context, List<Cards> target, object value, int arg) { }

        public override object Evaluate(GlobalContext context, List<Cards> target)
        {
            return null;
        }
    }
}