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
        public static Value Parse(string script_code, Dictionary<string, Value> args)
        {
            string[] string_expressions = script_code.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries); //split to lines
            for (int i = 0; i < string_expressions.Length; i++)
            {
                string string_expression = string_expressions[i];
                string[] tokens = string_expression.Split(new char[4] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                switch (tokens[0])
                {
                    case "push-local":
                        args.Add(tokens[1], Value.GetValue(tokens[2]));
                        break;
                    case "return":
                        return Value.GetValue(tokens[1]);
                    case "block":
                        string body = "";
                        for (int j = i; j < string_expressions.Length; j++)
                        {
                            i = j;
                            if (string_expressions[j].Trim() == "end")
                                break;
                            body += string_expressions[j];
                        }
                        VirtualMachine.mainstack.values.Add(tokens[1], new Value(new wolBlock(body)));
                        break;
                    default:
                        ParseExpression(string_expression, args);
                        break;
                }
            }
            return Value.VoidValue;
        }

        public static Value Parse(string code) => Parse(code, new Dictionary<string, Value>());

        public static Value ParseExpression(string expr) => ParseExpression(expr, new Dictionary<string, Value>());

        public static Value ParseExpression(string string_expression, Dictionary<string, Value> arguments)
        {
            string[] tokens = string_expression.Split(new char[4] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            string keyword = tokens[0];
            if (keyword.StartsWith("@") || keyword.StartsWith("#") || keyword.StartsWith("$") 
                || keyword.StartsWith("%") || keyword.StartsWith("<") || keyword.StartsWith("&"))
            {
                wolFunc value = (wolFunc) Value.GetValue(keyword).type;
                string args = string_expression.Substring(string_expression.IndexOf(':') + 1).Trim(); //code after name of function (string with arguments)
                string[] argums = args.Split(','); //array with arguments of expression
                Value[] values = new Value[argums.Length]; //array with arguments who converted to Value
                for (int i = 0; i < argums.Length; i++)
                    values[i] = Value.GetValue(argums[i].TrimStart()); //convert string arguments to Value arguments
                return value.Call(values);
            } 
            else
            {
                bool haveExpression = true; //check on found expression by this name
                foreach (KeyValuePair<string, VMExpression> expression in VirtualMachine.expressions)
                {
                    if (expression.Key == keyword)
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
                            values[i] = Value.GetValue(argums[i].TrimStart()); //convert string arguments to Value arguments
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
                    VirtualMachine.ThrowVMException($"VM Expression by name {keyword} not found and will cannot parse", VirtualMachine.position, ExceptionType.NotFoundException);
                }
                return null;
            }
        }
    }
}
