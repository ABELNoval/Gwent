namespace Console
{
    public enum TypeOfToken
    {
        unhownToken,
        numberToken,//number
        identifierToken,//identifier
        ifToken,//if
        elseToken,//else
        forToken,//for
        whileToken,//while
        foreachToken,//foreach
        intToken,//int
        stringToken,//string
        boolToken,//bool
        floatToken,//float
        openParenthesisToken,//(
        closeParenthesisToken,//)
        openBracketToken,//[
        closeBracketToken,//]
        openBraceToken,//{
        closeBraceToken,//}
        commaToken,//,
        dotToken,//.
        colonToken,//:
        semicolonToken,//;
        equalToken,//=
        plusToken,//+
        minusToken,//-
        multiplyToken,// *
        divideToken,// /
        percentToken,//%
        greaterThanToken,//>
        lessThanToken,//<
        greaterEqualToken,//>=
        lessEqualToken,//<=
        notEqualToken,//!=
        equalEqualToken,//==
        notToken,//!
        andToken,//&&
        orToken,//|
        newToken,//new
        classToken,//class/no
        publicToken,//public/no
        privateToken,//private/no
        staticToken,//static/no
        voidToken,//void/no
        returnToken,//return/no
        breakToken,//break/no
        continueToken,//continue/no
        trueToken,//true/no
        falseToken,//false/no
        thisToken,//this/no
        nullToken,//null/no
        getToken,//get/no
        setToken,//set/no
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