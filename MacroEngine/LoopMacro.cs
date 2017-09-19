using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEngine
{
    class LoopMacro : IMacroElement
    {
        public string itemName;
        public string containerName;
        public string loopBody;


        public LoopMacro(string _item = null, string _container = null, string _loop = null)
        {
            itemName = _item;
            containerName = _container;
            loopBody = _loop;
        }



        public string Execute(Context c, MacroTable table)
        {
            object containerData = c.GetValue(containerName.Substring(1, containerName.Length - 1));

            if(containerData != null)
            {
                IEnumerable<object> objects = containerData as IEnumerable<object>;

                if(objects != null)
                {
                    ParseManager manager = new ParseManager();

                    Template t = manager.Parse(loopBody);

                    string toReturn = "";

                    foreach(object obj in objects)
                    {
                        Context toPass = (Context)c.Clone();

                        toPass.SetValue(itemName.Substring(1, itemName.Length - 1), obj);

                        toReturn += t.Execute(toPass, table);
                    }

                    return toReturn;
                }
                else
                {
                    throw new ArgumentException("Wrong data type for container");
                }
            }
            else
            {
                throw new ArgumentException("Container not found");
            }
        }
    }
}
