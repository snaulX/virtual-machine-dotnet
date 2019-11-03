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
                { "void", VirtualMachine.GetWolClass("void") }
            };
        }

        public wolByte(byte val) : this() => value = val;

        public void ParseByte(string val)
        {
            if (!byte.TryParse(val, out value))
            {
                VirtualMachine.ThrowVMException($"'{val}' cannot converted to byte", VirtualMachine.position, ExceptionType.NumberFormatException);
            }
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
