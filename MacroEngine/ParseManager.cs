using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEngine
{
    class ParseManager
    {
        private static IParser GetParser(StringBuilder builder)
        {
            string check = builder.ToString();

            switch (builder[0])
            {
                case '$':
                    return new VariableParser();
                case '#':
                    if (builder.Length > 6 && check.Substring(1, 5) == "macro")
                    {
                        return null;
                    }
                    else if(builder.Length > 8 && check.Substring(1,7) == "foreach")
                    {
                        return new LoopParser();
                    }
                    else if(builder.Length > 3 && check.Substring(1,2) == "if")
                    {
                        return new ConditionalParser();
                    }
                    else
                    {
                        return new MacroParser();
                    }
                default:
                    return new PlainTextParser();
            }
        }

        private Tuple<Tuple<string, int>, string> GetMacroDefinition(ref StringBuilder source)
        {
            int index = FindFirstOuterSymbol(source);

            string buf = source.ToString();

            string signature = buf.Substring(buf.IndexOf('(') + 1, buf.IndexOf(')') - (buf.IndexOf('(') + 1));

            List<string> parameters = signature.Split(' ').ToList();
            parameters.RemoveAll(x => x == "");

            string name = parameters[0];

            string[] variables = parameters.Skip(1).ToArray();

            string body = buf.Substring(buf.IndexOf(')') + 1, index - (buf.IndexOf(')') + 6));

            source = new StringBuilder(buf.Substring(index));

            return new Tuple<Tuple<string, int>, string>(new Tuple<string, int>(signature, variables.Count()), body);
        }

        private int FindFirstOuterSymbol(StringBuilder source)
        {
            string buf = source.ToString();

            int counter = 1;

            for (int i = 1; i < source.Length; ++i)
            {
                if(buf[i] == '#' && buf[i-1] != '\\')
                {
                    if(buf.Length - i >= 3 && buf.Substring(i, 4) == "#end")
                    {
                        --counter;

                        if(counter == 0)
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

            throw new ArgumentException("Macro definition: #end is unreachable");
        }

        public Template Parse(string toParse)
        {
            StringBuilder builder = new StringBuilder(toParse);

            CompositeMacro macro = new CompositeMacro();
            MacroTable table = new MacroTable();

            while(builder.Length > 0)
            {
                IParser parser = GetParser(builder);

                if(parser != null)
                {
                    macro.Add(parser.Parse(ref builder));
                }
                else
                {
                    Tuple<Tuple<string, int>, string> macroToAdd = GetMacroDefinition(ref builder);

                    table.AddMacros(macroToAdd.Item1.Item1, macroToAdd.Item1.Item2, macroToAdd.Item2);
                }
            }

            return new Template(macro, table);
        }
    }
}
