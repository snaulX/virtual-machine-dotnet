using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolLink : Void
    {
        // ** Fields for simpler work with this type ** //
        public Value LinkedValue;
        public string Address;
        public bool HasSetter = true;

        public wolLink() : base()
        {
            strtype = "Link";
            classType = wolClassType.DEFAULT;
            fields.Add("Address", new Value(new wolString(), SecurityModifer.PUBLIC, true));
            fields.Add("HasSetter", new Value(new wolBool(), SecurityModifer.PUBLIC, true));
            //constructor add in extenstion constructors in libraries (not in vm)
        }

        public wolLink(string link_name) : this()
        {
            Address = link_name;
            LinkedValue = ParseLink(link_name);
            try
            {
                if (LinkedValue.setter == null)
                {
                    HasSetter = false;
                }
            }
            catch (NullReferenceException)
            {
                HasSetter = false; //XD this will can to be
            }
        }

        public static Value ParseLink(string address)
        {
            string[] small_vals = address.Trim().Split('?'); //get parts of address
            Value parent = null; //it`s parent value
            foreach (string strval in small_vals)
            {
                parent = Value.GetSmallValue(strval, parent); //get value from every part of address
            }
            return parent; //last getting value is return out function
        }

        public Value GetValue()
        {
            LinkedValue = ParseLink(Address);
            if (LinkedValue.setter == null)
            {
                HasSetter = false;
            }
            else
            {
                HasSetter = true;
            }
            return LinkedValue;
        }

        public void SetValue(Value value)
        {
            string[] small_vals = Address.Split('?'); //get parts of address
            Value parent = null; //it`s parent value
            for (int i = 0; i < small_vals.Length; i++)
            {
                bool end = false;
                string val = small_vals[i];
                if (i == small_vals.Length - 1) end = true;
                if (val.StartsWith("@")) //example of syntax - plus : @a, @b ;
                {
                    val = val.Remove(0, 1);
                    if (parent != null)
                    {
                        if (parent.CheckType("Type"))
                        {
                            if (!end) parent = ((wolType)parent.type).value.GetStaticField(val);
                            else ((wolType)parent.type).value.static_fields[val] = value;
                        }
                        else
                        {
                            if (!end) parent = parent.GetField(val);
                            else parent.type.fields[val] = value;
                        }
                    }
                    else
                    {
                        try
                        {
                            if (!end) parent = VirtualMachine.mainstack.values[val];
                            else VirtualMachine.mainstack.values[val] = value;
                        }
                        catch (KeyNotFoundException)
                        {
                            VirtualMachine.ThrowVMException($"Variable by name '{val}' not found in main stack", VirtualMachine.position, ExceptionType.NotFoundException);
                        }
                    }
                }
                else if (val.StartsWith("&")) //example of syntax - set : &this, <null:void> ;
                {
                    val = val.Remove(0, 1);
                    if (parent != null)
                    {
                        if (parent.CheckType("Type"))
                        {
                            if (!end) parent = ((wolType)parent.type).value.GetStaticField(val);
                            else ((wolType)parent.type).value.static_fields[val] = value;
                        }
                        else
                        {
                            if (!end) parent = parent.GetField(val);
                            else parent.type.fields[val] = value;
                        }
                    }
                    else
                    {
                        if (!end) parent = VirtualMachine.mainstack.values[val];
                        else VirtualMachine.mainstack.values[val] = value;
                    }
                }
                else if (val.StartsWith("#")) //example of syntax - set : &this, #sum ;
                {
                    val = val.Remove(0, 1); //remove '#'
                    if (parent != null)
                    {
                        if (parent.CheckType("Type"))
                        {
                            ((wolType)parent.type).value.static_fields[val] = value;
                        }
                        else
                        {
                            parent.type.methods[val] = ((wolFunc)value.type).value; //return not static method of ParentValue by name
                        }
                    }
                    else
                    {
                        VirtualMachine.mainstack.functions[val] = ((wolFunc)value.type).value;
                    }
                }
                else if (val.StartsWith("$")) //example of syntax - equals : $void, (typeof : <null:void>) ;
                {
                    if (parent != null)
                    {
                        VirtualMachine.ThrowVMException("Class (Type) cannot have parent value", VirtualMachine.position, ExceptionType.ValueException);
                    }
                    if (!end) parent = new Value(new wolType(val.Remove(0, 1)));
                    else VirtualMachine.mainstack.classes[val.Remove(0, 1)] = ((wolType)value.type).value;
                }
                else if (val.StartsWith("%")) //example of syntax - if : ( equals : $void, (typeof : <null:void>) ), %if_block1 ;
                {
                    if (parent != null)
                    {
                        VirtualMachine.ThrowVMException("Block cannot have parent value", VirtualMachine.position, ExceptionType.ValueException);
                    }
                    val = val.Remove(0, 1);
                    if (!end)
                    {
                        parent = VirtualMachine.FindBlock(val);
                    }
                    else
                    {
                        if (!VirtualMachine.mainstack.values[val].CheckType("Block"))
                            VirtualMachine.ThrowVMException($"Variable by name {val} not found", VirtualMachine.position, ExceptionType.NotFoundException);
                        else
                            VirtualMachine.mainstack.values[val] = value;

                    }
                }
                else
                {
                    VirtualMachine.ThrowVMException("Invalid syntax of linked value", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                }
            }
        }

        public override string ToString()
        {
            return "wolvm::mainstack::Link:" + Address;
        }
    }
}
