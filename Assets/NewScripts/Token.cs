namespace Console
{
    public enum TypeOfToken
    {
        unhown_Token,
        number_Token,//number
        identifier_Token,//identifier
        if_Token,//if
        else_Token,//else
        for_Token,//for
        while_Token,//while
        foreach_Token,//foreach
        int_Token,//int
        string_Token,//string
        bool_Token,//bool
        float_Token,//float
        openParenthesis_Token,//(
        closeParenthesis_Token,//)
        openBracket_Token,//[
        closeBracket_Token,//]
        openBrace_Token,//{
        closeBrace_Token,//}
        comma_Token,//,
        dot_Token,//.
        colon_Token,//:
        semicolon_Token,//;
        equal_Token,//=
        plus_Token,//+
        minus_Token,//-
        multiply_Token,// *
        divide_Token,// /
        percent_Token,//%
        greaterThan_Token,//>
        lessThan_Token,//<
        greaterEqual_Token,//>=
        lessEqual_Token,//<=
        notEqual_Token,//!=
        equalEqual_Token,//==
        not_Token,//!
        and_Token,//&&
        or_Token,//|
        new_Token,//new
        class_Token,//class/no
        public_Token,//public/no
        private_Token,//private/no
        static_Token,//static/no
        void_Token,//void/no
        return_Token,//return/no
        break_Token,//break/no
        continue_Token,//continue/no
        true_Token,//true/no
        false_Token,//false/no
        this_Token,//this/no
        null_Token,//null/no
        get_Token,//get/no
        set_Token,//set/no
    }

    public class Token
    {
        public TypeOfToken typeOfToken;
        public string value;
        public Token(TypeOfToken typeOfToken, string value)
        {
            this.typeOfToken = typeOfToken;
            this.value = value;
        }

    }
}