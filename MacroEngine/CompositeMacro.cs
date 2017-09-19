using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEngine
{
    class CompositeMacro : IMacroElement
    {
        private List<IMacroElement> children;

        public CompositeMacro()
        {
            children = new List<IMacroElement>();
        }

        public void Add(IMacroElement el)
        {
            children.Add(el);
        }

        public void Remove(IMacroElement el)
        {
            children.RemoveAll(x => x.Equals(el));
        }

        public string Execute(Context c, MacroTable table)
        {
            StringBuilder toReturn = new StringBuilder();

            foreach(IMacroElement el in children)
            {
                toReturn.Append(el.Execute(c, table));
            }

            return toReturn.ToString();
        }
    }
}
