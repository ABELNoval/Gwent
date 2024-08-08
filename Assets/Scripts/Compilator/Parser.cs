using System.Collections.Generic;
using System;

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
        private Dictionary<TokenType, Action<ProgramNode>> effectDataTokenHandlers;
        private Dictionary<TokenType, Action<ProgramNode>> onActValueTokenHandlers;
        private Dictionary<TokenType, Action<ProgramNode>> selectorTokenHandler;
        private Dictionary<TokenType, Action<ProgramNode>> posActionTokenHandler;
        private Dictionary<TokenType, Action<ProgramNode>> actionTokenHandler;
        public ProgramNode currretnNode;

        public Parser(List<Token> input)
        {
            this.input = input;
            position = 0;
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
                {TokenType.Amount, HandlePower}
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
                {TokenType.Target, HandleReflection},
                {TokenType.Context, HandleReflection}
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

        #endregion


        #region Handle

        private void HandleName(ProgramNode node)
        {
            UnityEngine.Debug.Log("Name");
            Expect(TokenType.Colon);
            node.SetName(ParseString());
            Match(TokenType.Comma);
        }

        private void HandleType(ProgramNode node)
        {
            UnityEngine.Debug.Log("Type");
            Expect(TokenType.Colon);
            node.SetType(ParseString());
            Match(TokenType.Comma);
        }

        private void HandleFaction(ProgramNode node)
        {
            UnityEngine.Debug.Log("Faction");
            Expect(TokenType.Colon);
            node.SetFaction(ParseString());
            Match(TokenType.Comma);
        }

        private void HandlePower(ProgramNode node)
        {
            UnityEngine.Debug.Log("Power");
            Expect(TokenType.Colon);
            node.SetInt(ParseInt());
            Match(TokenType.Comma);
        }

        private void HandleRange(ProgramNode node)
        {
            UnityEngine.Debug.Log("Range");
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
            UnityEngine.Debug.Log("OnActivation");
            Expect(TokenType.Colon);
            Expect(TokenType.LeftBracket);

            OnActivationNode onActivationNode = new OnActivationNode();

            while (currentToken.type != TokenType.RightBracket)
            {
                ProgramNode actValueNode = ParseNode(new OnActValueNode(), onActValueTokenHandlers);
                onActivationNode.AddOnActValue(actValueNode as OnActValueNode);

                Match(TokenType.Comma);
            }

            Expect(TokenType.RightBracket);
            node.SetOnActivation(onActivationNode);
            Match(TokenType.Comma);
        }

        private void HandleEffectData(ProgramNode node)
        {
            UnityEngine.Debug.Log("EffectData");
            Expect(TokenType.Colon);
            ProgramNode value = new();
            EffectDataNode val = new();

            if (currentToken.type == TokenType.String)
            {
                val.SetName(ParseString());
                node.SetEffectDataNode(val);
            }
            else
            {
                value = ParseNode(new EffectDataNode(), effectDataTokenHandlers);
                node.SetEffectDataNode(value as EffectDataNode);
            }

            Match(TokenType.Comma);
        }

        private void HandleSelector(ProgramNode node)
        {
            UnityEngine.Debug.Log("Selector");
            Expect(TokenType.Colon);
            ProgramNode value = ParseNode(new SelectorNode(), selectorTokenHandler);
            node.SetSelector(value as SelectorNode);
            Match(TokenType.Comma);
        }

        private void HandlePosAction(ProgramNode node)
        {
            UnityEngine.Debug.Log("PosAction");
            Expect(TokenType.Colon);
            ProgramNode value = ParseNode(new PosActionNode(), posActionTokenHandler);
            node.SetPosAction(value as PosActionNode);
            Match(TokenType.Comma);
        }

        private void HandleSource(ProgramNode node)
        {
            UnityEngine.Debug.Log("Source");
            Expect(TokenType.Colon);
            node.SetSource(ParseString());
            Match(TokenType.Comma);
        }

        private void HandleSingle(ProgramNode node)
        {
            UnityEngine.Debug.Log("Single");
            Expect(TokenType.Colon);
            node.SetSingle(ParseBool());
            Match(TokenType.Comma);
        }

        private void HandlePredicate(ProgramNode node)
        {
            UnityEngine.Debug.Log("Predicate");
            Expect(TokenType.Colon);
            node.SetPredicate(ParseString());
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

            Expect(TokenType.LeftBrace);

            node.SetAction(ParseNode(new ActionNode(), actionTokenHandler) as ActionNode);
            Match(TokenType.Comma);
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