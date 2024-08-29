namespace Console
{
    public enum TokenType
    {
        //Palabras clave del juego
        card, effect, Name, Params, Action, Type, Power, Faction, Range, OnActivation,
        Selector, Effect, Amount, Single, Source, Predicate, PosAction, Target, Context,
        Board, TriggerPlayer, HandOfPlayer, FieldOfPLayer, GraveyardOfPlayer, DeckOfPlayer,
        Hand, Deck, Field, Graveyard, Owner, Find, Shuffle, Pop, Push, SendBottom, Remove,

        //Palabras claves extras
        for_Token, while_Token, foreach_Token, if_Token, in_Token,

        // Operators (Operadores)
        Plus, Minus, Multiply, Divide, Exponent, Assign, Equals, NotEquals, LessThan, GreaterThan, LessThanOrEqual,
        GreaterThanOrEqual, LogicalAnd, LogicalOr, Not, Dot, Increment, Decrement, Arrow,

        // Punctuation (Puntuacion)
        LeftParenthesis, RightParenthesis, LeftBrace, RightBrace, LeftBracket, RightBracket, Comma, Semicolon, Colon,
        Quote,

        // Literals (Literales)
        NumberLiteral, StringLiteral, BooleanLiteral, CommentLine, CommentBlock,

        // Types (Tipos)
        Identifier, Number, Str, Bool, String, Boolean, Int,

        // End of Input (Fin de entrada)
        EndOfFile
    }

    public class Token
    {
        public TokenType type;
        public string value;
        public Token(TokenType type, string value)
        {
            this.type = type;
            this.value = value;
        }
    }
}