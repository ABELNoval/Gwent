using System;
using System.Collections.Generic;

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
                switch (property.Key)
                {
                    case "Name":
                    case "Power":
                    case "Faction":
                    case "Type":
                        if (!CompareTypes(property.Key, context, node.GetProperty<ExpressionNode>(property.Key)))
                            throw new Exception("No son del mismo tipo");
                        break;

                    case "OnActivation":
                        foreach (var onActValue in (property.Value as OnActivationNode).OnActValues)
                        {
                            CheckOnActValueNode(onActValue, new GlobalContext(context));
                        }
                        break;

                    case "Range":
                        foreach (var expression in property.Value as List<ExpressionNode>)
                        {
                            if (!CompareTypes((expression as LiteralNode).value.ToString(), context, expression))
                                throw new Exception("No son del mismo tipo");
                        }
                        break;

                    default:
                        throw new Exception("Propiedad no valida");
                }
            }
        }

        public void CheckEffectNode(EffectNode node, GlobalContext context)
        {
            foreach (var property in node.properties)
            {
                if (property.Key == "Action")
                {
                    CheckActionNode(property.Value as ActionNode, new GlobalContext(context));
                    return;
                }
                if (!CompareTypes(property.Key, context, node.GetProperty<ExpressionNode>(property.Key)))
                    throw new Exception("No son del mismo tipo");
            }
        }

        private void CheckOnActValueNode(OnActValueNode node, GlobalContext context)
        {
            foreach (var property in node.properties)
            {
                switch (property.Key)
                {
                    case "Selector":
                        CheckSelectorNode(property.Value as SelectorNode, new GlobalContext(context));
                        break;
                    case "EffectData":
                        CheckEffectDataNode(property.Value as EffectDataNode, new GlobalContext(context));
                        break;
                    case "PosAction":
                        CheckPosActionNode(property.Value as PosActionNode, new GlobalContext(context));
                        break;
                    default:
                        throw new Exception("Propiedad no valida");
                }
            }
        }

        private void CheckSelectorNode(SelectorNode node, GlobalContext context)
        {
            foreach (var property in node.properties)
            {
                if (property.Key == "Predicate")
                {
                    CheckExpressionType(node.GetProperty<ExpressionNode>(property.Key), context);
                    return;
                }
                if (!CompareTypes(property.Key, context, node.GetProperty<ExpressionNode>(property.Key)))
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
                        CheckExpressionType(expression.Item2, context);
                    }
                    return;
                }
                if (!CompareTypes(property.Key, context, node.GetProperty<ExpressionNode>(property.Key)))
                    throw new Exception("No son del mismo tipo");
            }
        }

        private void CheckPosActionNode(ProgramNode node, GlobalContext context)
        {
            foreach (var property in node.properties)
            {
                if (property.Key == "Selector")
                {
                    CheckSelectorNode(property.Value as SelectorNode, new GlobalContext(context));
                    return;
                }
                if (!CompareTypes(property.Key, context, node.GetProperty<ExpressionNode>(property.Key)))
                    throw new Exception("No son del mismo tipo");
            }
        }

        private void CheckActionNode(ActionNode node, GlobalContext context)
        {
            foreach (var expression in node.expressions)
            {
                CheckExpressionType(expression, context);
            }
        }

        public Type CheckExpressionType(ExpressionNode expression, GlobalContext context)
        {
            Type leftExpression;
            Type rightExpression;
            switch (expression)
            {
                case IdentifierNode:
                    if ((expression as IdentifierNode).property == null)
                    {
                        if ((expression as IdentifierNode).type != null)
                        {
                            if (context.ConteinsSymbol((expression as IdentifierNode).Name))
                            {
                                return context.LookupSymbol((expression as IdentifierNode).Name);
                            }
                            if (context.parentContext.ConteinsSymbol((expression as IdentifierNode).Name))
                            {
                                return context.parentContext.LookupSymbol((expression as IdentifierNode).Name);
                            }
                            throw new Exception("Indentifier not defined");
                        }
                        return (expression as IdentifierNode).type.GetType();
                    }
                    return CheckExpressionType((expression as IdentifierNode).property, context);

                case LiteralNode:
                    return (expression as LiteralNode).value.GetType();

                case BinaryExpressionNode:
                    leftExpression = CheckExpressionType((expression as BinaryExpressionNode).left, context);
                    rightExpression = CheckExpressionType((expression as BinaryExpressionNode).right, context);
                    if (leftExpression != rightExpression)
                        throw new Exception("The two expressions aren't the same type");
                    return leftExpression;

                case AssignamentNode:
                    leftExpression = CheckExpressionType((expression as AssignamentNode).value, context);
                    rightExpression = CheckExpressionType((expression as AssignamentNode).identifier, context);
                    if (leftExpression != rightExpression)
                        throw new Exception("The two expressions aren't the same type");
                    return leftExpression;

                case MethodListNode:
                    return typeof(List<Cards>);

                case MethodCardNode:
                case ListNode:
                    return typeof(Cards);

                case ForNode:
                    foreach (var exp in (expression as ForNode).body)
                    {
                        CheckExpressionType(exp, context);
                    }
                    return null;

                case WhileNode:
                    CheckExpressionType((expression as WhileNode).condition, context);
                    foreach (var exp in (expression as ForNode).body)
                    {
                        CheckExpressionType(exp, context);
                    }
                    return null;

                case PropertyNode:
                    return GetExpectedTypeForProperty((expression as PropertyNode).Name);

                case PredicateNode:
                    return CheckExpressionType((expression as PredicateNode).condition, context);

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

        private bool CompareTypes(string propertyName, GlobalContext context, ExpressionNode expression)
        {
            Type exprectedType = GetExpectedTypeForProperty(propertyName);
            Type expType = CheckExpressionType(expression, context);
            if (expType != exprectedType)
                return false;
            return true;
        }
    }
}
