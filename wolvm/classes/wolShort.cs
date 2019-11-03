using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolShort : wolByte
    {
        public new short value;

        public wolShort() : base()
        {
            strtype = "short";
            parents = new Dictionary<string, wolClass>
            {
                { "byte", VirtualMachine.GetWolClass("byte") }
            };
        }

        public wolShort(short val) : this() => value = val;

        public void ParseShort(string val)
        {
            if (!short.TryParse(val, out value))
            {
                VirtualMachine.ThrowVMException($"'{val}' cannot converted to short", VirtualMachine.position, ExceptionType.NumberFormatException);
            }
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
