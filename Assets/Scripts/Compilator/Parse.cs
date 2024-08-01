using System.Collections.Generic;

namespace Console
{
    public class Parse
    {
        private List<Token> input;
        private int position;
        private int line;

        public Parse(List<Token> input)
        {
            this.input = input;
            position = 0;
        }


    }
}