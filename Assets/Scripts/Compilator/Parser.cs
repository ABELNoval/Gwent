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
        private Dictionary<TokenType, Action<ProgramNode>> cardTokenHandlers;
        private Dictionary<TokenType, Action<ProgramNode>> effectTokenHandlers;
        private Dictionary<TokenType, Action<ProgramNode>> onActivationTokenHandlers;
        public ProgramNode currretnNode;

        public Parser(List<Token> input)
        {
            this.input = input;
            position = 0;
            currentToken = input[position];
            cardTokenHandlers = new Dictionary<TokenType, Action<ProgramNode>>(){
                {TokenType.Name, HandleName},
                {TokenType.Type, HandleType},
                {TokenType.Faction, HandleFaction},
                {TokenType.Power, HandlePower},
                {TokenType.Range, HandleRange},
                {TokenType.OnActivation, HandleOnActivation}
            };

            effectTokenHandlers = new Dictionary<TokenType, Action<ProgramNode>>(){
                {TokenType.Name, HandleName},
            };

            onActivationTokenHandlers = new Dictionary<TokenType, Action<ProgramNode>>()
            {

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

            while (Peek().type != TokenType.EndOfFile && Peek().type != TokenType.RightBrace)
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

            Expect(TokenType.RightBrace);
            node.Validate();

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

        #endregion


        #region Handle

        private void HandleName(ProgramNode node)
        {
            Expect(TokenType.Colon);
            string value = ParseString();
            node.SetName(value);
            Match(TokenType.Comma);
        }

        private void HandleType(ProgramNode node)
        {
            Expect(TokenType.Colon);
            string value = ParseString();
            node.SetType(value);
            Match(TokenType.Comma);
        }

        private void HandleFaction(ProgramNode node)
        {
            Expect(TokenType.Colon);
            string value = ParseString();
            node.SetFaction(value);
            Match(TokenType.Comma);
        }

        private void HandlePower(ProgramNode node)
        {
            Expect(TokenType.Colon);
            int value = ParseInt();
            node.SetPower(value);
            Match(TokenType.Comma);
        }

        private void HandleRange(ProgramNode node)
        {
            Expect(TokenType.Colon);
            Expect(TokenType.LeftBracket);

            while (!Match(TokenType.RightBracket))
            {
                node.AddRange(ParseString());
                Match(TokenType.Comma);
            }

            Match(TokenType.Comma);
        }

        private void HandleOnActivation(ProgramNode node)
        {
            Expect(TokenType.Colon);
            Expect(TokenType.LeftBracket);
            ProgramNode onActivation = ParseNode(new OnActivationNode(), onActivationTokenHandlers);
            node.SetOnActivation(onActivation as OnActivationNode);
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