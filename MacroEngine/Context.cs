using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MacroEngine
{
    public class Context : ICloneable
    {
        private Dictionary<string, object> parameters;

        public Context()
        {
            parameters = new Dictionary<string, object>();
        }

        public object GetValue(string key)
        {
            return parameters.ContainsKey(key) ? parameters[key] : null;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public void SetValue(string key, object val)
        {
            parameters[key] = val;
        }

        public static Context GetXMLContext(string filePath)
        {
            var xDoc = XDocument.Load(new StreamReader(filePath));

            dynamic root = new ExpandoObject();

            XmlToDynamic.Parse(root, xDoc.Elements().First());

            IDictionary<string, object> propertyValues = (IDictionary<string, object>)root.Data;

            Context toReturn = new Context();

            foreach (var kvp in propertyValues)
            {
                toReturn.SetValue(kvp.Key, kvp.Value);
            }

            return toReturn;
        }
    }
}
