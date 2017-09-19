using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroEngine
{
    static class LogicManager
    {
        public static Dictionary<string,string> GetStringReplacers(ref string toEval, int start_index, int end_index)
        {
            Dictionary<string, string> toReturn = new Dictionary<string, string>();

            int replacementsCount = 0;

            int openingQuoteIndex = 0;
            int openQuotesCount = 0;

            StringBuilder buffer = new StringBuilder();

            while(start_index < end_index)
            {
                if (toEval[start_index] == '"')
                {
                    if (openQuotesCount == 0)
                    {
                        ++openQuotesCount;
                        openingQuoteIndex = start_index;
                    }
                    else
                    {

                        --openQuotesCount;

                        toReturn.Add("{" + replacementsCount.ToString() + "}", buffer.ToString());
                        toEval = toEval.Remove(openingQuoteIndex, buffer.Length + 2);
                        toEval = toEval.Insert(openingQuoteIndex, " {" + replacementsCount.ToString() + "} ");

                        int shift = buffer.Length - (replacementsCount.ToString().Length + 2);

                        start_index -= shift;
                        end_index -= shift;

                        ++replacementsCount;
                        buffer.Clear();
                    }
                }
                else
                {
                    if (openQuotesCount > 0)
                    {
                        buffer.Append(toEval[start_index]);
                    }
                }

                ++start_index;
            }

            if(openQuotesCount > 0)
            {
                throw new ArgumentException("Logic expression: wrong number of quotes");
            }

            return toReturn;
        }
        private static string GetNextTerm(ref StringBuilder builder)
        {
            switch (builder[0])
            {
                case '!':
                    if (builder.Length > 1 && builder[1] == '=')
                    {
                        builder.Remove(0, 2);
                        return "!=";
                    }
                    else
                    {
                        builder.Remove(0, 1);
                        return "!";
                    }
                case '(':
                    builder.Remove(0, 1);
                    return "(";
                case ')':
                    builder.Remove(0, 1);
                    return ")";
                case '=':
                    builder.Remove(0, 2);
                    return "==";
                case '&':
                    builder.Remove(0, 2);
                    return "&&";
                case '|':
                    builder.Remove(0, 2);
                    return "||";
                case '<':
                    builder.Remove(0, 2);
                    return "<=";
                case '>':
                    builder.Remove(0, 2);
                    return ">=";
                case '{':
                    string buf1 = builder.ToString();
                    int index1 = buf1.IndexOf('}');
                    builder.Remove(0, index1 + 1);
                    return buf1.Substring(0, index1 + 1);
                default:
                    string buf2 = builder.ToString();
                    int index2 = buf2.IndexOfAny(new char[] { '!', '(', ')', '=', '&', '|', '{', '<', '>' });
                    builder.Remove(0, index2);
                    return buf2.Substring(0, index2);
            }
        }
        private static List<string> GetConditionTermsForInfix(string buf)
        {
            StringBuilder joinedStr = new StringBuilder(buf);

            List<string> toReturn = new List<string>();

            while(joinedStr.Length != 0)
            {
                toReturn.Add(GetNextTerm(ref joinedStr));
            }

            return toReturn;
        }     
        public static bool IsTrue(string infix, Context c)
        {
            Dictionary<string, string> replacers = GetStringReplacers(ref infix, 0, infix.Length);
            string postfix = ConvertInfixToPostfix(infix);

            List<string> terms = postfix.Split(' ').ToList();

            terms.RemoveAll(x => x == "");

            return Convert.ToBoolean(ComputePostfix(terms, replacers, c));
        }
        private static string ConvertInfixToPostfix(string infix)
        {
            List<string> buf = infix.Split(' ').ToList();
            buf.RemoveAll(x => x == "");

            List<string> parameters = GetConditionTermsForInfix(buf.Aggregate((a,b) => a + b));

            Stack<string> stack = new Stack<string>();
            StringBuilder postfix = new StringBuilder();

            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i] == "(")
                {
                    stack.Push(parameters[i]);
                }
                else if ((parameters[i] == "!") ||
                         (parameters[i] == "==") ||
                         (parameters[i] == "!=") ||
                         (parameters[i] == "&&") ||
                         (parameters[i] == "||") ||
                         (parameters[i] == ">=") ||
                         (parameters[i] == "<="))
                {
                    while ((stack.Count > 0) && (stack.Peek() != "("))
                    {
                        if (GetPriority(stack.Peek()) > GetPriority(parameters[i]))
                        {
                            postfix.Append(stack.Pop());
                            postfix.Append(" ");
                        }
                        else
                        {
                            break;
                        }
                    }
                    stack.Push(parameters[i]);
                }
                else if (parameters[i] == ")")
                {
                    while ((stack.Count > 0) && (stack.Peek() != "("))
                    {
                        postfix.Append(stack.Pop());
                        postfix.Append(" ");
                    }
                    if (stack.Count > 0)
                        stack.Pop();
                }
                else
                {
                    postfix.Append(parameters[i]);
                    postfix.Append(" ");
                }
            }
            while (stack.Count > 0)
            {
                postfix.Append(stack.Pop());
                postfix.Append(" ");
            }
            return postfix.ToString();
        }
        private static int GetPriority(string s)
        {
            switch (s)
            {
                case "&&":
                case "||":
                    return 1;
                case "!":
                    return 2;
                case "==":
                case "!=":
                case ">=":
                case "<=":
                    return 3;
                default: return -1;
            }
        }
        private static bool ComputePostfix(List<string> terms, Dictionary<string, string> replacements, Context c)
        {
            string toAdd = "";

            ParseManager manager = new ParseManager();

            for (int i = 0; i < terms.Count; ++i)
            {
                terms[i] = manager.Parse(terms[i]).Execute(c);
            }

            foreach (var kvp in replacements)
            {
                for (int i = 0; i < terms.Count; ++i)
                {
                    if(terms[i] == kvp.Key)
                    {
                        terms[i] = kvp.Value;
                    }
                }
            }

            while (terms.Count() != 1)
            {
                bool signFound = false;

                for (int i = 0; i < terms.Count(); ++i)
                {
                    switch (terms[i])
                    {
                        case "!=":
                        case "==":
                        case "&&":
                        case "||":
                        case ">=":
                        case "<=":
                            toAdd = Compute(terms[i - 2], terms[i - 1], terms[i]);
                            terms.Insert(i - 2, toAdd);
                            terms.RemoveAt(i - 1);
                            terms.RemoveAt(i - 1);
                            terms.RemoveAt(i - 1);
                            signFound = true;
                            break;
                        case "!":
                            toAdd = Compute(terms[i - 1], "", "!");
                            terms.Insert(i - 1, toAdd);
                            terms.RemoveAt(i);
                            terms.RemoveAt(i);
                            signFound = true;
                            break;
                    }

                    if (signFound)
                    {
                        signFound = false;
                        break;
                    }
                }
            }

            return bool.Parse(terms[0]);
        }
        private static string Compute(string operandOne, string operandTwo, string operation)
        {
            string toReturn = "";
            bool operand1 = false;
            bool operand2 = false;

            double number1 = 0;
            double number2 = 0;

            switch (operation)
            {
                case "!":
                    operand1 = bool.Parse(operandOne);

                    toReturn = !operand1 ? "true" : "false";
                    break;
                case "==":
                    toReturn = (operandOne == operandTwo ? "true" : "false");
                    break;
                case "!=":
                    toReturn = (operandOne != operandTwo ? "true" : "false");
                    break;
                case "&&":
                    operand1 = bool.Parse(operandOne);
                    operand2 = bool.Parse(operandTwo);

                    toReturn = (operand1 && operand2 ? "true" : "false");
                    break;
                case "||":
                    operand1 = bool.Parse(operandOne);
                    operand2 = bool.Parse(operandTwo);

                    toReturn = (operand1 || operand2 ? "true" : "false");
                    break;
                case "<=":
                    if (double.TryParse(operandOne, out number1) && double.TryParse(operandTwo, out number2))
                    {
                        toReturn = (number1 <= number2 ? "true" : "false");
                    }
                    else
                    {
                        throw new ArgumentException("<= operator: unable to convert operand to double");
                    }
                    break;
                case ">=":
                    if(double.TryParse(operandOne, out number1) && double.TryParse(operandTwo, out number2))
                    {
                        toReturn = (number1 >= number2 ? "true" : "false");
                    }
                    else
                    {
                        throw new ArgumentException(">= operator: unable to convert operand to double");
                    }
                    break;
            }

            return toReturn;
        }
    }
}
