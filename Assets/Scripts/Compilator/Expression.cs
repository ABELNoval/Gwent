using System;

namespace Console
{
    public abstract class ExpressionNode
    {
        public abstract int Evaluate();
    }

    public class BinaryExpressionNode : ExpressionNode
    {
        public Token Operator { get; }
        public ExpressionNode left { get; set; }
        public ExpressionNode right { get; set; }

        public BinaryExpressionNode(ExpressionNode left, Token Operator, ExpressionNode rigth)
        {
            this.Operator = Operator;
            this.left = left;
            this.right = rigth;
        }

        public override int Evaluate()
        {
            switch (Operator.type)
            {
                case TokenType.Plus:
                    return left.Evaluate() + right.Evaluate();

                case TokenType.Minus:
                    return left.Evaluate() - right.Evaluate();

                case TokenType.Multiply:
                    return left.Evaluate() * right.Evaluate();

                case TokenType.Divide:
                    return left.Evaluate() / right.Evaluate();

                case TokenType.Exponent:
                    return (int)Math.Pow(left.Evaluate(), right.Evaluate());

                default:
                    throw new Exception("Operador no valido");
            }
        }
    }

    public class NumberNode : ExpressionNode
    {
        public int value { get; }

        public NumberNode(int value)
        {
            this.value = value;
        }

        public override int Evaluate()
        {
            return value;
        }
    }
}