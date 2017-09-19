using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEngine
{
    class LoopParser : IParser
    {

        private int GetFirstOuterElemPosition(StringBuilder source)
        {
            string buf = source.ToString();

            int counter = 1;

            for (int i = 1; i < source.Length; ++i)
            {
                if (buf[i] == '#' && buf[i - 1] != '\\')
                {
                    if (buf.Length - i >= 3 && buf.Substring(i, 4) == "#end")
                    {
                        --counter;

                        if (counter == 0)
                        {
                            return i + 4;
                        }
                    }
                    else if (buf.Length - i >= 2 && buf.Substring(i, 3) == "#if")
                    {
                        ++counter;
                    }
                    else if (buf.Length - i >= 7 && buf.Substring(i, 8) == "#foreach")
                    {
                        ++counter;
                    }
                }
            }

            throw new ArgumentException("Conditional parser: #end is unreachable");
        }

        public IMacroElement Parse(ref StringBuilder source)
        {
            int index = GetFirstOuterElemPosition(source);

            string buf = source.ToString();

            List<string> signature = buf.Substring(buf.IndexOf('(') + 1, buf.IndexOf(')') - (buf.IndexOf('(') + 1)).Split(' ').ToList();

            signature.RemoveAll(x => x == "");

            string item_name = signature[0];

            string container_name = signature[2];

            string body = buf.Substring(buf.IndexOf(')') + 1, index - (buf.IndexOf(')') + 5));

            IMacroElement toReturn = new LoopMacro(item_name, container_name, body);

            source = new StringBuilder(buf.Substring(index));

            return toReturn;

        }
    }
}
