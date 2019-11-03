using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolString : Void
    {
        public string value;

        public wolString(): base()
        {
            strtype = "string";
        }

        public wolString(string val) : this() => value = val;

        public override string ToString()
        {
            return value;
        }
    }
}
