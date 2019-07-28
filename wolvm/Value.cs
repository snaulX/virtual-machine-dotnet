using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class Value
    {
        public wolClass type;
        public string value;
        public wolFunction getter, setter;
        
        public Value(wolClass wolclass, string val = "<null:void>", SecurityModifer modifer = SecurityModifer.PRIVATE)
        {
            value = val;
            type = wolclass;
            getter = new wolFunction(modifer);
            setter = new wolFunction(modifer);
            setter.body = "Set : &this ;";
            getter.body = "Return : @this ;";
        }

        public Value(wolClass wolclass, string constr_name, params Value[] arguments) : this(wolclass, "<null:void", SecurityModifer.PUBLIC)
        {
            Script.Parse(type.constructors[constr_name].body, VirtualMachine.mainstack + new Stack
            {
                values = new Dictionary<string, Value>
            {
                { "this", this }
            }
            });
        }

        public Value GetField(string name)
        {
            try
            {
                return type.fields[name];
            }
            catch (KeyNotFoundException)
            {
                VirtualMachine.ThrowVMException($"Field by name {name} not found", VirtualMachine.position, ExceptionType.NotFoundException);
                return null;
            }
        }

        public bool CheckType(string name) => VirtualMachine.GetWolClass(name) == type ? true : false; //thanks C# for one-string functions))

        public static bool IsVoid(string val) => val.Trim() == "null";

        public static bool IsBool(string val) => val.Trim() == "true" || val.Trim() == "false";

        public static bool IsDouble(string val) => double.TryParse(val, out double gyg);

        public static bool IsInt(string val) => long.TryParse(val, out long gyg);

        public static bool IsDouble(Value val, out double gyg) => double.TryParse(val.value, out gyg);

        public static Value GetValue(string val)
        {
            val = val.Trim();
            if (val.StartsWith("<") && val.EndsWith(">"))
            {
                val = val.Remove(0, 1).Remove(val.Length - 2); //remove '<' and '>'
                string[] vals = val.Split(':');
                if (vals.Length == 2)
                {
                    Value value = new Value(VirtualMachine.GetWolClass(vals[1]))
                    {
                        value = vals[0]
                    };
                    return value;
                }
                else
                {
                    VirtualMachine.ThrowVMException("Value not found in this string", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                    return null;
                }
            }
            else if (val.StartsWith("@"))
            {
                val = val.Remove(0, 1); //remove '@'
                Console.WriteLine(val.IndexOf('.')); //test
                return null;
            }
            else if (val.StartsWith("&"))
            {
                val = val.Remove(0, 1); //remove '&'
                Value value = new Value(VirtualMachine.wolLink.Value);
                return value;
            }
            else if (val.StartsWith("#"))
            {
                val = val.Remove(0, 1); //remove '#'
                Value value = new Value(VirtualMachine.wolFunc.Value);
                return value;
            }
            else if (val.StartsWith("$"))
            {
                val = val.Remove(0, 1); //remove '$'
                Value value = new Value(VirtualMachine.wolType.Value);
                return null;
            }
            else if (val.StartsWith("("))
            {
                StringBuilder buffer = new StringBuilder();
                char current = val[1];
                int pos = 1;
                while (current != ')')
                {
                    try
                    {
                        buffer.Append(current);
                        current = val[++pos];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        VirtualMachine.ThrowVMException("End of string expression not found", VirtualMachine.position - val.Length + pos, ExceptionType.BLDSyntaxException);
                    }
                }
                string string_expression = buffer.ToString();
                string[] tokens = string_expression.Split(new char[4] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (VMExpression expression in VirtualMachine.expressions)
                {
                    if (expression.GetType().Name.Remove(expression.GetType().Name.Length - 10) == tokens[0])
                    {
                        string args = string_expression.Substring(tokens[0].Length); //code after name of expression (string with arguments)
                        string[] argums = string_expression.Split(','); //array with arguments of expression
                        if (argums.Length == 1)
                        {
                            return expression.ParseExpression();
                        }
                        Value[] values = new Value[argums.Length]; //array with arguments who converted to Value
                        for (int i = 0; i < argums.Length; i++)
                        {
                            values[i] = GetValue(argums[i]);
                            //convert string arguments to Value arguments
                        }
                        return expression.ParseExpression(values);
                    }
                }
                VirtualMachine.ThrowVMException("VM Expression by name " + tokens[0] + " not found and will cannot parse", VirtualMachine.position, ExceptionType.NotFoundException);
                return null;
            }
            else
            {
                VirtualMachine.ThrowVMException("Value cannot find", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                return null;
            }
        }

        public static Value VoidValue => new Value(VirtualMachine.Void);

        public override string ToString() => "VALUE:" + value + "\nTYPE:" + type.ToString();

        /// <summary>
        /// Get not static method in this value
        /// </summary>
        /// <param name="name">Name of not static method</param>
        /// <returns></returns>
        public wolFunction GetMethod(string name)
        {
            wolFunction meth = type.methods[name];
            if (meth.arguments.ContainsKey("this")) return meth;
            else return null;
        }
    }
}
