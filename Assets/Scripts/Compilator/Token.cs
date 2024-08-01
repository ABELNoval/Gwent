namespace Console
{
    public enum TokenType
    {
        //Palabras clave del juego
        card, effect, Name, Params, Action, Type, Power, Faction, Range, OnActivation,
        Selector, Effect, Amount, Single, Source, Predicate, PosAction,

        //Palabras claves extras
        for_Token, while_Token, foreach_Token, if_Token, in_Token,

        // Operators (Operadores)
        Plus, Minus, Multiply, Divide, Assign, Equals, NotEquals, LessThan, GreaterThan, LessThanOrEqual,
        GreaterThanOrEqual, LogicalAnd, LogicalOr, Not, Dot, Increment, Decrement,

        // Punctuation (Puntuacion)
        LeftParenthesis, RigthParenthesis, LeftBrace, RightBrace, LeftBracket, RightBracket, Comma, Semicolon, Colon,
        Quote,

        // Literals (Literales)
        Number, String, Boolean, CommentLine, CommentBlock,

        // Identifiers (Identificadores)
        Identifier,

        // End of Input (Fin de entrada)
        EndOfFile
    }

    public class Token
    {
        public TokenType tokenType;
        public string value;
        public Token(TokenType tokenType, string value)
        {
            this.tokenType = tokenType;
            this.value = value;
        }
    }
}