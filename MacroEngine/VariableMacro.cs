using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MacroEngine
{
    class VariableMacro : IMacroElement
    {
        private string name;
        private List<string> properties;

        public VariableMacro(string _name=null, List<string> _properties = null)
        {
            name = _name;
            properties = _properties;
        }

        private object GetPropertyData(object lastObj, string propertyName)
        {
            object toReturn = null;

            var data = (IDictionary<string, object>)lastObj;

            if (data.ContainsKey(propertyName))
            {
                toReturn = data[propertyName];
            }
            else
            {
                throw new ArgumentException(String.Format("Variable macro: wrong property name {0}", propertyName));
            }

            return toReturn;
        }

        public string Execute(Context c, MacroTable table)
        {
            object parameter = c.GetValue(name);

            if(parameter != null)
            {
                
               if(properties != null)
                {
                    for(int i = 0; i < properties.Count; ++i)
                    {
                        parameter = GetPropertyData(parameter, properties[i]);
                    }
                }

                return parameter.ToString();      
            }
            else
            {
                throw new ArgumentException("Variable macro: context does not contain proper variable");
            }
        }
    }
}
