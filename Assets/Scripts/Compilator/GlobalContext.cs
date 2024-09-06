using System;
using System.Collections.Generic;

namespace Console
{
    public class GlobalContext
    {
        private Dictionary<string, Type> symbols;
        private Dictionary<string, object> variables;
        public GlobalContext parentContext;

        public GlobalContext(GlobalContext parent = null)
        {
            symbols = new Dictionary<string, Type>();
            variables = new Dictionary<string, object>();
            parentContext = parent;
        }

        public void DefineSymbol(string name, Type type)
        {
            if (ConteinsSymbol(name) && symbols[name] != type)
                throw new Exception("Variable ya definida con otro valor");
            symbols[name] = type;
        }

        public void DefineVariable(string name, object value)
        {
            variables[name] = value;
        }

        public Type LookupSymbol(string name)
        {
            if (symbols.TryGetValue(name, out Type element))
                return element;

            return parentContext?.LookupSymbol(name);
        }

        public object LookupVariable(string name)
        {
            if (variables.TryGetValue(name, out object value))
                return value;

            return parentContext?.LookupVariable(name);
        }

        public bool ConteinsSymbol(string name)
        {
            return symbols.ContainsKey(name);
        }

        public bool ConteinsVariable(string name)
        {
            return variables.ContainsKey(name);
        }
    }
}