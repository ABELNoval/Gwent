using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Console
{
    public class Lexer
    {
        private string _input;
        private int _position;
        private int _line;

        private static readonly Dictionary<string, TypeOfToken> _specialCharacters = new Dictionary<string, TypeOfToken>
        {
            {"(", TypeOfToken.openParenthesis_Token},
            {")", TypeOfToken.closeParenthesis_Token},
            { "{", TypeOfToken.openBrace_Token},
            { "}", TypeOfToken.closeBrace_Token},
            { "[", TypeOfToken.openBrace_Token},
            { "]", TypeOfToken.closeBrace_Token},
            { ",", TypeOfToken.comma_Token},
            { ".", TypeOfToken.dot_Token},
            { ";", TypeOfToken.semicolon_Token},
            { ":", TypeOfToken.colon_Token},
            { "+", TypeOfToken.plus_Token},
            { "-", TypeOfToken.minus_Token},
            { "*", TypeOfToken.multiply_Token},
            { "/", TypeOfToken.divide_Token},
            { "%", TypeOfToken.percent_Token},
            { "&&", TypeOfToken.and_Token},
            { "||", TypeOfToken.or_Token},
            { "!", TypeOfToken.not_Token},
            { "=", TypeOfToken.equal_Token},
            { "<", TypeOfToken.lessThan_Token},
            { ">", TypeOfToken.greaterThan_Token},
            { "<=", TypeOfToken.lessEqual_Token},
            { ">=", TypeOfToken.greaterEqual_Token},
            { "new", TypeOfToken.new_Token},
            { "if", TypeOfToken.if_Token},
            { "while", TypeOfToken.while_Token},
            { "else", TypeOfToken.else_Token},
            { "for", TypeOfToken.for_Token},
            { "foreach", TypeOfToken.foreach_Token},
            { "int", TypeOfToken.int_Token},
            { "string", TypeOfToken.string_Token},
            { "bool", TypeOfToken.bool_Token},
            {"float", TypeOfToken.float_Token}
        };

        public Lexer(string input)
        {
            _input = input;
            _position = 0;
        }

        public List<Token> GetTokenList()
        {
            List<Token> tokens = new List<Token>();
            Token token;
            while (_position < _input.Length)
            {
                token = GetToken();
                tokens.Add(token);
            }
            return tokens;
        }

        private Token GetToken()
        {
            if (_position >= _input.Length)
            {
                return new Token(TypeOfToken.unhown_Token, " ");
            }
            char current = _input[_position];

            // Ignorar espacio en blanco
            if (char.IsWhiteSpace(current))
            {
                _position++;
                return GetToken();
            }

            // Detectar cuando hay un operador de dos caracteres
            string twoCharOperator = _position < _input.Length - 1 ? _input.Substring(_position, 2) : null;

            // Detectar si el caracter doble es un caracter especial
            if (twoCharOperator != null && _specialCharacters.ContainsKey(twoCharOperator))
            {
                _position += 2;
                return new Token(_specialCharacters[twoCharOperator], twoCharOperator);
            }

            // Si no es un caracter doble, definimos el caracter actual como string para verificar si es especial
            string one_Char_Operator = current.ToString();

            // Detectar si ese caracter es especial
            if (_specialCharacters.ContainsKey(one_Char_Operator))
            {
                _position++;
                return new Token(_specialCharacters[one_Char_Operator], one_Char_Operator);
            }

            // Detectar si es un numero
            if (char.IsDigit(current))
            {
                return GetNumberToken();
            }

            // Detectar si es una palabra no especial
            if (char.IsLetter(current) || current == '_')
            {
                return GetIdentifierToken();
            }

            _position++;

            // Devolver carcater deconocido
            return new Token(TypeOfToken.unhown_Token, current.ToString());
        }

        // Obtener todos los caracteres consecutivos que son digitos
        private Token GetNumberToken()
        {
            int start = _position;
            while (_position < _input.Length && char.IsDigit(_input[_position]))
            {
                _position++;
            }

            string value = _input.Substring(start, _position - start);
            if (_specialCharacters.ContainsKey(value))
            {
                return new Token(_specialCharacters[value], value);
            }

            return new Token(TypeOfToken.number_Token, value);
        }

        // Obtener todos los caracteres consecutivos que son letras o digitos(buscando hasta donde llega la variable)
        private Token GetIdentifierToken()
        {
            int start = _position;
            while (_position < _input.Length && (char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
            {
                _position++;
            }

            string value = _input.Substring(start, _position - start);
            if (_specialCharacters.ContainsKey(value))
            {
                return new Token(_specialCharacters[value], value);
            }
            return new Token(TypeOfToken.identifier_Token, value);
        }
    }
}