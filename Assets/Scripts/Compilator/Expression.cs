using System;
using System.Collections.Generic;
using System.Diagnostics;
using Gwent_Proyect.Assets.Scripts.Compilator;
using Unity.VisualScripting;

namespace Console
{

    public abstract class ExpressionNode
    {
        public abstract object Evaluate(Context context, List<Cards> target);
    }

    //BinaryExpression
    public class BinaryExpressionNode : ExpressionNode
    {
        public Token Operator { get; }
        public ExpressionNode left { get; set; }
        public ExpressionNode right { get; set; }

        public BinaryExpressionNode(ExpressionNode left, Token Operator, ExpressionNode right)
        {
            this.Operator = Operator;
            this.left = left;
            this.right = right;
        }

        public override object Evaluate(Context context, List<Cards> target)
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
        public override object Evaluate(Context context, List<Cards> targets)
        {
            return context.board;
        }
    }

    public class DeckList : ExpressionNode
    {
        ExpressionNode player { get; }
        public DeckList(ExpressionNode player = null)
        {
            this.player = player;
        }

        public override object Evaluate(Context context, List<Cards> target)
        {
            if (player != null)
            {
                return context.DeckOfPlayer((Player)player.Evaluate(context, target));
            }
            return context.Deck;
        }
    }

    public class HandList : ExpressionNode
    {
        ExpressionNode player { get; }
        public HandList(ExpressionNode player = null)
        {
            this.player = player;
        }

        public override object Evaluate(Context context, List<Cards> target)
        {
            if (player != null)
            {
                return context.HandOfPlayer((Player)player.Evaluate(context, target));
            }
            return context.Hand;
        }
    }

    public class FieldList : ExpressionNode
    {
        ExpressionNode player { get; }
        public FieldList(ExpressionNode player = null)
        {
            this.player = player;
        }

        public override object Evaluate(Context context, List<Cards> target)
        {
            if (player != null)
            {
                return context.FieldOfPlayer((Player)player.Evaluate(context, target));
            }
            return context.Field;
        }
    }

    public class GraveyardList : ExpressionNode
    {
        ExpressionNode player { get; }
        public GraveyardList(ExpressionNode player = null)
        {
            this.player = player;
        }

        public override object Evaluate(Context context, List<Cards> target)
        {
            if (player != null)
            {
                return context.GraveyardOfPlayer((Player)player.Evaluate(context, target));
            }
            return context.Graveyard;
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

        public override object Evaluate(Context context, List<Cards> target)
        {
            return ((List<Cards>)list.Evaluate(context, target))[(int)arg.Evaluate(context, target)];
        }
    }

    public class LiteralNode : ExpressionNode
    {
        public object value { get; }
        public override void SetProperty(ExpressionNode property) { }

        public LiteralNode(object value)
        {
            this.value = value;
        }

        public override object Evaluate(Context context, List<Cards> target)
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

        public override object Evaluate(Context context, List<Cards> target)
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
        public override void SetProperty(ExpressionNode property) { }

        public WhileNode(ExpressionNode condition, List<ExpressionNode> body)
        {
            this.condition = condition;
            this.body = body;
        }

        public override object Evaluate(Context context, List<Cards> target)
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
        public string value { get; }
        public string type { get; }
        public ExpressionNode property { get; private set; }

        public IdentifierNode(string value, string type)
        {
            this.value = value;
            this.type = type;
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

        // public override void SetValue(GlobalContext context, List<Cards> targets, object value)
        // {

        // }

        public override object Evaluate(Context context, List<Cards> target)
        {
            if (property == null)
                return property.Evaluate(context, target);
            return value;
        }
    }

    public class AssignamentNode : ExpressionNode
    {
        public ExpressionNode identifier { get; }
        public ExpressionNode value { get; private set; }
        public override void SetProperty(ExpressionNode property) { }

        public AssignamentNode(ExpressionNode identifier, ExpressionNode value)
        {
            this.identifier = identifier;
            this.value = value;
        }

        public override object Evaluate(Context context, List<Cards> target)
        {
            identifier.SetValue(context, target, value.Evaluate(context, target));
            return val;
        }

    }
}