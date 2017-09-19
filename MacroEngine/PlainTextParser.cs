using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEngine
{
    class PlainTextParser : IParser
    {
        private int GetNextControlSymbol(StringBuilder source)
        {
            for (int i = 1; i < source.Length; ++i)
            {
                if ((source[i] == '#' || source[i] == '$') && source[i - 1] != '\\')
                {
                    return i;
                }
            }

            return -1;
        }

        public IMacroElement Parse(ref StringBuilder source)
        {
            IMacroElement toReturn = null;

            int last_index = GetNextControlSymbol(source);

            if (last_index != -1)
            {
                string buf = source.ToString();

                toReturn = new PlainTextMacro(buf.Substring(0, last_index));

                source = new StringBuilder(buf.Substring(last_index));
            }
            else
            {
                toReturn = new PlainTextMacro(source.ToString());

                source = new StringBuilder();
            }

            return toReturn;
        }
    }
}
