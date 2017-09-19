using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEngine
{
    interface IMacroElement
    {
        string Execute(Context c, MacroTable table);
    }
}
