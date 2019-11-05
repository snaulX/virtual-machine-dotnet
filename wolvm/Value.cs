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

        /// <summary>
        /// Create default value with type, security modifer and flag which ask of generator of setter
        /// </summary>
        /// <param name="wolclass">Type of generated value</param>
        /// <param name="modifer">Security modifer of generated value</param>
        /// <param name="isConstant">Flag which ask of generator of setter</param>
        public Value(wolClass wolclass, SecurityModifer modifer = SecurityModifer.PRIVATE, bool isConstant = false)
        {
            type = wolclass;
            getter = new wolFunction(modifer, "return @this;");
            if (!isConstant)
                setter = new wolFunction(SecurityModifer.PRIVATE, "set : &@this, @_this ;");
                /*{
                    security = SecurityModifer.PRIVATE, body = "set : &@this, @_this ;",
                    arguments = new Dictionary<string, wolClass> { { "_this", wolclass } }
                };*/
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

        /// <summary>
        /// Get field from type of this value
        /// </summary>
        /// <param name="name">Name of field</param>
        /// <returns></returns>
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

        public bool CheckType(string name) => name == type.strtype ? true : false; //thanks C# for one-string functions))

        public static Value GetSmallValue(string val, Value parent = null)
        {
            //Console.WriteLine("Value is " + val);
            Value value = VoidValue;
            val = val.Trim();
            if (val.StartsWith("<") && val.EndsWith(">")) //example of syntax - _loads : <wolSystem:string> ;
            {
                if (parent != null)
                {
                    VirtualMachine.ThrowVMException("Default value cannot have parent value", VirtualMachine.position, ExceptionType.ValueException);
                }
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
                        type.ParseBool(val_word);
                        value = new Value(type);
                    }
                    else if (type_word == "void")
                    {
                        value = VoidValue;
                    }
                    else if (type_word == "short")
                    {
                        wolShort type = new wolShort();
                        type.ParseShort(val_word);
                        value = new Value(type);
                    }
                    else if (type_word == "float")
                    {
                        wolFloat type = new wolFloat();
                        type.ParseFloat(val_word);
                        value = new Value(type);
                    }
                    else if (type_word == "byte")
                    {
                        wolByte type = new wolByte();
                        type.ParseByte(val_word);
                        value = new Value(type);
                    }
                    else if (type_word == "char")
                    {
                        wolChar type = new wolChar();
                        type.ParseChar(val_word);
                        value = new Value(type);
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
                //Console.WriteLine(val);
                val = val.Remove(0, 1); //skip '@'
                if (parent != null)
                {
                    if (parent.CheckType("Type"))
                    {
                        return ((wolType) parent.type).value.GetStaticField(val);
                    }
                    else
                    {
                        return parent.GetField(val);
                    }
                }
                else
                {
                    try
                    {
                        value = VirtualMachine.mainstack.values[val];
                    }
                    catch (KeyNotFoundException)
                    {
                        VirtualMachine.ThrowVMException($"Variable by name '{val}' not found in main stack", VirtualMachine.position, ExceptionType.NotFoundException);
                    }
                    return value;
                }
            }
            else if (val.StartsWith("&")) //example of syntax - set : &@this, <null:void> ;
            {
                val = val.Remove(0, 1); //remove '&'
                if (parent != null)
                {
                    VirtualMachine.ThrowVMException("Link cannot haven`t ParentValue. He can have only valid address", VirtualMachine.position, ExceptionType.ValueException);
                    return new Value(new wolLink());
                }
                else
                {
                    return new Value(new wolLink(val));
                }
            }
            else if (val.StartsWith("#")) //example of syntax - set : &@this, #sum ;
            {
                val = val.Remove(0, 1); //remove '#'
                if (parent != null)
                {
                    if (parent.CheckType("Type"))
                    {
                        return new Value(new wolFunc(((wolType) parent.type).value.GetStaticMethod(val))); //one string or how make code unreadable
                    }
                    else
                    {
                        if (val == "set") return new Value(new wolFunc(parent.setter));
                        else if (val == "get") return new Value(new wolFunc(parent.getter));
                        else return new Value(new wolFunc(parent.GetMethod(val))); //return not static method of ParentValue by name
                    }
                }
                else
                {
                    return VirtualMachine.FindFunc(val); //one string)
                }
            }
            else if (val.StartsWith("$")) //example of syntax - equals : $void, (typeof : <null:void>) ;
            {
                if (parent != null)
                {
                    VirtualMachine.ThrowVMException("Class (Type) cannot have parent value", VirtualMachine.position, ExceptionType.ValueException);
                }
                return new Value(new wolType(val.Remove(0, 1))); //let`s write in one string!!!
            }
            else if (val.StartsWith("%")) //example of syntax - if : ( equals : $void, (typeof : <null:void>) ), %if_block1 ;
            {
                if (parent != null)
                {
                    VirtualMachine.ThrowVMException("Block cannot have parent value", VirtualMachine.position, ExceptionType.ValueException);
                }
                return VirtualMachine.FindBlock(val.Remove(0, 1)); //one string again!
            }
            else if (val.StartsWith("(")) //example of syntax - return (typeof : @this ) ;
            {
                if (parent != null)
                {
                    VirtualMachine.ThrowVMException("Expression cannot have parent value", VirtualMachine.position - val.Length, ExceptionType.ValueException);
                }
                StringBuilder buffer = new StringBuilder();
                char current = val[1]; //skip '('
                int pos = 1; //skip '('
                byte priority = 0;
                while (true) //add body of expression
                {
                    if (current == '(') priority++;
                    else if (current == ')')
                    {
                        if (priority == 0) break;
                        else priority--;
                    }
                    try
                    {
                        buffer.Append(current);
                        current = val[++pos];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        VirtualMachine.ThrowVMException($"End of string expression ('{val}') not found", VirtualMachine.position - val.Length + pos, ExceptionType.BLDSyntaxException);
                    }
                }
                return Script.ParseExpression(buffer.ToString());
            }
            else
            {
                VirtualMachine.ThrowVMException($"Value {val} cannot find", VirtualMachine.position - val.Length, ExceptionType.BLDSyntaxException);
                return null;
            }
        }

        public static Value GetValue(string val)
        {
            string[] small_vals = val.Trim().Split('.');
            Value parent = null; //it`s parent value
            foreach (string strval in small_vals)
            {
                parent = GetSmallValue(strval, parent);
            }
            return parent;
        }

        /// <summary>
        /// Return default value - null - nothing
        /// </summary>
        public static Value VoidValue => new Value(VirtualMachine.GetWolClass("void"));

        public override string ToString()
        {
            try
            {
                string str = ((wolString)type).value; //it`s a pass
                return $"VALUE:{str}\nTYPE:{type.ToString()}";
            }
            catch (InvalidCastException)
            {
                return type.ToString();
            }
        }

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
