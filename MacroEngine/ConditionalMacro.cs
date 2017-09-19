using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEngine
{
    class ConditionalMacro : IMacroElement
    {
        List<Tuple<string, string>> conditionSignatures;
        List<string> conditionBodies;

        public ConditionalMacro(List<Tuple<string, string>> _sign = null, List<string> _bodies = null)
        {
            conditionSignatures = (_sign != null ? _sign : new List<Tuple<string, string>>());
            conditionBodies = (_bodies != null ? _bodies : new List<string>());
        }
        public string Execute(Context c, MacroTable table)
        {
            ParseManager manager = new ParseManager();

            string toReturn = "";

            if(conditionSignatures[conditionSignatures.Count - 1].Item1 == "else")
            {
                bool executed = false;

                for(int i = 0; i < conditionSignatures.Count - 1; ++i)
                {
                    string toCheck = conditionSignatures[i].Item2;

                    if (LogicManager.IsTrue(toCheck, c))
                    {
                        toReturn = manager.Parse(conditionBodies[i]).Execute(c);
                        executed = true;
                        break;
                    }
                }

                if (!executed)
                {
                    toReturn = manager.Parse(conditionBodies[conditionBodies.Count - 1]).Execute(c);
                }
            }
            else
            {
                for (int i = 0; i < conditionSignatures.Count; ++i)
                {
                    string toCheck = manager.Parse(conditionSignatures[i].Item2).Execute(c);

                    if (LogicManager.IsTrue(toCheck,c))
                    {
                        toReturn = manager.Parse(conditionBodies[i]).Execute(c, table);
                        break;
                    }
                }
            }

            return toReturn;
        }
    }
}
