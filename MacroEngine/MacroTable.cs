using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEngine
{
    public class MacroTable
    {
        private Dictionary<Tuple<string, int>, string> macroSignatures;

        public MacroTable()
        {
            macroSignatures = new Dictionary<Tuple<string, int>, string>();
        }

        public void AddMacros(string name, int paramNumber, string definition)
        {
            Tuple<string, int> key = new Tuple<string, int>(name, paramNumber);

            macroSignatures[key] = definition;
        }

        public KeyValuePair<Tuple<string, int>, string> GetMacros(string name, int paramNumber)
        {
            foreach(var kvp in macroSignatures)
            {
                if(kvp.Key.Item1.Substring(0, name.Length) == name)
                {
                    return kvp;
                }
            }

            return new KeyValuePair<Tuple<string, int>, string>(new Tuple<string, int>("", -1), "");
        }
    }
}
