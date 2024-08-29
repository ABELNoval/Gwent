using System;
using System.Collections.Generic;
using System.Text;

namespace Console
{
    // Implementación del lexer
    public class Lexer
    {
        private string input;
        private int position;
        private char currentCharacter;

        public Lexer(string input)
        {
            this.input = input;
            position = 0;
            currentCharacter = input[position];
        }

        public List<Token> Analyze()
        {
            List<Token> tokens = new List<Token>();

            while (position < input.Length - 1)
            {
                switch (currentCharacter)
                {
                    case '{':
                        tokens.Add(new Token(TokenType.LeftBrace, "{"));
                        Advance();
                        break;
                    case '}':
                        tokens.Add(new Token(TokenType.RightBrace, "}"));
                        Advance();
                        break;
                    case '(':
                        tokens.Add(new Token(TokenType.LeftParenthesis, "("));
                        Advance();
                        break;
                    case ')':
                        tokens.Add(new Token(TokenType.RightParenthesis, ")"));
                        Advance();
                        break;
                    case '[':
                        tokens.Add(new Token(TokenType.LeftBracket, "["));
                        Advance();
                        break;
                    case ']':
                        tokens.Add(new Token(TokenType.RightBracket, "]"));
                        Advance();
                        break;

                    case ':':
                        tokens.Add(new Token(TokenType.Colon, ":"));
                        Advance();
                        break;
                    case ',':
                        tokens.Add(new Token(TokenType.Comma, ","));
                        Advance();
                        break;
                    case ';':
                        tokens.Add(new Token(TokenType.Semicolon, ";"));
                        Advance();
                        break;
                    case '.':
                        tokens.Add(new Token(TokenType.Dot, "."));
                        Advance();
                        break;

                    case '+':
                        if (Peek() == '+')
                        {
                            tokens.Add(new Token(TokenType.Increment, "++"));
                            Advance();
                            Advance();
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Plus, "+"));
                            Advance();
                        }
                        break;
                    case '-':
                        if (Peek() == '-')
                        {
                            tokens.Add(new Token(TokenType.Decrement, "--"));
                            Advance();
                            Advance();
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Minus, "-"));
                            Advance();
                        }
                        break;
                    case '*':
                        tokens.Add(new Token(TokenType.Multiply, "*"));
                        Advance();
                        break;
                    case '^':
                        tokens.Add(new Token(TokenType.Exponent, "^"));
                        Advance();
                        break;
                    case '/':
                        if (Peek() == '/')
                        {
                            Advance();
                            Advance();
                            string comment = ReadWhile(c => c != '\n');
                            tokens.Add(new Token(TokenType.CommentLine, comment));
                        }
                        else if (Peek() == '*')
                        {
                            Advance();
                            Advance();
                            string comment = ReadUntil("*/");
                            tokens.Add(new Token(TokenType.CommentBlock, comment));
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Divide, "/"));
                            Advance();
                        }
                        break;
                    case '=':
                        if (Peek() == '=')
                        {
                            tokens.Add(new Token(TokenType.Equals, "=="));
                            Advance();
                            Advance();
                        }
                        else if (Peek() == '>')
                        {
                            tokens.Add(new Token(TokenType.Arrow, "=>"));
                            Advance();
                            Advance();
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Assign, "="));
                            Advance();
                        }
                        break;
                    case '!':
                        if (Peek() == '=')
                        {
                            tokens.Add(new Token(TokenType.NotEquals, "!="));
                            Advance();
                            Advance();
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Not, "!"));
                            Advance();
                        }
                        break;
                    case '<':
                        if (Peek() == '=')
                        {
                            tokens.Add(new Token(TokenType.LessThanOrEqual, "<="));
                            Advance();
                            Advance();
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.LessThan, "<"));
                            Advance();
                        }
                        break;
                    case '>':
                        if (Peek() == '=')
                        {
                            tokens.Add(new Token(TokenType.GreaterThanOrEqual, ">="));
                            Advance();
                            Advance();
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.GreaterThan, ">"));
                            Advance();
                        }
                        break;
                    case '&':
                        if (Peek() == '&')
                        {
                            tokens.Add(new Token(TokenType.LogicalAnd, "&&"));
                            Advance();
                            Advance();
                        }
                        else
                        {
                            throw new Exception($"Unexpected character: {currentCharacter}");
                        }
                        break;
                    case '|':
                        if (Peek() == '|')
                        {
                            tokens.Add(new Token(TokenType.LogicalOr, "||"));
                            Advance();
                            Advance();
                        }
                        else
                        {
                            throw new Exception($"Unexpected character: {currentCharacter}");
                        }
                        break;
                    case '"':
                        Advance();
                        string str = ReadWhile(c => c != '"');
                        tokens.Add(new Token(TokenType.StringLiteral, str));
                        Advance();
                        break;

                    default:
                        if (char.IsWhiteSpace(currentCharacter))
                        {
                            Advance();
                        }
                        else if (char.IsDigit(currentCharacter))
                        {
                            tokens.Add(new Token(TokenType.NumberLiteral, ReadNumber()));
                        }
                        else if (char.IsLetter(currentCharacter))
                        {
                            string palabra = ReadWord();
                            tokens.Add(IdentifyKeywordOrIdentifier(palabra));
                        }
                        else
                        {
                            throw new Exception($"Caracter inesperado: {currentCharacter}");
                        }
                        break;
                }
            }

            tokens.Add(new Token(TokenType.EndOfFile, "fin"));

            return tokens;
        }

        private void Advance()
        {
            position++;
            if (position >= input.Length)
            {
                currentCharacter = '\0'; // Indicador de fin de entrada
            }
            else
            {
                currentCharacter = input[position];
            }
        }

        private char Peek()
        {
            return position + 1 >= input.Length ? '\0' : input[position + 1];
        }

        private string ReadNumber()
        {
            string numero = string.Empty;

            while (char.IsDigit(currentCharacter))
            {
                numero += currentCharacter;
                Advance();
            }

            return numero;
        }

        private string ReadWord()
        {
            string palabra = string.Empty;

            while (char.IsLetter(currentCharacter))
            {
                palabra += currentCharacter;
                Advance();
            }

            return palabra;
        }

        private string ReadWhile(Func<char, bool> condition)
        {
            StringBuilder sb = new StringBuilder();

            while (position < input.Length && condition(currentCharacter))
            {
                sb.Append(currentCharacter);
                Advance();
            }

            return sb.ToString();
        }

        private string ReadUntil(string delimiter)
        {
            StringBuilder sb = new StringBuilder();

            while (!input.Substring(position).StartsWith(delimiter) && position < input.Length)
            {
                sb.Append(currentCharacter);
                Advance();
            }

            position += delimiter.Length;
            currentCharacter = position < input.Length ? input[position] : '\0';
            return sb.ToString();
        }

        private Token IdentifyKeywordOrIdentifier(string identifier)
        {
            switch (identifier)
            {
                case "card":
                    return new Token(TokenType.card, identifier);
                case "effect":
                    return new Token(TokenType.effect, identifier);

                case "Name":
                    return new Token(TokenType.Name, identifier);
                case "Power":
                    return new Token(TokenType.Power, identifier);
                case "Type":
                    return new Token(TokenType.Type, identifier);
                case "Faction":
                    return new Token(TokenType.Faction, identifier);
                case "Range":
                    return new Token(TokenType.Range, identifier);
                case "OnActivation":
                    return new Token(TokenType.OnActivation, identifier);
                case "Effect":
                    return new Token(TokenType.Effect, identifier);
                case "Selector":
                    return new Token(TokenType.Selector, identifier);
                case "Source":
                    return new Token(TokenType.Source, identifier);
                case "Single":
                    return new Token(TokenType.Single, identifier);
                case "Predicate":
                    return new Token(TokenType.Predicate, identifier);
                case "Params":
                    return new Token(TokenType.Params, identifier);
                case "PosAction":
                    return new Token(TokenType.PosAction, identifier);
                case "Action":
                    return new Token(TokenType.Action, identifier);
                case "targets":
                    return new Token(TokenType.Target, identifier);

                case "Hand":
                    return new Token(TokenType.Hand, identifier);
                case "HandofPlayer":
                    return new Token(TokenType.HandOfPlayer, identifier);
                case "Deck":
                    return new Token(TokenType.Deck, identifier);
                case "DeckOfPlayer":
                    return new Token(TokenType.DeckOfPlayer, identifier);
                case "Field":
                    return new Token(TokenType.Field, identifier);
                case "FieldOfPlayer":
                    return new Token(TokenType.FieldOfPlayer, identifier);
                case "Graveyard":
                    return new Token(TokenType.Graveyard, identifier);
                case "GraveyardOfPlayer":
                    return new Token(TokenType.GraveyardOfPlayer, identifier);

                case "Pop":
                    return new Token(TokenType.Pop, identifier);
                case "Push":
                    return new Token(TokenType.Push, identifier);
                case "SendBottom":
                    return new Token(TokenType.SendBottom, identifier);
                case "Remove":
                    return new Token(TokenType.Remove, identifier);
                case "Find":
                    return new Token(TokenType.Find, identifier);
                case "Shuffle":
                    return new Token(TokenType.Shuffle, identifier);

                case "for":
                    return new Token(TokenType.for_Token, identifier);
                case "while":
                    return new Token(TokenType.while_Token, identifier);
                case "if":
                    return new Token(TokenType.if_Token, identifier);
                case "foreach":
                    return new Token(TokenType.foreach_Token, identifier);
                case "in":
                    return new Token(TokenType.in_Token, identifier);
                case "false":
                    return new Token(TokenType.BooleanLiteral, identifier);
                case "true":
                    return new Token(TokenType.BooleanLiteral, identifier);

                case "string":
                    return new Token(TokenType.String, identifier);
                case "int":
                    return new Token(TokenType.Int, identifier);
                case "bool":
                    return new Token(TokenType.Boolean, identifier);
                case "Number":
                    return new Token(TokenType.Number, identifier);
                case "String":
                    return new Token(TokenType.Str, identifier);
                case "Bool":
                    return new Token(TokenType.Bool, identifier);

                default:
                    return new Token(TokenType.Identifier, identifier);
            }
        }
    }

}