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
            {"Predicate", typeof(Cards)}
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
                }
                else if (property.Key == "Parameters")
                {
                    foreach (var parameter in property.Value as List<(string, Type)>)
                    {
                        context.DefineSymbol(parameter.Item1, parameter.Item2);
                    }
                }
                else if (!CompareTypes(property.Key, context, node.GetProperty<ExpressionNode>(property.Key)))
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
                    context.DefineSymbol("unit", typeof(Cards));
                    CheckExpression(node.GetProperty<ExpressionNode>(property.Key), context);
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
                        CheckExpression(expression.Item2, context);
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
                CheckExpression(expression, context);
            }
        }

        public Type CheckExpression(ExpressionNode expression, GlobalContext context)
        {
            Type leftExpression;
            Type rightExpression;
            switch (expression)
            {
                case IdentifierNode:
                    Type identifierType;
                    if (context.ConteinsSymbol((expression as IdentifierNode).Name))
                        identifierType = context.LookupSymbol((expression as IdentifierNode).Name);
                    else if (context.parentContext.ConteinsSymbol((expression as IdentifierNode).Name))
                        identifierType = context.parentContext.LookupSymbol((expression as IdentifierNode).Name);
                    else
                        throw new Exception("Indentifier not defined");
                    switch ((expression as IdentifierNode).property)
                    {
                        case null:
                            return identifierType;

                        case PropertyNode:
                            if (identifierType != typeof(Cards))
                                throw new Exception("Propiedad invalida");
                            return CheckExpression((expression as IdentifierNode).property, context);

                        case MethodCardNode:
                            if (identifierType != typeof(List<Cards>))
                                throw new Exception("Propiedad invalida");
                            return CheckExpression((expression as IdentifierNode).property, context);

                        case MethodListNode:
                            if (identifierType != typeof(Context))
                                throw new Exception("Propiedad invalid");
                            return CheckExpression((expression as IdentifierNode).property, context);

                        default:
                            throw new Exception("Propiedad invalida");
                    }

                case LiteralNode:
                    return (expression as LiteralNode).value.GetType();

                case BinaryExpressionNode:
                    rightExpression = CheckExpression((expression as BinaryExpressionNode).right, context);
                    leftExpression = CheckExpression((expression as BinaryExpressionNode).left, context);
                    if (leftExpression != rightExpression)
                        throw new Exception("The two expressions aren't the same type");
                    return leftExpression;

                case AssignamentNode:
                    AssignamentNode assignament = expression as AssignamentNode;
                    rightExpression = CheckExpression(assignament.value, context);
                    if (assignament.identifier is IdentifierNode && (assignament.identifier as IdentifierNode).property == null)
                    {
                        context.DefineSymbol((assignament.identifier as IdentifierNode).Name, rightExpression);
                        return rightExpression;
                    }
                    leftExpression = CheckExpression(assignament.identifier, context);
                    if (leftExpression != rightExpression)
                        throw new Exception("The two expressions aren't the same type");
                    return leftExpression;

                case MethodListNode:
                    if ((expression as MethodListNode).property is null)
                        return typeof(List<Cards>);
                    if ((expression as MethodListNode).property is not MethodCardNode)
                        throw new Exception("Propiedad invalida");
                    return CheckExpression((expression as MethodListNode).property, context);

                case MethodCardNode:
                    if ((expression as MethodCardNode).property is null)
                        return typeof(Cards);
                    if ((expression as MethodCardNode).property is not PropertyNode)
                        throw new Exception("Propiedad invalida");
                    return CheckExpression((expression as MethodCardNode).property, context);

                case ListNode:
                    if (CheckExpression((expression as ListNode).list, context) != typeof(List<Cards>) || CheckExpression((expression as ListNode).arg, context) != typeof(int))
                        throw new Exception("Propiedad invalida");
                    if ((expression as ListNode).property is null)
                    {
                        return typeof(Cards);
                    }
                    if ((expression as ListNode).property is not PropertyNode)
                        throw new Exception("Propiedad invalida");
                    return CheckExpression((expression as ListNode).property, context);

                case ForNode:
                    context.DefineSymbol("target", typeof(Cards));
                    context.DefineSymbol("context", typeof(Context));
                    foreach (var exp in (expression as ForNode).body)
                    {
                        CheckExpression(exp, context);
                    }
                    return null;

                case WhileNode:
                    CheckExpression((expression as WhileNode).condition, context);
                    foreach (var exp in (expression as WhileNode).body)
                    {
                        CheckExpression(exp, context);
                    }
                    return null;

                case PropertyNode:
                    return GetExpectedTypeForProperty((expression as PropertyNode).Name);

                case PredicateNode:
                    CheckExpression((expression as PredicateNode).condition, context);
                    return typeof(Cards);

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
            Type expType = CheckExpression(expression, context);
            if (expType != exprectedType)
                return false;
            return true;
        }
    }
}
