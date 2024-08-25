using System;
using System.Collections.Generic;

namespace Gwent_Proyect.Assets.Scripts.Compilator
{
    public class GlobalContext
    {
        private Dictionary<string, (Type, object)> symbols;
        private GlobalContext parentContext;

        public GlobalContext(GlobalContext parent = null)
        {
            symbols = new Dictionary<string, (Type, object)>();
            parentContext = parent;
        }

        public void DefineSymbol(string name, Type type, object value)
        {
            if (symbols.ContainsKey(name))
                throw new Exception($"Symbol {name} already defined.");
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