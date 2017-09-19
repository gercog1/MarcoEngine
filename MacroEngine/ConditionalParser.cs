using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEngine
{
    class ConditionalParser : IParser
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

        private int GetClosingBracketIndex(string  source)
        {
            int curr_pos = source.IndexOf("(");
            int index = 1;

            while(index != 0)
            {
                ++curr_pos;

                if (curr_pos == source.Length)
                {
                    throw new ArgumentException("Conditional parser: wrong number of brackets in logical statement");
                }
                else
                {
                    if (source[curr_pos] == '(')
                    {
                        ++index;
                    }
                    else if (source[curr_pos] == ')')
                    {
                        --index;
                    }
                }
            }

            return curr_pos;
        }

        public IMacroElement Parse(ref StringBuilder source)
        {
            int index = GetFirstOuterElemPosition(source);

            if(index != -1)
            {
                string buf = source.ToString();

                List<string> parts = buf.Substring(0, index).Split('#').ToList();
                parts.RemoveAll(x => x == "");

                List<Tuple<string, string>> conditionalSignatures = new List<Tuple<string, string>>();
                List<string> conditionalBodies = new List<string>();

                foreach(string part in parts)
                {
                    if(part.Length >= 2 && part.Substring(0,2) == "if")
                    {
                        string signature = part.Substring(part.IndexOf('('), GetClosingBracketIndex(part) - (part.IndexOf('(') - 1));
                        Tuple<string, string> toAdd = new Tuple<string, string>("if", signature);

                        string body = part.Substring(GetClosingBracketIndex(part) + 1, part.Length - (GetClosingBracketIndex(part) + 1));

                        conditionalSignatures.Add(toAdd);
                        conditionalBodies.Add(body);
                    }
                    else if(part.Length >= 6 && part.Substring(0,6) == "elseif")
                    {
                        string signature = part.Substring(part.IndexOf('('), GetClosingBracketIndex(part) - (part.IndexOf('(') - 1));
                        Tuple<string, string> toAdd = new Tuple<string, string>("elseif", signature);

                        string body = part.Substring(GetClosingBracketIndex(part) + 1, part.Length - (GetClosingBracketIndex(part) + 1));

                        conditionalSignatures.Add(toAdd);
                        conditionalBodies.Add(body);
                    }
                    else if(part.Length >= 4 && part.Substring(0,4) == "else")
                    {
                        Tuple<string, string> toAdd = new Tuple<string, string>("else", "");

                        string body = part.Substring(part.IndexOf("else") + 4, part.Length - (part.IndexOf("else") + 4));

                        conditionalSignatures.Add(toAdd);
                        conditionalBodies.Add(body);
                    }
                }

                source = new StringBuilder(buf.Substring(index));

                return new ConditionalMacro(conditionalSignatures, conditionalBodies);
            }
            else
            {
                throw new ArgumentException("Conditional parser: #end not found");
            }
        }
    }
}
