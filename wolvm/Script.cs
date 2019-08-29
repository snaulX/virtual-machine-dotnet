using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public static class Script
    {
        /// <summary>
        /// Parsing script code with imported stack
        /// </summary>
        /// <param name="script_code">Code who will parsing to script</param>
        /// <param name="_stack">Stack who will import to script</param>
        public static Value Parse(string script_code)
        {
            string[] string_expressions = script_code.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries); //split to lines
            foreach (string string_expression in string_expressions)
            {
                ParseExpression(string_expression);
            }
            return Value.VoidValue;
        }

        public static Value ParseExpression(string string_expression)
        {
            string[] tokens = string_expression.Split(new char[4] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens[0].StartsWith("@"))
            {
                return Value.GetValue(tokens[0]); //it`s pass
            }
            else if (tokens[0] == "return")
            {
                try
                {
                    return Value.GetValue(tokens[1]);
                }
                catch (IndexOutOfRangeException)
                {
                    return Value.VoidValue;
                }
            }
            else if (tokens[0] == "block")
            {
                return new Value(VirtualMachine.wolBlock);
            }
            else
            {
                bool haveExpression = true; //check on found expression by this name
                foreach (KeyValuePair<string, VMExpression> expression in VirtualMachine.expressions)
                {
                    if (expression.Key == tokens[0])
                    {
                        string[] argums = new string[0];
                        if (string_expression.Contains(":"))
                        {
                            string args = string_expression.Substring(string_expression.IndexOf(':') + 1).Trim(); //code after name of expression (string with arguments)
                            argums = args.Split(','); //array with arguments of expression
                        }
                        else
                        {
                            haveExpression = true;
                            return expression.Value.ParseExpression();
                        }
                        Value[] values = new Value[argums.Length]; //array with arguments who converted to Value
                        for (int i = 0; i < argums.Length; i++)
                        {
                            string normstr = argums[i].TrimStart();
                            values[i] = Value.GetValue(normstr);
                            //convert string arguments to Value arguments
                        }
                        haveExpression = true;
                        return expression.Value.ParseExpression(values);
                    }
                    else
                    {
                        haveExpression = false;
                    }
                }
                if (!haveExpression)
                {
                    VirtualMachine.ThrowVMException($"VM Expression by name {tokens[0]} not found and will cannot parse", VirtualMachine.position, ExceptionType.NotFoundException);
                }
                return null;
            }
        }
    }
}
