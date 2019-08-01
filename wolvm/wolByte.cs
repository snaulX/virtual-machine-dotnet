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
            parents = new Dictionary<string, wolClass>
            {
                { "void", VirtualMachine.Void }
            };
            Implements();
        }

        public wolByte(byte val) : this() => value = val;
    }
}
