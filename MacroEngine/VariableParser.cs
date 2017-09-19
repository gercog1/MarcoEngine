using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEngine
{
    class VariableParser : IParser
    {
        public IMacroElement Parse(ref StringBuilder source)
        {
            IMacroElement toReturn = null;

            int index = GetTerminator(source);

            if(index != -1)
            {
                string buf = source.ToString();

                string variable = buf.Substring(1, index-1); // omit $

                List<string> parts = variable.Split('.').ToList();

                parts.RemoveAll(x => x == "");

                string name = parts[0];
                
                if(parts.Count > 1)
                {
                    parts = parts.Skip(1).ToList();
                }
                else
                {
                    parts = null;
                }

                toReturn = new VariableMacro(name, parts);

                source = new StringBuilder(buf.Substring(index));
            }
            else
            {
                string buf = source.ToString();

                string variable = buf.Substring(1); // omit $

                List<string> parts = variable.Split('.').ToList();

                parts.RemoveAll(x => x == "");

                string name = parts[0];

                if (parts.Count > 1)
                {
                    parts = parts.Skip(1).ToList();
                }
                else
                {
                    parts = null;
                }

                toReturn = new VariableMacro(name, parts);

                source = new StringBuilder();
            }

            return toReturn;
        }

        private int GetTerminator(StringBuilder source)
        {
            for (int i = 1; i < source.Length; ++i)
            {
                if(
                   !char.IsLetterOrDigit(source[i]) &&
                   !(source[i] == '.' && 
                   i < source.Length - 1 &&
                   char.IsLetter(source[i + 1]))
                    )
                {
                    return i;
                }
            }

                return -1;
        }
    }
}
