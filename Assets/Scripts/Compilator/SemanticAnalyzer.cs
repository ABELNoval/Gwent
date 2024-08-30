using System;
using System.Collections.Generic;
using System.Diagnostics;

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
            { "Source", typeof(string) },
            {"Single", typeof(bool)},
            {"Predicate", typeof(string)}
        };

        public void CheckCardNode(CardNode node, GlobalContext context)
        {
            foreach (var property in node.properties)
            {
                if (property.Key == "OnActivation")
                {
                    foreach (var onActValue in (property.Value as OnActivationNode).OnActValues)
                    {
                        CheckOnActValueNode(onActValue, new GlobalContext(context));
                    }
                }
                else if (property.Key == "Range")
                {
                    foreach (var expression in property.Value as List<ExpressionNode>)
                    {
                        Type exprectedType = GetExpectedTypeForProperty((expression as LiteralNode).value.ToString());
                        Type expType = CheckExpression(expression, context);
                        if (expType != exprectedType)
                            throw new Exception("No son del mismo tipo");
                    }
                }
                else
                {
                    Type exprectedType = GetExpectedTypeForProperty(property.Key);
                    Type expType = CheckExpression(node.GetProperty<ExpressionNode>(property.Key), context);
                    if (expType != exprectedType)
                        throw new Exception("No son del mismo tipo");
                }
            }
        }

        public void CheckEffectNode(EffectNode node, GlobalContext context)
        {
            foreach (var property in node.properties)
            {
                if (property.Key == "Action")
                {
                    node.SetProperty(property.Key, CheckActionNode(property.Value as ActionNode, new GlobalContext(context)));
                }
                Type exprectedType = GetExpectedTypeForProperty(property.Key);
                Type expType = CheckExpression(node.GetProperty<ExpressionNode>(property.Key), context);
                if (expType != exprectedType)
                    throw new Exception("No son del mismo tipo");
            }
        }

        private void CheckOnActValueNode(OnActValueNode node, GlobalContext context)
        {
            foreach (var property in node.properties)
            {
                if (property.Key == "Selector")
                    CheckSelectorNode(property.Value as SelectorNode, new GlobalContext(context));
                else if (property.Key == "EffectData")
                    CheckEffectDataNode(property.Value as EffectDataNode, new GlobalContext(context));
                else if (property.Key == "PosAction")
                    CheckPosActionNode(property.Value as PosActionNode, new GlobalContext(context));
            }
        }

        private void CheckSelectorNode(SelectorNode node, GlobalContext context)
        {
            foreach (var property in node.properties)
            {
                if (property.Key == "Predicate")
                {
                    return;
                }
                Type exprectedType = GetExpectedTypeForProperty(property.Key);
                Type expType = CheckExpression(node.GetProperty<ExpressionNode>(property.Key), context);
                if (expType != exprectedType)
                    throw new Exception("No son del mismo tipo");
            }
        }

        private void CheckEffectDataNode(ProgramNode node, GlobalContext context)
        {
            foreach (var property in node.properties)
            {
                if (property.Key == "Params")
                {
                    foreach (var expression in property.Value as List<(string, ExpressionNode)>)
                    {
                        CheckExpression(expression.Item2, context);
                    }
                }
                else
                {
                    Type exprectedType = GetExpectedTypeForProperty(property.Key);
                    Type expType = CheckExpression(node.GetProperty<ExpressionNode>(property.Key), context);
                    if (expType != exprectedType)
                        throw new Exception("No son del mismo tipo");
                }
            }
        }

        private void CheckPosActionNode(ProgramNode node, GlobalContext context)
        {
            foreach (var property in node.properties)
            {
                if (property.Key == "Selector")
                {
                    CheckSelectorNode(property.Value as SelectorNode, new GlobalContext(context));
                }
                else
                {
                    Type exprectedType = GetExpectedTypeForProperty(property.Key);
                    Type expType = CheckExpression(node.GetProperty<ExpressionNode>(property.Key), context);
                    if (expType != exprectedType)
                        throw new Exception("No son del mismo tipo");
                }
            }
        }

        private ActionNode CheckActionNode(ActionNode node, GlobalContext context)
        {
            foreach (var expression in node.expressions)
            {
                CheckExpression(expression, context);
            }
            return node;
        }

        public Type CheckExpression(ExpressionNode expression, GlobalContext context)
        {
            switch (expression)
            {
                case IdentifierNode:
                    if ((expression as IdentifierNode).type != null)
                    {
                        if (context.ConteinsSymbol((expression as IdentifierNode).Name))
                        {
                            return context.LookupSymbol((expression as IdentifierNode).Name).Item1;
                        }
                        else if (context.parentContext.ConteinsSymbol((expression as IdentifierNode).Name))
                        {
                            return context.parentContext.LookupSymbol((expression as IdentifierNode).Name).Item1;
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
                    string name = (string)(expression as AssignamentNode).identifier.Evaluate(null, null, null);
                    Type right = CheckExpression((expression as AssignamentNode).value, context);
                    if (context.ConteinsSymbol(name))
                    {
                        Type left = context.LookupSymbol(name).Item1;
                        if (left != right)
                            throw new Exception("The two expressions aren't the same type");
                        context.DefineSymbol((string)(expression as AssignamentNode).identifier.Evaluate(null, null, null), right, (expression as AssignamentNode).value);
                        return left;
                    }
                    context.DefineSymbol(name, right, (expression as AssignamentNode).value);
                    return right;
                case MethodListNode:
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
