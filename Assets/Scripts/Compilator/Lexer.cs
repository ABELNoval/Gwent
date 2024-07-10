using System.Collections.Generic;

namespace Console
{
    public class Lexer
    {
        private string input;
        private int position;
        private int line;

        private static readonly Dictionary<string, TypeOfToken> specialCharacters = new Dictionary<string, TypeOfToken>
        {
            {"(", TypeOfToken.openParenthesisToken},
            {")", TypeOfToken.closeParenthesisToken},
            { "{", TypeOfToken.openBraceToken},
            { "}", TypeOfToken.closeBraceToken},
            { "[", TypeOfToken.openBraceToken},
            { "]", TypeOfToken.closeBraceToken},
            { ",", TypeOfToken.commaToken},
            { ".", TypeOfToken.dotToken},
            { ";", TypeOfToken.semicolonToken},
            { ":", TypeOfToken.colonToken},
            { "+", TypeOfToken.plusToken},
            { "-", TypeOfToken.minusToken},
            { "*", TypeOfToken.multiplyToken},
            { "/", TypeOfToken.divideToken},
            { "%", TypeOfToken.percentToken},
            { "&&", TypeOfToken.andToken},
            { "||", TypeOfToken.orToken},
            { "!", TypeOfToken.notToken},
            { "=", TypeOfToken.equalToken},
            { "<", TypeOfToken.lessThanToken},
            { ">", TypeOfToken.greaterThanToken},
            { "<=", TypeOfToken.lessEqualToken},
            { ">=", TypeOfToken.greaterEqualToken},
            { "new", TypeOfToken.newToken},
            { "if", TypeOfToken.ifToken},
            { "while", TypeOfToken.whileToken},
            { "else", TypeOfToken.elseToken},
            { "for", TypeOfToken.forToken},
            { "foreach", TypeOfToken.foreachToken},
            { "int", TypeOfToken.intToken},
            { "string", TypeOfToken.stringToken},
            { "bool", TypeOfToken.boolToken},
            {"float", TypeOfToken.floatToken}
        };

        public Lexer(string input)
        {
            this.input = input;
            position = 0;
        }

        public List<Token> GetTokenList()
        {
            List<Token> tokens = new List<Token>();
            Token token;
            while (position < input.Length)
            {
                token = GetToken();
                tokens.Add(token);
            }
            return tokens;
        }

        private Token GetToken()
        {
            if (position >= input.Length)
            {
                return new Token(TypeOfToken.unhownToken, " ");
            }
            char current = input[position];

            // Ignorar espacio en blanco
            if (char.IsWhiteSpace(current))
            {
                position++;
                return GetToken();
            }

            // Detectar cuando hay un operador de dos caracteres
            string twoCharOperator = position < input.Length - 1 ? input.Substring(position, 2) : null;

            // Detectar si el caracter doble es un caracter especial
            if (twoCharOperator != null && specialCharacters.ContainsKey(twoCharOperator))
            {
                position += 2;
                return new Token(specialCharacters[twoCharOperator], twoCharOperator);
            }

            // Si no es un caracter doble, definimos el caracter actual como string para verificar si es especial
            string oneCharOperator = current.ToString();

            // Detectar si ese caracter es especial
            if (specialCharacters.ContainsKey(oneCharOperator))
            {
                position++;
                return new Token(specialCharacters[oneCharOperator], oneCharOperator);
            }

            // Detectar si es un numero
            if (char.IsDigit(current))
            {
                return GetNumberToken();
            }

            // Detectar si es una palabra no especial
            if (char.IsLetter(current) || current == ' ')
            {
                return GetIdentifierToken();
            }

            position++;

            // Devolver carcater deconocido
            return new Token(TypeOfToken.unhownToken, current.ToString());
        }

        // Obtener todos los caracteres consecutivos que son digitos
        private Token GetNumberToken()
        {
            int start = position;
            while (position < input.Length && char.IsDigit(input[position]))
            {
                position++;
            }

            string value = input.Substring(start, position - start);
            if (specialCharacters.ContainsKey(value))
            {
                return new Token(specialCharacters[value], value);
            }

            return new Token(TypeOfToken.numberToken, value);
        }

        // Obtener todos los caracteres consecutivos que son letras o digitos(buscando hasta donde llega la variable)
        private Token GetIdentifierToken()
        {
            int start = position;
            while (position < input.Length && (char.IsLetterOrDigit(input[position]) || input[position] == ' '))
            {
                position++;
            }

            string value = input.Substring(start, position - start);
            if (specialCharacters.ContainsKey(value))
            {
                return new Token(specialCharacters[value], value);
            }
            return new Token(TypeOfToken.identifierToken, value);
        }
    }
}