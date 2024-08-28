using System.Linq.Expressions;
using System.Collections.Generic;
using System;
using Unity.Properties;
using System.Diagnostics;

namespace Console
{
    public class Parser
    {
        private List<Token> input;
        private int position;
        private int expPosition;
        private int line;
        private Token currentToken;
        private Token currentExpression;
        private Dictionary<TokenType, Action<ProgramNode>> cardTokenHandlers;
        private Dictionary<TokenType, Action<ProgramNode>> effectTokenHandlers;
        private Dictionary<TokenType, Action<ProgramNode>> effectDataTokenHandlers;
        private Dictionary<TokenType, Action<ProgramNode>> onActValueTokenHandlers;
        private Dictionary<TokenType, Action<ProgramNode>> selectorTokenHandler;
        private Dictionary<TokenType, Action<ProgramNode>> posActionTokenHandler;
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
                {TokenType.Identifier, ParseParams}
            };

            selectorTokenHandler = new Dictionary<TokenType, Action<ProgramNode>>()
            {
                {TokenType.Source, HandleSource},
                {TokenType.Single, HandleSingle},
                {TokenType.Predicate, HandlePredicate},
            };

            posActionTokenHandler = new Dictionary<TokenType, Action<ProgramNode>>()
            {
                {TokenType.Type, HandleType},
                {TokenType.Selector, HandleSelector}
            };
        }


        #region Parse

        public ProgramNode Parse()
        {
            return ParseNode();
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
            return ParseConditions();
        }

        private ExpressionNode ParseConditions()
        {
            var left = ParseConditional();
            List<TokenType> types = new List<TokenType>()
            {
                TokenType.LogicalOr,
                TokenType.LogicalAnd
            };
            while (expPosition < expression.Count && MatchExp(types))
            {
                Token operatorToken = PreviousExp();

                var right = ParseConditional();
                left = new BinaryExpressionNode(left, operatorToken, right);
            }
            return left;
        }

        private ExpressionNode ParseConditional()
        {
            var left = ParseAddAndSubstractExpression();
            List<TokenType> types = new List<TokenType>()
            {
                TokenType.LessThan,
                TokenType.GreaterThan,
                TokenType.Equals,
                TokenType.NotEquals,
                TokenType.LessThanOrEqual,
                TokenType.GreaterThanOrEqual
            };
            while (expPosition < expression.Count && MatchExp(types))
            {
                Token operatorType = Previous();

                var right = ParseAddAndSubstractExpression();
                left = new BinaryExpressionNode(left, operatorType, right);
            }

            return left;
        }

        private ExpressionNode ParseAddAndSubstractExpression()
        {
            var left = ParseMultyAndDivExpression();
            List<TokenType> types = new List<TokenType>()
            {
                TokenType.Plus,
                TokenType.Minus
            };
            while (expPosition < expression.Count && MatchExp(types))
            {
                Token operatorToken = PreviousExp();
                var right = ParseMultyAndDivExpression();
                left = new BinaryExpressionNode(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionNode ParseMultyAndDivExpression()
        {
            var left = ParseExponentExpression();
            List<TokenType> types = new List<TokenType>()
            {
                TokenType.Multiply,
                TokenType.Divide
            };
            while (expPosition < expression.Count && MatchExp(types))
            {
                Token operatorToken = PreviousExp();

                var right = ParseExponentExpression();
                left = new BinaryExpressionNode(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionNode ParseExponentExpression()
        {
            var left = ParsePrimaryExpression();
            while (expPosition < expression.Count && MatchExp(TokenType.Exponent))
            {
                Token operatorToken = PreviousExp();

                var right = ParsePrimaryExpression();
                left = new BinaryExpressionNode(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionNode ParsePrimaryExpression()
        {
            if (expPosition < expression.Count)
            {
                switch (currentExpression.type)
                {
                    case TokenType.NumberLiteral:
                        LiteralNode number = new LiteralNode(int.Parse(currentExpression.value));
                        AdvanceExp();
                        return number;

                    case TokenType.Identifier:
                        AdvanceExp();
                        return ParseAssignment();

                    case TokenType.String:
                    case TokenType.Number:
                    case TokenType.Boolean:
                        string value = currentExpression.value;
                        AdvanceExp();
                        ExpectExp(TokenType.Identifier);
                        return ParseIdentifier();

                    case TokenType.LeftParenthesis:
                        AdvanceExp();
                        ExpressionNode exp = ParseExpression();
                        ExpectExp(TokenType.RightParenthesis);
                        return exp;

                    case TokenType.BooleanLiteral:
                        LiteralNode boolean = new LiteralNode(currentExpression.value == "true");
                        AdvanceExp();
                        return boolean;

                    case TokenType.StringLiteral:
                        LiteralNode text = new LiteralNode(currentExpression.value);
                        AdvanceExp();
                        return text;

                    default:
                        throw new Exception("Token indefinido");
                }
            }
            else
            {
                throw new Exception("Expression no valida");
            }
        }

        private ExpressionNode ParseAssignment()
        {
            var left = ParseIdentifier();
            while (expPosition < expression.Count && MatchExp(TokenType.Dot))
            {
                var right = ParseListAccess();
                left.SetProperty(right);
            }
            return left;
        }

        private ExpressionNode ParseIdentifier()
        {
            var left = ParseListAccess();
            while (expPosition < expression.Count && MatchExp(TokenType.Dot))
            {
                var right = ParseListAccess();
                left.SetProperty(right);
            }
            return left;
        }

        private ExpressionNode ParseListAccess()
        {
            var left = ParseMethodAccess();
            if (expPosition < expression.Count && MatchExp(TokenType.LeftBracket))
            {
                left = new ListNode(left, ParseExpression());
                ExpectExp(TokenType.RightBracket);
            }
            return left;
        }

        private ExpressionNode ParseMethodAccess()
        {
            List<TokenType> methodTypes = new List<TokenType>()
            {
                TokenType.Board,
                TokenType.DeckOfPlayer,
                TokenType.FieldOfPLayer,
                TokenType.GraveyardOfPlayer,
                TokenType.HandOfPlayer,
                TokenType.Hand,
                TokenType.Deck,
                TokenType.Field,
                TokenType.Graveyard
            };
            var left = ParsePropertyAccess();
            if (expPosition < expression.Count && MatchExp(methodTypes))
            {
                left = new MethodAccessNode(PreviousExp().value);
            }
            return left;
        }

        private ExpressionNode ParsePropertyAccess()
        {
            List<TokenType> propertyTypes = new List<TokenType>()
            {
                TokenType.Power,
                TokenType.Faction,
                TokenType.Name,
                TokenType.Type,
                TokenType.Range,
            };
            if (expPosition < expression.Count && MatchExp(propertyTypes))
            {
                var left = new PropertyAccessNode(PreviousExp().value);
                return left;
            }
            else
            {
                var left = new IdentifierNode(currentExpression.value, null);
                return left;
            }
        }


        private ExpressionNode ParseIncrement(string value)
        {
            IdentifierNode left = new(value, null);
            LiteralNode right = new(1);
            Token op = new(TokenType.Plus, "+");
            return new BinaryExpressionNode(left, op, right);
        }

        private ExpressionNode ParseDecrement(string value)
        {
            IdentifierNode left = new(value, null);
            LiteralNode right = new(1);
            Token op = new(TokenType.Minus, "-");
            return new BinaryExpressionNode(left, op, right);
        }

        private void GenerateExpression()
        {
            int cantLeftParen = 0;
            if (Match(TokenType.while_Token))
            {
                ParseWhile();
                return;
            }
            if (Match(TokenType.for_Token))
            {
                ParseFor();
                return;
            }

            expression = new List<Token>();

            while (!Match(TokenType.Comma) && !Match(TokenType.Semicolon) && currentToken.type != TokenType.RightBrace && currentToken.type != TokenType.RightBracket && (currentToken.type != TokenType.RightParenthesis || cantLeftParen > 0))
            {
                if (currentToken.type == TokenType.LeftParenthesis)
                {
                    cantLeftParen++;
                }
                if (currentToken.type == TokenType.RightParenthesis)
                {
                    cantLeftParen--;
                }
                expression.Add(currentToken);
                Advance();
            }
            expPosition = 0;
            currentExpression = expression[expPosition];
        }

        private ExpressionNode ParseWhile()
        {
            List<ExpressionNode> body = new();
            Expect(TokenType.LeftParenthesis);
            GenerateExpression();
            var condition = ParseExpression();
            Expect(TokenType.RightParenthesis);
            if (!Match(TokenType.LeftBrace))
            {
                GenerateExpression();
                body.Add(ParseExpression());
            }
            else
            {
                while (!Match(TokenType.RightBrace))
                {
                    GenerateExpression();
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
                GenerateExpression();
                body.Add(ParseExpression());
            }
            else
            {
                while (!Match(TokenType.RightBrace))
                {
                    GenerateExpression();
                    body.Add(ParseExpression());
                }
            }
            Expect(TokenType.Semicolon);
            return new ForNode(body);
        }

        private void ParseParams(ProgramNode node)
        {
            node.SetProperty("Params", new List<(string, ExpressionNode)>());
            while (currentToken.type != TokenType.RightBrace)
            {
                Token identifier = Previous();
                Expect(TokenType.Colon);
                GenerateExpression();
                node.AddProperty("Params", (identifier.value, ParseExpression()));
                Match(TokenType.Comma);
            }
            Match(TokenType.Comma);
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

            if (currentToken.type == TokenType.StringLiteral)
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
            Expect(TokenType.LeftBrace);
            Expect(TokenType.Identifier);
            ParseParams(node);
            Expect(TokenType.RightBrace);
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
            while (!Match(TokenType.RightBrace) && !Match(TokenType.EndOfFile))
            {
                GenerateExpression();
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

        private bool MatchExp(TokenType expectedType)
        {
            if (currentExpression.type == expectedType)
            {
                if (expPosition < expression.Count - 1)
                {
                    AdvanceExp();
                }
                return true;
            }
            return false;
        }

        private bool MatchExp(List<TokenType> expectedTypes)
        {
            int i = 0;
            while (i <= expectedTypes.Count - 1 && currentExpression.type != expectedTypes[i])
            {
                i++;
            }
            if (i <= expectedTypes.Count - 1)
            {
                AdvanceExp();
                return true;
            }
            return false;
        }

        private Token PeekNext()
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

        private void AdvanceExp()
        {
            if (expPosition < expression.Count - 1)
            {
                expPosition++;
                currentExpression = expression[expPosition];
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

        private void ExpectExp(TokenType expect)
        {
            UnityEngine.Debug.Log(expect);
            if (!MatchExp(expect))
            {
                throw new Exception($"Token inesperado en la expresion {currentExpression.type}");
            }
        }

        private Token Previous()
        {
            return input[position - 1];
        }

        private Token PreviousExp()
        {
            return expression[expPosition - 1];
        }
    }
}