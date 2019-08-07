using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class Value
    {
        public wolClass type;
        public wolFunction getter, setter;

        public Value(wolClass wolclass, SecurityModifer modifer = SecurityModifer.PRIVATE, bool isConstant = false)
        {
            type = wolclass;
            getter = new wolFunction(modifer);
            if (!isConstant)
            {
                setter = new wolFunction(modifer);
                setter.body = "set : &this ;";
            }
            getter.body = "return @this ;";
        }

        public Value(wolClass wolclass, string constr_name, params Value[] arguments) : this(wolclass, SecurityModifer.PUBLIC)
        {
            if (wolclass.classType == wolClassType.STRUCT)
            {
                //pass
            }
            else if (wolclass.classType == wolClassType.ENUM)
            {
                //pass
            }
            else
            {
                if (wolclass is wolDouble)
                {
                    //pass
                }
                else if (wolclass is wolString)
                {
                    //pass
                }
                else
                {
                    try
                    {
                        Script.Parse(type.constructors[constr_name].body, VirtualMachine.mainstack + new Dictionary<string, Value>
                            {
                                { "this", this }
                            }
                        );
                    }
                    catch (KeyNotFoundException)
                    {
                        VirtualMachine.ThrowVMException($"Constructor by name {constr_name} not found in {wolclass}", VirtualMachine.position, ExceptionType.NotFoundException);
                    }
                }
            }
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

        public static Value GetValue(string val)
        {
            val = val.Trim();
            if (val.StartsWith("<") && val.EndsWith(">")) //example of syntax - Loads : <System:string> ;
            {
                val = val.Remove(0, 1).Remove(val.Length - 2); //remove '<' and '>'
                string[] vals = val.Split(':');
                if (vals.Length == 2)
                {
                    Value value;
                    if (vals[1] == "double")
                    {
                        wolDouble type = (wolDouble)VirtualMachine.wolDouble;
                        type.ParseDouble(vals[0]);
                        value = new Value(type);
                    }
                    else if (vals[1] == "int")
                    {
                        wolInt type = (wolInt)VirtualMachine.wolInt;
                        type.ParseInt(vals[0]);
                        value = new Value(type);
                    }
                    else if (vals[1] == "string")
                    {
                        wolString type = (wolString)VirtualMachine.wolString;
                        type.value = vals[0];
                        value = new Value(type);
                    }
                    else
                    {
                        value = new Value(VirtualMachine.GetWolClass(vals[1]));
                    }
                    return value;
                }
                else
                {
                    VirtualMachine.ThrowVMException("Value and his type not found in this string", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                    return null;
                }
            }
            else if (val.StartsWith("@")) //example of syntax - plus : @a, @b ;
            {
                val = val.Remove(0, 1); //remove '@'
                Console.WriteLine(val.IndexOf('.')); //test
                return null;
            }
            else if (val.StartsWith("&")) //example of syntax - set : &this, <null:void> ;
            {
                val = val.Remove(0, 1); //remove '&'
                string[] tokens = val.Split('.', '#');
                wolLink type = (wolLink)VirtualMachine.wolLink;
                type.Address = tokens[0];
                Value value = type.GetValue();
                return value;
            }
            else if (val.StartsWith("#")) //example of syntax - set : &this, #sum ;
            {
                val = val.Remove(0, 1); //remove '#'
                Value value = new Value(VirtualMachine.wolFunc);
                return value;
            }
            else if (val.StartsWith("$")) //example of syntax - equals : $void, (typeof : <null:void>) ;
            {
                val = val.Remove(0, 1); //remove '$'
                Value value = new Value(VirtualMachine.wolType);
                return value;
            }
            else if (val.StartsWith("(")) //example of syntax - return (typeof : @this ) ;
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
                foreach (KeyValuePair<string, VMExpression> expression in VirtualMachine.expressions)
                {
                    string[] argums = new string[0];
                    if (string_expression.Contains(":"))
                    {
                        string args = string_expression.Substring(string_expression.IndexOf(':') + 1).Trim(); //code after name of expression (string with arguments)
                        argums = args.Split(','); //array with arguments of expression
                    }
                    else
                    {
                        return expression.Value.ParseExpression();
                    }
                    Value[] values = new Value[argums.Length]; //array with arguments who converted to Value
                    for (int i = 0; i < argums.Length; i++)
                    {
                        values[i] = GetValue(argums[i]);
                        //convert string arguments to Value arguments
                    }
                    return expression.Value.ParseExpression(values);
                }
                VirtualMachine.ThrowVMException($"VM Expression by name {tokens[0]} not found and will cannot parse", VirtualMachine.position, ExceptionType.NotFoundException);
                return null;
            }
            else
            {
                VirtualMachine.ThrowVMException("Value cannot find", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                return null;
            }
        }

        public static Value VoidValue => new Value(VirtualMachine.Void);

        public override string ToString() => $"VALUE:{type.fields.ToString()}\nTYPE:{type.ToString()}";

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
