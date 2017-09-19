using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEngine
{
    class MacroCall : IMacroElement
    {
        private string name;

        private string[] variables;

        public MacroCall(string _name = null, string[] _variables = null)
        {
            name = _name;
            variables = _variables;
        }

        public string Execute(Context c, MacroTable table)
        {
            KeyValuePair<Tuple<string,int>, string> data = table.GetMacros(name, variables.Count());

            if(data.Key.Item2 != -1)
            {
                List<string> parameters = data.Key.Item1.Split(' ').ToList();
                parameters.RemoveAll(x => x == "");
                parameters = parameters.Skip(1).ToList();

                for(int i = 0; i < parameters.Count; ++i)
                {
                    parameters[i] = parameters[i].Substring(1);
                }

                Context newContext = (Context)c.Clone();

                for(int i = 0; i < parameters.Count; ++i)
                {
                    if(variables[i].Length > 1 && variables[i][0] == '$')
                    {
                        string toPass = c.GetValue(variables[i].Substring(1)).ToString();
                        newContext.SetValue(parameters[i], toPass);
                    }
                    else
                    {
                        newContext.SetValue(parameters[i], variables[i]);
                    }
                }

                ParseManager manager = new ParseManager();

                Template innerMacros = manager.Parse(data.Value);

                string toReturn = innerMacros.Execute(newContext, table);

                return toReturn;
            }
            else
            {
                throw new ArgumentException(String.Format("Macro table does not contain definition for method {0} with {1} parameters", name, variables.Count()));
            }
        }
    }
}
