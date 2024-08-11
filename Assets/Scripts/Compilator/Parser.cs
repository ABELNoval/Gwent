using System.Collections.Generic;
using System;

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
        private Dictionary<TokenType, Action<ProgramNode>> actionTokenHandler;
        private Dictionary<TokenType, Action<ProgramNode>> parametersTokenHandler;
        public ProgramNode currretnNode;
        private List<Token> expressions = new List<Token>();

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

            actionTokenHandler = new Dictionary<TokenType, Action<ProgramNode>>()
            {
                {TokenType.Identifier, HandleReflection},
                {TokenType.for_Token, HandleFor},
                {TokenType.while_Token, HandleWhile}
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

            // TODO: Implementar semantico

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

        private string ParseString()
        {
            if (currentToken.type != TokenType.String)
            {
                throw new Exception($"Se esperaba un texto, pero se encontró {currentToken.type}");
            }

            string value = currentToken.value;
            Advance();
            return value;
        }

        private int ParseInt()
        {
            while (currentToken.type != TokenType.RightBrace && currentToken.type != TokenType.Comma)
            {
                expressions.Add(currentToken);
                Advance();
            }

            ExpressionNode result = ParseExpression();

            return result.Evaluate();
        }

        private bool ParseBool()
        {
            if (currentToken.type != TokenType.Boolean)
            {
                throw new Exception($"Se esperraba un booleano, pero se encontro {currentToken.type}");
            }
            if (currentToken.value == "false")
            {
                Advance();
                return false;
            }
            else
            {
                Advance();
                return true;
            }
        }

        private ExpressionNode ParseExpression()
        {
            ExpressionNode expression = ParseAddAndSubstractExpression();
            if (cantOpenParenthesis > 0)
            {
                cantOpenParenthesis--;
                expPosition++;
            }
            return expression;
        }

        private ExpressionNode ParseAddAndSubstractExpression()
        {
            var left = ParseMultyAndDivExpression();

            while (expPosition < expressions.Count && (expressions[expPosition].type == TokenType.Plus || expressions[expPosition].type == TokenType.Minus))
            {
                Token operatorToken = expressions[expPosition];
                expPosition++;

                var rigth = ParseMultyAndDivExpression();
                left = new BinaryExpressionNode(left, operatorToken, rigth);
            }

            return left;
        }

        private ExpressionNode ParseMultyAndDivExpression()
        {
            var left = ParseExponentExpression();

            while (expPosition < expressions.Count && (expressions[expPosition].type == TokenType.Multiply || expressions[expPosition].type == TokenType.Divide))
            {
                Token operatorToken = expressions[expPosition];
                expPosition++;

                var rigth = ParseExponentExpression();
                left = new BinaryExpressionNode(left, operatorToken, rigth);
            }

            return left;
        }

        private ExpressionNode ParseExponentExpression()
        {
            var left = ParsePrimaryExpression();

            while (expPosition < expressions.Count && expressions[expPosition].type == TokenType.Exponent)
            {
                Token operatorToken = expressions[expPosition];
                expPosition++;

                var rigth = ParsePrimaryExpression();
                left = new BinaryExpressionNode(left, operatorToken, rigth);
            }

            return left;
        }

        private ExpressionNode ParsePrimaryExpression()
        {
            if (expPosition < expressions.Count && expressions[expPosition].type == TokenType.LeftParenthesis)
            {
                expPosition++;
                cantOpenParenthesis++;
                return ParseExpression();
            }
            if (expPosition < expressions.Count && expressions[expPosition].type == TokenType.Number)
            {
                int.TryParse(expressions[expPosition].value, out int value);
                expPosition++;
                UnityEngine.Debug.Log(value);
                return new NumberNode(value);
            }
            else
            {
                throw new Exception("Error");
            }
        }

        #endregion


        #region Handle

        private void HandleName(ProgramNode node)
        {
            UnityEngine.Debug.Log("Name");
            Expect(TokenType.Colon);
            node.SetProperty("Name", ParseString());
            Match(TokenType.Comma);
        }

        private void HandleType(ProgramNode node)
        {
            UnityEngine.Debug.Log("Type");
            Expect(TokenType.Colon);
            node.SetProperty("Type", ParseString());
            Match(TokenType.Comma);
        }

        private void HandleFaction(ProgramNode node)
        {
            UnityEngine.Debug.Log("Faction");
            Expect(TokenType.Colon);
            node.SetProperty("Faction", ParseString());
            Match(TokenType.Comma);
        }

        private void HandlePower(ProgramNode node)
        {
            UnityEngine.Debug.Log("Power");
            Expect(TokenType.Colon);
            node.SetProperty("Power", ParseInt());
            Match(TokenType.Comma);
        }

        private void HandleRange(ProgramNode node)
        {
            UnityEngine.Debug.Log("Range");
            Expect(TokenType.Colon);
            Expect(TokenType.LeftBracket);

            while (!Match(TokenType.RightBracket))
            {
                node.AddProperty("Range", ParseString());
                Match(TokenType.Comma);
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
                val.SetProperty("Name", ParseString());
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
            node.SetProperty("Source", ParseString());
            Match(TokenType.Comma);
        }

        private void HandleSingle(ProgramNode node)
        {
            UnityEngine.Debug.Log("Single");
            Expect(TokenType.Colon);
            node.SetProperty("Single", ParseBool());
            Match(TokenType.Comma);
        }

        private void HandlePredicate(ProgramNode node)
        {
            UnityEngine.Debug.Log("Predicate");
            Expect(TokenType.Colon);
            node.SetProperty("Predicate", ParseString());
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
            switch (currentToken.type)
            {
                case TokenType.String:
                    node.SetProperty("Amount", ParseString());
                    break;

                case TokenType.Number:
                    node.SetProperty("Amount", ParseInt());
                    break;

                case TokenType.Boolean:
                    node.SetProperty("Amount", ParseBool());
                    break;

                default:
                    throw new Exception("Tipo inesperado");
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
            Expect(TokenType.RigthParenthesis);
            Expect(TokenType.Arrow);

            node.SetProperty("Action", ParseNode(new ActionNode(), actionTokenHandler));
            Match(TokenType.Comma);
        }

        private void HandleWhile(ProgramNode node)
        {
            UnityEngine.Debug.Log("While");
            Expect(TokenType.LeftParenthesis);

            

            Expect(TokenType.RigthParenthesis);
        }

        private void HandleFor(ProgramNode node)
        {
            UnityEngine.Debug.Log("For");
        }

        private void HandleReflection(ProgramNode node)
        {

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