using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

public class User
{
    public bool IsVerified { get; set; }
}

namespace MacroEngine
{
    class Program
    {
        // read template from specified directory
        public static string GetTemplate(string filePath)
        {
            StreamReader rdr = new StreamReader(filePath);

            string buf = "";

            StringBuilder builder = new StringBuilder();

            while ((buf = rdr.ReadLine()) != null)
            {
                builder.Append(buf);
                builder.Append("\r\n");
            }

            return builder.ToString();
        }

        // save ready template to specified directory
        public static void SaveFile(string filePath, string toSave)
        {
            StreamWriter writer = new StreamWriter(filePath);

            Console.WriteLine("Writing to directory: {0}", filePath);

            writer.WriteLine(toSave);

            writer.Close();
        }
    

        static void Main(string[] args)
        {
            string template = GetTemplate(args[1]);
            Context c = Context.GetXMLContext(args[0]);
       
            ParseManager manager = new ParseManager();
            Template t = manager.Parse(template);
            string toWrite = t.Execute(c);

            SaveFile(args[2], toWrite);

            Console.WriteLine("Operation complete");
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
