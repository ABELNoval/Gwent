using System;
using System.Collections.Generic;

namespace Console
{
    public abstract class Expression
    {
        public abstract object Evaluate(Dictionary<string, object> variables);
    }

    public class BinaryExpression : Expression
    {
        private Token _operator { get; }
        private Expression _left { get; }
        private Expression _right { get; }

        public BinaryExpression(Token _operator, Expression _left, Expression _rigth)
        {
            this._operator = _operator;
            this._left = _left;
            this._right = _rigth;
        }

        public override object Evaluate(Dictionary<string, object> variables)
        {
            var leftValue = _left.Evaluate(variables);
            var rightValue = _right.Evaluate(variables);
            if (leftValue.GetType() == rightValue.GetType())
            {
                /* switch (_operator.typeOfToken)
                 {
                     case TypeOfToken.plus_Token:
                         return leftValue + rightValue;
                     case TypeOfToken.minus_Token:
                         return leftValue - rightValue;
                     case TypeOfToken.multiply_Token:
                         return leftValue * rightValue;
                     case TypeOfToken.divide_Token:
                         return leftValue / rightValue;
                     default:
                         throw new Exception("Operador desconocido");
                 }*/
            }
            throw new Exception("No son del mismo tipo");
        }
    }

    public class LiteralExpression : Expression
    {
        public object _value { get; }

        public LiteralExpression(object _value)
        {
            this._value = _value;
        }

        public override object Evaluate(Dictionary<string, object> variables)
        {
            return _value;
        }
    }

    //Expresion de una variable
    public class VariableExpression : Expression
    {
        public Token _name { get; }

        public VariableExpression(Token _name)
        {
            this._name = _name;
        }

        public override object Evaluate(Dictionary<string, object> variables)
        {
            if (variables.TryGetValue(_name.value, out var value))
            {
                return value;
            }

            throw new Exception($"Variable no declarada: {_name.value}");
        }
    }

    // Expresion de asignacion
    public class AssignmentExpression : Expression
    {
        public Token _name { get; }
        public Expression _value { get; }

        public AssignmentExpression(Token _name, Expression _value)
        {
            this._name = _name;
            this._value = _value;
        }

        public override object Evaluate(Dictionary<string, object> variables)
        {
            var value = _value.Evaluate(variables);
            variables[_name.value] = value;
            return value;
        }
    }

}