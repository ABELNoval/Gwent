using System;
using System.Collections.Generic;

namespace Console
{

    public abstract class ExpressionNode
    {
        public abstract object Evaluate(Context context, List<Cards> target);
    }

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
                TokenType.Equals => (bool)left.Evaluate(context, target) == (bool)right.Evaluate(context, target),
                TokenType.NotEquals => (bool)left.Evaluate(context, target) != (bool)right.Evaluate(context, target),
                TokenType.LessThan => (int)left.Evaluate(context, target) < (int)right.Evaluate(context, target),
                TokenType.GreaterThan => (int)left.Evaluate(context, target) > (int)right.Evaluate(context, target),
                TokenType.LessThanOrEqual => (int)left.Evaluate(context, target) <= (int)right.Evaluate(context, target),
                TokenType.GreaterThanOrEqual => (int)left.Evaluate(context, target) >= (int)right.Evaluate(context, target),

                _ => throw new Exception("Operador no valido"),
            };

        }
    }

    public class LiteralNode : ExpressionNode
    {
        public object value { get; }

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
        public List<ExpressionNode> properties { get; }

        public IdentifierNode(string value, string type)
        {
            this.value = value;
            this.type = type;
        }

        public void AddProperty(ExpressionNode expression)
        {
            properties.Add(expression);
        }

        public override object Evaluate(Context context, List<Cards> target)
        {
            throw new NotImplementedException();
        }

    }

    public class AssignamentNode : ExpressionNode
    {
        public string identifier { get; }
        public ExpressionNode value { get; private set; }

        public AssignamentNode(string identifier)
        {
            this.identifier = identifier;
        }

        public void SetValue(ExpressionNode value)
        {
            this.value = value;
        }

        public override object Evaluate(Context context, List<Cards> target)
        {
            return value.Evaluate(context, target);
        }

    }

    public class MethodAccessNode : ExpressionNode
    {
        public string identifier { get; }
        public List<ExpressionNode> parameters { get; }

        public MethodAccessNode(string identifier)
        {
            this.identifier = identifier;
        }

        public void AddParameter(ExpressionNode parameter)
        {
            parameters.Add(parameter);
        }

        public override object Evaluate(Context context, List<Cards> target)
        {
            throw new NotImplementedException();
        }
    }

    public class ListNode : ExpressionNode
    {
        public string identifier { get; }
        public List<ExpressionNode> members { get; }

        public ListNode(string identifier)
        {
            this.identifier = identifier;
        }

        public void AddMember(ExpressionNode member)
        {
            members.Add(member);
        }

        public override object Evaluate(Context context, List<Cards> target)
        {
            throw new NotImplementedException();
        }
    }
}