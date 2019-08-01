using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolByte : Void
    {
        public byte value;

        public wolByte() : base()
        {
            strtype = "byte";
        }

        public wolByte(byte val) : this() => value = val;
    }
}
