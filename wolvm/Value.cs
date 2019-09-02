using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
                        Script.Parse(type.constructors[constr_name].body);
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

        public static Value GetSmallValue(string val)
        {
            Value value;
            val = val.Trim();
            if (val.StartsWith("<") && val.EndsWith(">")) //example of syntax - Loads : <System:string> ;
            {
                val = val.Remove(0, 1).Remove(val.Length - 2); //remove '<' and '>'
                string[] vals = val.Split(':');
                if (vals.Length == 2)
                {
                    string type_word = vals[1].Trim(), val_word = vals[0].Trim();
                    if (type_word == "double")
                    {
                        wolDouble type = new wolDouble();
                        type.ParseDouble(val_word);
                        value = new Value(type);
                    }
                    else if (type_word == "int")
                    {
                        wolInt type = new wolInt();
                        type.ParseInt(val_word);
                        value = new Value(type);
                    }
                    else if (type_word == "string")
                    {
                        wolString type = new wolString();
                        type.value = Regex.Unescape(vals[0]);
                        value = new Value(type);
                    }
                    else if (type_word == "long")
                    {
                        wolLong type = new wolLong();
                        type.ParseLong(val_word);
                        value = new Value(type);
                    }
                    else if (type_word == "bool")
                    {
                        wolBool type = new wolBool();
                        value = new Value(type);
                    }
                    else if (type_word == "void")
                    {
                        value = VoidValue;
                    }
                    else
                    {
                        value = new Value(VirtualMachine.GetWolClass(type_word));
                        foreach (string f in vals[0].Split(','))
                        {
                            string[] fs = f.Split('=');
                            try
                            {
                                value.type.fields[fs[0].Trim()] = GetValue(fs[1]);
                            }
                            catch (KeyNotFoundException)
                            {
                                VirtualMachine.ThrowVMException($"Field by name {fs[0]} not found", VirtualMachine.position, ExceptionType.NotFoundException);
                            }
                        }
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
                return VoidValue; //pass
            }
            else if (val.StartsWith("&")) //example of syntax - set : &this, <null:void> ;
            {
                return new wolLink(val.Remove(0, 1)).GetValue(); //one string!!!
            }
            else if (val.StartsWith("#")) //example of syntax - set : &this, #sum ;
            {
                val = val.Remove(0, 1); //remove '#'
                wolFunc type = new wolFunc();
                value = new Value(type); //create empty value with type Func
                return value;
            }
            else if (val.StartsWith("$")) //example of syntax - equals : $void, (typeof : <null:void>) ;
            {
                return new Value(new wolType(val.Remove(0, 1))); //let`s write in one string!!!
            }
            else if (val.StartsWith("%")) //example of syntax - if : ( equals : $void, (typeof : <null:void>) ), %if_block1 ;
            {
                return VirtualMachine.FindBlock(val.Remove(0, 1)); //one string again!
            }
            else if (val.StartsWith("(")) //example of syntax - return (typeof : @this ) ;
            {
                StringBuilder buffer = new StringBuilder();
                char current = val[1]; //skip '('
                int pos = 1; //skip '('
                while (current != ')') //add body of expression
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
                return Script.ParseExpression(buffer.ToString());
            }
            else
            {
                VirtualMachine.ThrowVMException("Value cannot find", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                return null;
            }
        }

        public static Value GetValue(string val)
        {
            string[] small_vals = val.Trim().Split('.');
            Value value = VoidValue; //it`s pass
            return value;
        }

        public static Value VoidValue => new Value(VirtualMachine.Void);

        public override string ToString() => $"VALUE:{type.fields.ToString()}\nTYPE:{type.ToString()}"; //is test version

        /// <summary>
        /// Get not static method in this value
        /// </summary>
        /// <param name="name">Name of not static method</param>
        /// <returns></returns>
        public wolFunction GetMethod(string name)
        {
            wolFunction meth = new wolFunction(); //create empty funciton for that compiler don`t get error
            try
            {
                meth = type.methods[name];
            }
            catch (KeyNotFoundException)
            {
                VirtualMachine.ThrowVMException("Method by name  not found", VirtualMachine.position, ExceptionType.NotFoundException);
            }
            if (meth.arguments.ContainsKey("this")) return meth; //check on 'static'
            else return null;
        }
    }
}
