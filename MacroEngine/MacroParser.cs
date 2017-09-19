using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEngine
{
    class MacroParser : IParser
    {

        public IMacroElement Parse(ref StringBuilder source)
        {
            IMacroElement toReturn = null;

            int index = GetFirstOuterElemPosition(source);

            string buf = source.ToString();

            string name = buf.Substring(1, buf.IndexOf('(') - 1);

            string variableList = buf.Substring(buf.IndexOf('(') + 1, buf.IndexOf(')') - (buf.IndexOf('(') + 1));

            Dictionary<string, string> replaced = LogicManager.GetStringReplacers(ref variableList, 0, variableList.Length);

            List<string> variables = variableList.Split(' ').ToList();

            variables.RemoveAll(x => x == "");

            foreach(var kvp in replaced)
            {
                for(int i = 0; i < variables.Count; ++i)
                {
                    if(variables[i] == kvp.Key)
                    {
                        variables[i] = kvp.Value;
                    }
                }
            }

            source = new StringBuilder(buf.Substring(index));

            toReturn = new MacroCall(name, variables.ToArray());

            return toReturn;
        }

        private int GetFirstOuterElemPosition(StringBuilder source)
        {
            for(int i = 0; i < source.Length; ++i)
            {
                if(source[i] == ')')
                {
                    return i + 1;
                }
            }

            throw new ArgumentException("Macro parser: end of parser was not detected");
        }
    }
}
