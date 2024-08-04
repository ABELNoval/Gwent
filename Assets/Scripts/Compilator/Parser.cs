using System.Runtime.ExceptionServices;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace Console
{
    public class Parser
    {
        private List<Token> input;
        private int position;
        private int line;
        private Token currentToken;
        private Dictionary<TokenType, Action> tokenHandlers;
        private CardNode cardNode;
        private EffectNode effectNode;

        public Parser(List<Token> input)
        {
            this.input = input;
            position = 0;
            currentToken = input[position];
            cardNode = new CardNode();
            effectNode = new EffectNode();
            tokenHandlers = new Dictionary<TokenType, Action>
            {
                {TokenType.Name, HandleName},
                {TokenType.Faction, HandleFaction},
                {TokenType.Power, HandlePower},
                {TokenType.Type, HandleType},
                {TokenType.Range, HandleRange},
                {TokenType.OnActivation, HandleOnActivation}
            };
        }

        public ASTNode Parse()
        {
            ASTNode aSTNode = ParseNode();

            // TODO: Implementar semantico

            return aSTNode;
        }

        private ASTNode ParseNode()
        {
            if (Match(TokenType.card))
            {
                return ParseCard();
            }
            else if (Match(TokenType.effect))
            {
                return ParseEffect();
            }
            else
            {
                throw new Exception($"Tipo desconocido {currentToken}");
            }
        }

        private CardNode ParseCard()
        {
            Expect(TokenType.LeftBrace);

            while (Peek().type != TokenType.EndOfFile)
            {
                if (tokenHandlers.TryGetValue(currentToken.type, out Action handle))
                {
                    Advance();
                    handle();
                }
                else
                {
                    throw new Exception("Accion no valida");
                }
            }

            Expect(TokenType.RightBrace);

            return cardNode;
        }

        #region Handle

        private void HandleName()
        {
            Expect(TokenType.Colon);
            string value = ParseString();
            cardNode.name = value;
            Match(TokenType.Comma);
        }

        private void HandleType()
        {
            Expect(TokenType.Colon);
            string value = ParseString();
            cardNode.type = value;
            Match(TokenType.Comma);
        }

        private void HandleFaction()
        {
            Expect(TokenType.Colon);
            string value = ParseString();
            cardNode.faction = value;
            Match(TokenType.Comma);
        }

        private void HandlePower()
        {
            Expect(TokenType.Colon);
            int value = ParseInt();
            cardNode.power = value;
            Match(TokenType.Comma);
        }

        private void HandleRange()
        {
            Expect(TokenType.Colon);
            Expect(TokenType.LeftBracket);

            List<string> range = new List<string>();
            while (!Match(TokenType.RightBracket))
            {
                range.Add(ParseString());
                Match(TokenType.Comma);
            }
            cardNode.range = range;
            Match(TokenType.Comma);
        }

        private void HandleOnActivation()
        {
            Expect(TokenType.Colon);
            Expect(TokenType.LeftBracket);
            OnActivation onActivation = ParseOnActivation();
            cardNode.onActivation = onActivation;
            Match(TokenType.Comma);
        }

        #endregion

        private EffectNode ParseEffect()
        {
            return new EffectNode();
        }

        private OnActivation ParseOnActivation()
        {
            return null;
        }

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
            if (currentToken.type != TokenType.Number)
            {
                throw new Exception($"Se esperaba un número, pero se encontró {currentToken.type}");
            }
            if (!int.TryParse(currentToken.value, out int value))
            {
                throw new Exception($"Valor de número inválido: {currentToken.value}");
            }

            Advance();
            return value;
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