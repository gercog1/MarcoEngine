using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEngine
{
    class Template
    {
        private IMacroElement aggregate;
        private MacroTable macroTable;
        private Context context;

        public Template(IMacroElement el = null, MacroTable table = null, Context c = null)
        {
            aggregate = el;
            macroTable = table;
            context = c;
        }

        public string Execute(Context c = null, MacroTable table = null)
        {
            if (c != null)
            {
                context = c;
            }

            if (table != null)
            {
                macroTable = table;
            }

            return aggregate.Execute(context,macroTable);
        }

    }
}
