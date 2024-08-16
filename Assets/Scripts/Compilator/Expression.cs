using System;
using System.Collections.Generic;

namespace Console
{

    public abstract class ExpressionNode
    {
        public abstract object Evaluate();
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

        public override object Evaluate()
        {
            return Operator.type switch
            {
                TokenType.Plus => (int)left.Evaluate() + (int)right.Evaluate(),
                TokenType.Minus => (int)left.Evaluate() - (int)right.Evaluate(),
                TokenType.Multiply => (int)left.Evaluate() * (int)right.Evaluate(),
                TokenType.Divide => (int)left.Evaluate() / (int)right.Evaluate(),
                TokenType.Exponent => (int)Math.Pow((int)left.Evaluate(), (int)right.Evaluate()),
                TokenType.Equals => (bool)left.Evaluate() == (bool)right.Evaluate(),
                TokenType.NotEquals => (bool)left.Evaluate() != (bool)right.Evaluate(),
                TokenType.LessThan => (int)left.Evaluate() < (int)right.Evaluate(),
                TokenType.GreaterThan => (int)left.Evaluate() > (int)right.Evaluate(),
                TokenType.LessThanOrEqual => (int)left.Evaluate() <= (int)right.Evaluate(),
                TokenType.GreaterThanOrEqual => (int)left.Evaluate() >= (int)right.Evaluate(),

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

        public override object Evaluate()
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

        public override object Evaluate()
        {
            foreach (var expression in body)
            {
                return expression.Evaluate();
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

        public override object Evaluate()
        {
            foreach (var expression in body)
            {
                return expression.Evaluate();
            }
            return null;
        }
    }

    public class IdentifierNode : ExpressionNode
    {
        public string value { get; }
        public List<ExpressionNode> properties { get; }

        public IdentifierNode(string value)
        {
            this.value = value;
        }

        public void AddProperty(ExpressionNode expression)
        {
            properties.Add(expression);
        }

        public override object Evaluate()
        {
            throw new NotImplementedException();
        }

    }

    public class AssignamentNode : ExpressionNode
    {
        public string identifier { get; }
        public ExpressionNode value { get; }

        public AssignamentNode(string identifier, ExpressionNode value)
        {
            this.identifier = identifier;
            this.value = value;
        }

        public override object Evaluate()
        {
            return value.Evaluate();
        }

    }
}