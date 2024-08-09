

using System;

namespace Console
{
    public class CompilationException : Exception
    {
        public int line { get; private set; }
        public string element { get; set; }

        public CompilationException(string message, int line, string element) : base(message)
        {
            this.line = line;
            this.element = element;
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
