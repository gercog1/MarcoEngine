using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEngine
{
    class PlainTextMacro : IMacroElement
    {
        private string text;

        public PlainTextMacro(string str)
        {
            text = str;
        }
        public string Execute(Context c, MacroTable table)
        {
            string toReturn = text.Replace(@"\.", ".");
            toReturn = toReturn.Replace(@"\#", "#");
            toReturn = toReturn.Replace(@"\$", "$");
            toReturn = toReturn.Replace("\\\"", "\"");
            return toReturn;
        }
    }
}
