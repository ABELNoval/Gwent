using System.Linq.Expressions;
using System.Collections.Generic;
using System;
using Unity.Properties;

namespace Console
{
    public class Parser
    {
        private List<Token> input;
        private int position;
        private int expPosition;
        private int line;
        private int cantOpenParenthesis;
        private Token currentToken;
        private Dictionary<TokenType, Action<ProgramNode>> cardTokenHandlers;
        private Dictionary<TokenType, Action<ProgramNode>> effectTokenHandlers;
        private Dictionary<TokenType, Action<ProgramNode>> effectDataTokenHandlers;
        private Dictionary<TokenType, Action<ProgramNode>> onActValueTokenHandlers;
        private Dictionary<TokenType, Action<ProgramNode>> selectorTokenHandler;
        private Dictionary<TokenType, Action<ProgramNode>> posActionTokenHandler;
        private Dictionary<TokenType, Action<ProgramNode>> parametersTokenHandler;
        public ProgramNode currretnNode;
        private List<Token> expression;

        public Parser(List<Token> input)
        {
            this.input = input;
            position = 0;
            expPosition = 0;
            currentToken = input[position];
            cardTokenHandlers = new Dictionary<TokenType, Action<ProgramNode>>()
            {
                {TokenType.Name, HandleName},
                {TokenType.Type, HandleType},
                {TokenType.Faction, HandleFaction},
                {TokenType.Power, HandlePower},
                {TokenType.Range, HandleRange},
                {TokenType.OnActivation, HandleOnActivation}
            };

            effectTokenHandlers = new Dictionary<TokenType, Action<ProgramNode>>()
            {
                {TokenType.Name, HandleName},
                {TokenType.Params, HandleParams},
                {TokenType.Action, HandleAction}
            };

            onActValueTokenHandlers = new Dictionary<TokenType, Action<ProgramNode>>()
            {
                {TokenType.Effect, HandleEffectData},
                {TokenType.Selector, HandleSelector},
                {TokenType.PosAction, HandlePosAction}
            };

            effectDataTokenHandlers = new Dictionary<TokenType, Action<ProgramNode>>()
            {
                {TokenType.Name, HandleName},
                {TokenType.Amount, HandleAmount}
            };

            selectorTokenHandler = new Dictionary<TokenType, Action<ProgramNode>>()
            {
                {TokenType.Source, HandleSource},
                {TokenType.Single, HandleSingle},
                {TokenType.Predicate, HandlePredicate},
            };

            posActionTokenHandler = new Dictionary<TokenType, Action<ProgramNode>>()
            {
                {TokenType.Name, HandleName},
                {TokenType.Selector, HandleSelector}
            };

            parametersTokenHandler = new Dictionary<TokenType, Action<ProgramNode>>()
            {
                {TokenType.Amount, HandleAmount}
            };
        }


        #region Parse

        public ProgramNode Parse()
        {
            ProgramNode aSTNode = ParseNode();
            return aSTNode;
        }

        private ProgramNode ParseNode()
        {
            if (Match(TokenType.card))
            {
                return ParseNode(new CardNode(), cardTokenHandlers);
            }
            else if (Match(TokenType.effect))
            {
                return ParseNode(new EffectNode(), effectTokenHandlers);
            }
            else
            {
                throw new Exception($"Tipo desconocido {currentToken}");
            }
        }

        private ProgramNode ParseNode(ProgramNode node, Dictionary<TokenType, Action<ProgramNode>> dic)
        {
            Expect(TokenType.LeftBrace);

            while (currentToken.type != TokenType.EndOfFile && currentToken.type != TokenType.RightBrace)
            {
                if (dic.TryGetValue(currentToken.type, out Action<ProgramNode> handler))
                {
                    Advance();
                    handler(node);
                }
                else
                {
                    throw new Exception("Accion no valida");
                }
            }
            UnityEngine.Debug.Log("Fin");
            Expect(TokenType.RightBrace);

            //node.Validate();

            return node;
        }

        private ExpressionNode ParseExpression()
        {
            if (Match(TokenType.while_Token))
            {
                return ParseWhile();
            }
            if (Match(TokenType.for_Token))
            {
                return ParseFor();
            }
            return ParseConditions();
        }

        private ExpressionNode ParseConditions()
        {
            var left = ParseConditional();
            while (IsLogicalOperator(expression[expPosition].type))
            {
                Token operatorToken = expression[expPosition];
                expPosition++;

                var right = ParseConditional();
                left = new BinaryExpressionNode(left, operatorToken, right);
            }
            return left;
        }

        private ExpressionNode ParseConditional()
        {
            var left = ParseAddAndSubstractExpression();
            while (IsComparisonOperator(expression[expPosition].type))
            {
                Token operatorType = expression[expPosition];
                expPosition++;

                var right = ParseAddAndSubstractExpression();
                left = new BinaryExpressionNode(left, operatorType, right);
            }

            return left;
        }

        private ExpressionNode ParseAddAndSubstractExpression()
        {
            var left = ParseMultyAndDivExpression();

            while (expPosition < expression.Count && (expression[expPosition].type == TokenType.Plus || expression[expPosition].type == TokenType.Minus))
            {
                Token operatorToken = expression[expPosition];
                expPosition++;

                var right = ParseMultyAndDivExpression();
                left = new BinaryExpressionNode(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionNode ParseMultyAndDivExpression()
        {
            var left = ParseExponentExpression();

            while (expPosition < expression.Count && (expression[expPosition].type == TokenType.Multiply || expression[expPosition].type == TokenType.Divide))
            {
                Token operatorToken = expression[expPosition];
                expPosition++;

                var right = ParseExponentExpression();
                left = new BinaryExpressionNode(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionNode ParseExponentExpression()
        {
            var left = ParsePrimaryExpression();

            while (expPosition < expression.Count && expression[expPosition].type == TokenType.Exponent)
            {
                Token operatorToken = expression[expPosition];
                expPosition++;

                var right = ParsePrimaryExpression();
                left = new BinaryExpressionNode(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionNode ParsePrimaryExpression()
        {
            if (expPosition < expression.Count)
            {
                Token current = expression[expPosition];
                switch (current.type)
                {
                    case TokenType.Number:
                        return new LiteralNode(int.Parse(expression[expPosition].value));

                    case TokenType.Identifier:
                        return ParseIdentifier();

                    case TokenType.LeftParenthesis:
                        expPosition++;
                        ExpressionNode exp = ParseExpression();
                        Expect(TokenType.RightParenthesis);
                        return exp;

                    case TokenType.Boolean:
                        return ParseBoolean();

                    case TokenType.String:
                        return ParseString();

                    default:
                        throw new Exception("Token indefinido");
                }
            }
            else
            {
                throw new Exception("");
            }
        }

        private ExpressionNode ParseBoolean()
        {
            if (expression[expPosition].type != TokenType.Boolean)
            {
                throw new Exception($"Se esperraba un booleano, pero se encontro {currentToken.type}");
            }
            return new LiteralNode(expression[expPosition].value == "true");
        }

        private ExpressionNode ParseString()
        {
            if (expression[expPosition].type != TokenType.String)
            {
                throw new Exception($"Se esperaba un texto, pero se encontró {currentToken.type}");
            }

            string value = expression[expPosition].value;
            expPosition++;
            return new LiteralNode(value);
        }

        private ExpressionNode ParseIdentifier()
        {
            IdentifierNode node = new(expression[expPosition].value);
            expPosition++;
            if (expression[expPosition].type == TokenType.Dot)
            {
                expPosition++;
                if (expression[expPosition].type != TokenType.Identifier)
                {
                    throw new Exception("Se espera un identificador despues del punto");
                }
                node.AddProperty(ParseIdentifier());
            }
            else
            {
                if (expression[expPosition].type == TokenType.LeftParenthesis)
                {
                    return ParseMethodAccess(node.value);
                }
                if (expression[expPosition].type == TokenType.LeftBracket)
                {
                    return ParseListAccess(node.value);
                }
                if (expression[expPosition].type == TokenType.Assign)
                {
                    return ParseAssignment(node.value);
                }
                if (expression[expPosition].type == TokenType.Decrement)
                {
                    return ParseDecrement(node.value);
                }
                if (expression[expPosition].type == TokenType.Increment)
                {
                    return ParseIncrement(node.value);
                }
            }
            return node;
        }

        private ExpressionNode ParseMethodAccess(string value)
        {
            MethodAccessNode node = new(value);
            expPosition++;

            while (expression[expPosition].type != TokenType.RightParenthesis)
            {
                ExpressionNode param = ParseExpression();
                node.AddParameter(param);
                if (expression[expPosition].type == TokenType.Comma)
                {
                    expPosition++;
                }
            }
            expPosition++;
            return node;
        }

        private ExpressionNode ParseListAccess(string value)
        {
            ListNode node = new(value);
            expPosition++;

            while (expression[expPosition].type != TokenType.RightBracket)
            {
                ExpressionNode member = ParseExpression();
                node.AddMember(member);
            }
            expPosition++;
            return node;
        }

        private ExpressionNode ParseAssignment(string identifier)
        {
            AssignamentNode node = new(identifier);
            expPosition++;
            ExpressionNode value = ParseExpression();
            node.SetValue(value);
            return node;
        }

        private ExpressionNode ParseIncrement(string value)
        {
            IdentifierNode left = new IdentifierNode(value);
            LiteralNode right = new LiteralNode(1);
            Token op = new Token(TokenType.Minus, "+");
            return new BinaryExpressionNode(left, op, right);
        }

        private ExpressionNode ParseDecrement(string value)
        {
            IdentifierNode left = new IdentifierNode(value);
            LiteralNode right = new LiteralNode(1);
            Token op = new Token(TokenType.Minus, "-");
            return new BinaryExpressionNode(left, op, right);
        }

        private bool IsLogicalOperator(TokenType value)
        {
            return value == TokenType.LogicalAnd || value == TokenType.LogicalOr;
        }

        private bool IsComparisonOperator(TokenType type)
        {
            return type == TokenType.LessThan ||
            type == TokenType.GreaterThan ||
            type == TokenType.Equals ||
            type == TokenType.NotEquals ||
            type == TokenType.LessThanOrEqual ||
            type == TokenType.GreaterThanOrEqual;
        }

        private void GenerateExpression()
        {
            expression = new List<Token>();

            while (!Match(TokenType.Comma) && !Match(TokenType.Semicolon) && currentToken.type != TokenType.RightBrace)
            {
                expression.Add(currentToken);
                Advance();
            }
        }

        private ExpressionNode ParseWhile()
        {
            List<ExpressionNode> body = new();
            Expect(TokenType.LeftParenthesis);
            var condition = ParseExpression();
            Expect(TokenType.RightParenthesis);
            if (!Match(TokenType.LeftBrace))
            {
                body.Add(ParseExpression());
            }
            else
            {
                while (!Match(TokenType.RightBrace))
                {
                    body.Add(ParseExpression());
                    Expect(TokenType.Semicolon);
                }
            }
            Expect(TokenType.Semicolon);
            return new WhileNode(condition, body);
        }

        private ExpressionNode ParseFor()
        {
            List<ExpressionNode> body = new();
            Expect(TokenType.Identifier);
            Expect(TokenType.in_Token);
            Expect(TokenType.Target);
            if (!Match(TokenType.LeftBrace))
            {
                body.Add(ParseExpression());
            }
            else
            {
                while (!Match(TokenType.RightBrace))
                {
                    body.Add(ParseExpression());
                    Expect(TokenType.Semicolon);
                }
            }
            Expect(TokenType.Semicolon);
            return new ForNode(body);
        }

        #endregion


        #region Handle

        private void HandleName(ProgramNode node)
        {
            UnityEngine.Debug.Log("Name");
            Expect(TokenType.Colon);
            GenerateExpression();
            node.SetProperty("Name", ParseExpression());
            Match(TokenType.Comma);
        }

        private void HandleType(ProgramNode node)
        {
            UnityEngine.Debug.Log("Type");
            Expect(TokenType.Colon);
            GenerateExpression();
            node.SetProperty("Type", ParseExpression());
            Match(TokenType.Comma);
        }

        private void HandleFaction(ProgramNode node)
        {
            UnityEngine.Debug.Log("Faction");
            Expect(TokenType.Colon);
            GenerateExpression();
            node.SetProperty("Faction", ParseExpression());
            Match(TokenType.Comma);
        }

        private void HandlePower(ProgramNode node)
        {
            UnityEngine.Debug.Log("Power");
            Expect(TokenType.Colon);
            GenerateExpression();
            node.SetProperty("Power", ParseExpression());
            Match(TokenType.Comma);
        }

        private void HandleRange(ProgramNode node)
        {
            UnityEngine.Debug.Log("Range");
            Expect(TokenType.Colon);
            Expect(TokenType.LeftBracket);

            while (!Match(TokenType.RightBracket))
            {
                GenerateExpression();
                node.AddProperty("Range", ParseExpression());
            }

            Match(TokenType.Comma);
        }

        private void HandleOnActivation(ProgramNode node)
        {
            UnityEngine.Debug.Log("OnActivation");
            Expect(TokenType.Colon);
            Expect(TokenType.LeftBracket);

            OnActivationNode onActivationNode = new OnActivationNode();

            while (currentToken.type != TokenType.RightBracket)
            {
                ProgramNode actValueNode = ParseNode(new OnActValueNode(), onActValueTokenHandlers);
                onActivationNode.AddProperty("OnActValues", actValueNode as OnActValueNode);

                Match(TokenType.Comma);
            }

            Expect(TokenType.RightBracket);
            node.SetProperty("OnActivation", onActivationNode);
            Match(TokenType.Comma);
        }

        private void HandleEffectData(ProgramNode node)
        {
            UnityEngine.Debug.Log("EffectData");
            Expect(TokenType.Colon);
            EffectDataNode val = new();

            if (currentToken.type == TokenType.String)
            {
                GenerateExpression();
                val.SetProperty("Name", ParseExpression());
                node.SetProperty("EffectData", val);
            }
            else
            {
                ProgramNode value = ParseNode(new EffectDataNode(), effectDataTokenHandlers);
                node.SetProperty("EffectData", value as EffectDataNode);
            }

            Match(TokenType.Comma);
        }

        private void HandleSelector(ProgramNode node)
        {
            UnityEngine.Debug.Log("Selector");
            Expect(TokenType.Colon);
            ProgramNode value = ParseNode(new SelectorNode(), selectorTokenHandler);
            node.SetProperty("Selector", value as SelectorNode);
            Match(TokenType.Comma);
        }

        private void HandlePosAction(ProgramNode node)
        {
            UnityEngine.Debug.Log("PosAction");
            Expect(TokenType.Colon);
            ProgramNode value = ParseNode(new PosActionNode(), posActionTokenHandler);
            node.SetProperty("PosAction", value as PosActionNode);
            Match(TokenType.Comma);
        }

        private void HandleSource(ProgramNode node)
        {
            UnityEngine.Debug.Log("Source");
            Expect(TokenType.Colon);
            GenerateExpression();
            node.SetProperty("Source", ParseExpression());
            Match(TokenType.Comma);
        }

        private void HandleSingle(ProgramNode node)
        {
            UnityEngine.Debug.Log("Single");
            Expect(TokenType.Colon);
            GenerateExpression();
            node.SetProperty("Single", ParseExpression());
            Match(TokenType.Comma);
        }

        private void HandlePredicate(ProgramNode node)
        {
            UnityEngine.Debug.Log("Predicate");
            Expect(TokenType.Colon);
            GenerateExpression();
            node.SetProperty("Predicate", ParseExpression());
            Match(TokenType.Comma);
        }

        private void HandleParams(ProgramNode node)
        {
            UnityEngine.Debug.Log("Params");
            Expect(TokenType.Colon);
            node.SetProperty("Params", ParseNode(new ParameterNode(), parametersTokenHandler));
            Match(TokenType.Comma);
        }

        private void HandleAmount(ProgramNode node)
        {
            UnityEngine.Debug.Log("Amount");
            Expect(TokenType.Colon);
            GenerateExpression();
            while (currentToken.type != TokenType.RightBrace)
            {
                Advance();
                node.AddProperty("Amount", ParseExpression());
            }
            Match(TokenType.Comma);
        }

        private void HandleAction(ProgramNode node)
        {
            UnityEngine.Debug.Log("Action");

            Expect(TokenType.Colon);
            Expect(TokenType.LeftParenthesis);
            Expect(TokenType.Target);
            Expect(TokenType.Comma);
            Expect(TokenType.Context);
            Expect(TokenType.RightParenthesis);
            Expect(TokenType.Arrow);
            Expect(TokenType.LeftBrace);

            List<ExpressionNode> expressions = new();
            while (currentToken.type != TokenType.RightBrace && currentToken.type != TokenType.EndOfFile)
            {
                expressions.Add(ParseExpression());
            }

            node.SetProperty("Action", new ActionNode(expressions));
            Match(TokenType.Comma);
        }

        #endregion


        private bool Match(TokenType expectedType)
        {
            if (currentToken.type == expectedType)
            {
                Advance();
                return true;
            }
            return false;
        }

        private Token Peek()
        {
            if (position < input.Count - 1)
            {
                return input[position + 1];
            }
            else
            {
                throw new Exception("Llegaste al limite del input");
            }
        }

        private void Advance()
        {
            if (position < input.Count - 1)
            {
                position++;
                currentToken = input[position];
            }
            else
            {
                currentToken = null;
            }
        }

        private void Expect(TokenType expect)
        {
            UnityEngine.Debug.Log(expect);
            if (!Match(expect))
            {
                throw new Exception("Token inesperado");
            }
        }

        private Token Previous()
        {
            return input[position - 1];
        }
    }
}