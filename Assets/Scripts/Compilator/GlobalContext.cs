using System;
using System.Collections.Generic;

namespace Console
{
    public class GlobalContext
    {
        private Dictionary<string, (Type, object)> symbols;
        public GlobalContext parentContext;

        public GlobalContext(GlobalContext parent = null)
        {
            symbols = new Dictionary<string, (Type, object)>();
            parentContext = parent;
        }

        public void DefineSymbol(string name, Type type, object value)
        {
            symbols[name] = (type, value);
        }

        public (Type, object) LookupSymbol(string name)
        {
            if (symbols.TryGetValue(name, out (Type, object) element))
                return element;

            return ((Type, object))(parentContext?.LookupSymbol(name));
        }

        public bool ConteinsSymbol(string name)
        {
            return symbols.ContainsKey(name);
        }
    }
}