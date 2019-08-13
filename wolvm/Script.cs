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
        public static Value Parse(string script_code, Stack _stack)
        {
            string[] string_expressions = script_code.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries); //split to lines
            foreach (string string_expression in string_expressions)
            {
                string[] tokens = string_expression.Split(new char[4] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens[0].StartsWith("@"))
                {
                    if (tokens[0].Contains("#"))
                    {
                        if (tokens[0].Split('#').Length == 2)
                        {
                            string valname = tokens[0].Remove(0, 1).Split('#')[0]; //get var name (remove '@') and remove '#'
                            Value value = null;
                            try
                            {
                                value = _stack.values[valname]; //create get value from valname
                            }
                            catch (KeyNotFoundException)
                            {
                                VirtualMachine.ThrowVMException($"Variable by name {valname} not found", VirtualMachine.position - script_code.Length, ExceptionType.NotFoundException);
                                break; //time break for willn`t throw NullRefrenceException 
                            }
                            string meth_name = valname.Split('#')[1]; //get calls method name
                            if (meth_name == "set")
                            {
                                Console.WriteLine("set " + valname);
                                if (value.setter.security == SecurityModifer.PRIVATE)
                                {
                                    VirtualMachine.ThrowVMException("Setter is private", VirtualMachine.position, ExceptionType.SecurityException);
                                    break;
                                }
                                string[] argums = new string[0];
                                string args = string_expression.Substring(string_expression.IndexOf(':') + 1).Trim(); //code after name of setter (string with arguments)
                                argums = args.Split(','); //array with arguments of setter
                                Value[] values = new Value[argums.Length]; //array with arguments who converted to Value
                                for (int i = 0; i < argums.Length; i++)
                                {
                                    values[i] = Value.GetValue(argums[i]);
                                    //convert string arguments to Value arguments
                                }
                                value.setter.Call(values); //call setter with arguments
                            }
                            else if (meth_name == "get")
                            {
                                if (value.getter.security == SecurityModifer.PRIVATE)
                                {
                                    VirtualMachine.ThrowVMException("Getter is private", VirtualMachine.position, ExceptionType.SecurityException);
                                    break;
                                }
                                value.getter.Call(); //just call getter
                            }
                            else
                            {
                                wolFunction func = value.GetMethod(meth_name);
                                if (func.security == SecurityModifer.PRIVATE)
                                {
                                    VirtualMachine.ThrowVMException($"Method by name {meth_name} is private", VirtualMachine.position, ExceptionType.SecurityException);
                                    break;
                                }
                                string[] argums = new string[0];
                                if (string_expression.Contains(":"))
                                {
                                    string args = string_expression.Substring(string_expression.IndexOf(':') + 1).Trim(); //code after name of method (string with arguments)
                                    argums = args.Split(','); //array with arguments of method
                                }
                                else
                                {
                                    func.Call(value); //call method with 'this' argument and exit from parsing
                                    break;
                                }
                                Value[] values = new Value[argums.Length + 1]; //array with arguments who converted to Value
                                values[0] = value; //'this' parameter
                                for (int i = 1; i < argums.Length + 1; i++)
                                {
                                    values[i] = Value.GetValue(argums[i]);
                                    //convert string arguments to Value arguments
                                }
                                func.Call(values); //call method with arguments
                            }
                        }
                        else
                        {
                            VirtualMachine.ThrowVMException("Function can`t has and call daughter functions", VirtualMachine.position, ExceptionType.ChildException);
                        }
                    }
                    else
                    {
                        VirtualMachine.ThrowVMException("Call function in this expression not found", VirtualMachine.position - script_code.Length + 1, ExceptionType.NotFoundException);
                    }
                }
                else if (tokens[0].StartsWith("#"))
                {
                    string func_name = tokens[0].Remove(0, 1); //get function name (remove '#')
                    wolFunction func;
                    try
                    {
                        func = _stack.functions[func_name]; //search in imported stack function by name after '#'
                    }
                    catch (KeyNotFoundException)
                    {
                        VirtualMachine.ThrowVMException($"Function by name {func_name} not found", VirtualMachine.position - script_code.Length, ExceptionType.NotFoundException);
                        continue; //time break
                    }
                    string[] argums = new string[0];
                    if (string_expression.Contains(":"))
                    {
                        string args = string_expression.Substring(string_expression.IndexOf(':') + 1).Trim(); //code after name of function (string with arguments)
                        argums = args.Split(','); //array with arguments of function
                    }
                    else
                    {
                        func.Call(); //call function and exit from parsing
                        break;
                    }
                    Value[] values = new Value[argums.Length]; //array with arguments who converted to Value
                    for (int i = 0; i < argums.Length; i++)
                    {
                        values[i] = Value.GetValue(argums[i]);
                        //convert string arguments to Value arguments
                    }
                    func.Call(values); //call function with arguments
                }
                else if (tokens[0].StartsWith("^"))
                {
                    //this is constructor
                }
                else if (tokens[0].StartsWith("$"))
                {
                    //if call static method
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
                else if (tokens[0] == "label")
                {
                    //pass
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
                                expression.Value.ParseExpression();
                                break;
                            }
                            Value[] values = new Value[argums.Length]; //array with arguments who converted to Value
                            for (int i = 0; i < argums.Length; i++)
                            {
                                string normstr = argums[i].TrimStart();
                                if (normstr.StartsWith("goto"))
                                    VirtualMachine.Goto(normstr.Remove(0, 4));
                                else
                                    values[i] = Value.GetValue(normstr);
                                //convert string arguments to Value arguments
                            }
                            haveExpression = true;
                            expression.Value.ParseExpression(values);
                            break;
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
                }
            }
            return Value.VoidValue;
        }
    }
}
