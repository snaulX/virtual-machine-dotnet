using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolLong : wolInt
    {
        public new long value;

        public wolLong() : base()
        {
            strtype = "long";
            Implements();
        }

        public wolLong(long val) : this() => value = val;

        public void ParseLong(string val)
        {
            if (!long.TryParse(val, out value))
            {
                VirtualMachine.ThrowVMException($"'{val}' cannot converted to long", VirtualMachine.position, ExceptionType.NumberFormatException);
            }
        }
    }
}
