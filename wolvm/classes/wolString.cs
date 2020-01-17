using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolString : Void
    {
        public new string value;

        public wolString(): base()
        {
            strtype = "string";
            parents = new Dictionary<string, wolClass>
            {
                { "void", VirtualMachine.GetWolClass("void") }
            };
        }

        public wolString(string val) : this() => value = val;

        public override string ToString()
        {
            return value;
        }
    }
}
