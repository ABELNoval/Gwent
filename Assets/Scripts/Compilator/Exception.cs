
using System;

namespace Console
{
    public class CompilationException : Exception
    {
        public int Line { get; private set; }
        public string Element { get; set; }

        public CompilationException(string message, int line, string element) : base(message)
        {
            Line = line;
            Element = element;
        }
    }

    public class ParsingException : CompilationException
    {
        public ParsingException(string message, int line, string element) : base(message, line, element) { }


    }

    public class ValidateException : CompilationException
    {
        public ValidateException(string message, int line, string element) : base(message, line, element) { }


    }

    public class LexerException : CompilationException
    {
        public LexerException(string message, int line, string element) : base(message, line, element) { }


    }
}
