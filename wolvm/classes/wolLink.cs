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
        public bool HasSetter;

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
            LinkedValue = VirtualMachine.mainstack.values[link_name];
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
            return LinkedValue;
        }
    }
}
