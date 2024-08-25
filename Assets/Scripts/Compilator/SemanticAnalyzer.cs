using System;
using System.Collections.Generic;
using Gwent_Proyect.Assets.Scripts.Compilator;

namespace Console
{
    public class SemanticAnalyzer
    {

        private readonly Dictionary<string, Type> expectedPropertyTypes = new Dictionary<string, Type>
        {
            { "Name", typeof(string) },
            { "Faction", typeof(string) },
            { "Power", typeof(int) },
            { "Range", typeof(string) },
            { "Melee", typeof(string) },
            { "Siege", typeof(string) },
            { "Type", typeof(string) },
            { "Source", typeof(int) },
            {"Single", typeof(bool)},
            {"Predicate", typeof(string)}
        };

        public ProgramNode Analyze(ProgramNode node)
        {
            GlobalContext context = new();
            switch (node)
            {
                case CardNode:
                    return CheckCardNode(node as CardNode, context);
                case EffectNode:
                    return CheckEffectNode(node as EffectNode, context);
                case OnActValueNode:
                    return CheckOnActValueNode(node as OnActValueNode, context);
                case SelectorNode:
                    return CheckSelectorNode(node as SelectorNode, context);
                case EffectDataNode:
                    return CheckEffectDataNode(node, context);

                default:
                    throw new ArgumentException("Invalid node");
            }
        }

        private CardNode CheckCardNode(CardNode node, GlobalContext context)
        {
            foreach (var property in node.properties)
            {
                if (property.Key == "OnActivation")
                {
                    node.SetProperty(property.Key, Analyze(property.Value as OnActivationNode));
                }
                Type exprectedType = GetExpectedTypeForProperty(property.Key);
                Type expType = CheckExpression(node.GetProperty<ExpressionNode>(property.Key), context);
                if (expType != exprectedType)
                    throw new Exception("No son del mismo tipo");

                object value = (property.Value as ExpressionNode).Evaluate(context, null);
                node.SetProperty(property.Key, value);
            }
            return node;
        }

        private EffectNode CheckEffectNode(EffectNode node, GlobalContext context)
        {
            return null;
        }

        private OnActValueNode CheckOnActValueNode(OnActValueNode node, GlobalContext context)
        {
            foreach (var property in node.properties)
            {
                if (property.Key == "Selector")
                    node.SetProperty(property.Key, Analyze(property.Value as SelectorNode));
                else if (property.Key == "EffectData")
                    node.SetProperty(property.Key, Analyze(property.Value as EffectDataNode));
                else if (property.Key == "PosAction")
                    node.SetProperty(property.Key, Analyze(property.Value as PosActionNode));
            }
            return node;
        }

        private SelectorNode CheckSelectorNode(SelectorNode node, GlobalContext context)
        {
            foreach (var property in node.properties)
            {
                Type exprectedType = GetExpectedTypeForProperty(property.Key);
                Type expType = CheckExpression(node.GetProperty<ExpressionNode>(property.Key), context);
                if (expType != exprectedType)
                    throw new Exception("No son del mismo tipo");

                object value = (property.Value as ExpressionNode).Evaluate(context, null);
                node.SetProperty(property.Key, value);
            }
            return node;
        }

        private ProgramNode CheckEffectDataNode(ProgramNode node, GlobalContext context)
        {

            return null;
        }

        public Type CheckExpression(ExpressionNode expression, GlobalContext context)
        {
            switch (expression)
            {
                case IdentifierNode:
                    if ((expression as IdentifierNode).type != null)
                    {
                        if (context.ConteinsSymbol((expression as IdentifierNode).value))
                        {
                            return context.LookupSymbol((expression as IdentifierNode).value).Item1;
                        }
                        else
                        {
                            throw new Exception("Indentifier not defined");
                        }
                    }
                    else
                    {
                        return (expression as IdentifierNode).type.GetType();
                    }

                case LiteralNode:
                    return (expression as LiteralNode).value.GetType();

                case BinaryExpressionNode:
                    Type leftExpression = CheckExpression((expression as BinaryExpressionNode).left, context);
                    Type rightExpression = CheckExpression((expression as BinaryExpressionNode).right, context);
                    if (leftExpression != rightExpression)
                        throw new Exception("The two expressions aren't the same type");
                    return leftExpression;

                case AssignamentNode:
                    Type left = CheckExpression((expression as AssignamentNode).identifier, context);
                    Type right = CheckExpression((expression as AssignamentNode).value, context);
                    if (left != right)
                        throw new Exception("The two expressions aren't the same type");
                    return left;

                case MethodAccessNode:
                case ListNode:
                case ForNode:
                case WhileNode:
                    return null;
                default: throw new Exception("Not handled expression types");
            }
        }

        public Type GetExpectedTypeForProperty(string propertyName)
        {
            if (expectedPropertyTypes.TryGetValue(propertyName, out Type expectedType))
            {
                return expectedType;
            }
            else
            {
                throw new ArgumentException($"Property '{propertyName}' not recognized.");
            }
        }
    }
}
