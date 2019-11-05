using System;
using System.Collections.Generic;
using System.Linq;
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
                string string_expression = string_expressions[i].Trim();
                if (string_expression == string.Empty)
                {
                    if (i == string_expressions.Length - 1) break;
                    else continue;
                } 
                string[] tokens = string_expression.Split(new char[4] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                switch (tokens[0])
                {
                    case "push-local":
                        args.Add(tokens[1], Value.GetValue( string.Join(' ', tokens.TakeLast(tokens.Length - 2)) ));
                        break;
                    case "delete":
                        if (tokens[1] == "local")
                        {
                            args.Remove(tokens[2]);
                        }
                        else if (tokens[1] == "class")
                        {
                            VirtualMachine.mainstack.classes.Remove(tokens[2]);
                        }
                        else if (tokens[1] == "func")
                        {
                            VirtualMachine.mainstack.functions.Remove(tokens[2]);
                        }
                        else
                        {
                            VirtualMachine.mainstack.values.Remove(tokens[1]);
                        }
                        break;
                    case "while":
                        while (((wolBool)Value.GetValue(string.Join(' ', tokens.TakeLast(tokens.Length - 2))).type).value)
                        {
                            ((wolBlock)VirtualMachine.FindBlock(tokens[1]).type).Run();
                        }
                        break;
                    case "return":
                        return Value.GetValue(tokens[1]);
                    case "block":
                        string body = "";
                        for (int j = ++i; j < string_expressions.Length; j++)
                        {
                            i = j;
                            if (string_expressions[j].Trim() == "end")
                                break;
                            body += string_expressions[j] + ';';
                        }
                        VirtualMachine.mainstack.values.Add(tokens[1], new Value(new wolBlock(body)));
                        if (VirtualMachine.test) Console.WriteLine("Body of " + tokens[1] + '\n' + body);
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
            //Console.WriteLine("String expression: " + string_expression);
            VirtualMachine.mainstack = VirtualMachine.mainstack + arguments;
            string[] tokens = string_expression.Split(new char[4] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            string keyword = tokens[0];
            if (keyword.StartsWith("@") || keyword.StartsWith("#") || keyword.StartsWith("$")
                || keyword.StartsWith("%") || keyword.StartsWith("<") || keyword.StartsWith("&"))
            {
                wolFunc value = new wolFunc(); //create empty 'Func' instance for not throwing NullRefrenceException
                try
                {
                    value = (wolFunc)Value.GetValue(keyword).type;
                }
                catch (InvalidCastException)
                {
                    VirtualMachine.ThrowVMException($"'{keyword}' haven`t type Func", VirtualMachine.position, ExceptionType.InvalidTypeException);
                }
                string args = string_expression.Substring(string_expression.IndexOf(':') + 1).Trim(); //code after name of function (string with arguments)
                List<string> argums = new List<string>(); //array with arguments of expression
                int pos = 0;
                char current = args[0];
                StringBuilder buffer = new StringBuilder();
                while (true)
                {
                    if (current == '(')
                    {
                        while (current != ')')
                        {
                            try
                            {
                                buffer.Append(current);
                                current = args[++pos];
                            }
                            catch (IndexOutOfRangeException)
                            {
                                VirtualMachine.ThrowVMException("End of expression not found", VirtualMachine.position - args.Length + pos, ExceptionType.BLDSyntaxException);
                            }
                        }
                    }
                    else if (current == '<')
                    {
                        while (current != '>')
                        {
                            try
                            {
                                buffer.Append(current);
                                current = args[++pos];
                            }
                            catch (IndexOutOfRangeException)
                            {
                                VirtualMachine.ThrowVMException("End of value not found", VirtualMachine.position - args.Length + pos, ExceptionType.BLDSyntaxException);
                            }
                        }
                    }
                    else if (current == ',')
                    {
                        argums.Add(buffer.ToString());
                        buffer.Clear();
                        try
                        {
                            current = args[++pos];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            break;
                        }
                    }
                    else
                    {
                        try
                        {
                            buffer.Append(current);
                            current = args[++pos];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            argums.Add(buffer.ToString());
                            break;
                        }
                    }
                }
                Value[] values = new Value[argums.Count]; //array with arguments who converted to Value
                for (int i = 0; i < argums.Count; i++)
                    values[i] = Value.GetValue(argums[i].TrimStart()); //convert string arguments to Value arguments
                VirtualMachine.mainstack = VirtualMachine.mainstack - arguments;
                return value.Call(values);
            }
            else if (keyword.StartsWith("^"))
            {
                string[] toks = keyword.Remove(0, 1).Split('.');
                string args = string_expression.Substring(string_expression.IndexOf(':') + 1).Trim(); //code after constructor (string with arguments)
                string[] argums = args.Split(','); //array with arguments of constructor
                Value[] values = new Value[argums.Length]; //array with arguments who converted to Value
                for (int i = 0; i < argums.Length; i++)
                    values[i] = Value.GetValue(argums[i].TrimStart()); //convert string arguments to Value arguments
                VirtualMachine.mainstack = VirtualMachine.mainstack - arguments;
                return new Value(VirtualMachine.GetWolClass(toks[0]), toks[1], values);
            }
            else if (keyword.StartsWith("~"))
            {
                string args = string_expression.Substring(string_expression.IndexOf(':') + 1).Trim(); //code after destructor (string with arguments)
                string[] argums = args.Split(','); //array with arguments of destructor
                Value[] values = new Value[argums.Length]; //array with arguments who converted to Value
                for (int i = 0; i < argums.Length; i++)
                    values[i] = Value.GetValue(argums[i].TrimStart()); //convert string arguments to Value arguments
                string[] toks = keyword.Remove(0, 1).Split(':');
                VirtualMachine.GetWolClass(toks[0]).CallDestructor(int.Parse(toks[1]), values);
                VirtualMachine.mainstack = VirtualMachine.mainstack - arguments;
                return Value.VoidValue;
            }
            else
            {
                bool haveExpression = true; //check on found expression by this name
                foreach (KeyValuePair<string, VMExpression> expression in VirtualMachine.expressions)
                {
                    if (expression.Key == keyword)
                    {
                        List<string> argums = new List<string>();
                        if (string_expression.Contains(":"))
                        {
                            string args = string_expression.Substring(string_expression.IndexOf(':') + 1).Trim(); //code after name of expression (string with arguments)
                            StringBuilder buffer = new StringBuilder();
                            byte expr = 0; //priority of expressions
                            for (int i = 0; i < args.Length; i++)
                            {
                                char cur = args[i];
                                if (cur == ',' && expr == 0)
                                {
                                    argums.Add(buffer.ToString());
                                    buffer.Clear();
                                }
                                else if (cur == ')' && expr > 0)
                                {
                                    buffer.Append(cur);
                                    expr--;
                                    //Console.WriteLine("Priority ): " + expr);
                                }
                                else if (cur == '(' && expr >= 0)
                                {
                                    buffer.Append(cur);
                                    expr++;
                                    //Console.WriteLine("Priority (: " + expr);
                                }
                                else
                                {
                                    buffer.Append(cur);
                                }
                            }
                            argums.Add(buffer.ToString());
                            //Console.WriteLine(string.Join(' ', argums) + '\t' + argums.Count);
                        }
                        else
                        {
                            haveExpression = true;
                            return expression.Value.ParseExpression();
                        }
                        Value[] values = new Value[argums.Count]; //array with arguments who converted to Value
                        for (int i = 0; i < argums.Count; i++)
                            values[i] = Value.GetValue(argums[i].TrimStart()); //convert string arguments to Value arguments
                        haveExpression = true;
                        VirtualMachine.mainstack = VirtualMachine.mainstack - arguments;
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
                VirtualMachine.mainstack = VirtualMachine.mainstack - arguments;
                return null;
            }
        }
    }
}
