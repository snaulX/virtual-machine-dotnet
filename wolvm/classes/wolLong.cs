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
            parents = new Dictionary<string, wolClass>
            {
                { "int", VirtualMachine.GetWolClass("int") }
            };
        }

        public wolLong(long val) : this() => value = val;

        public void ParseLong(string val)
        {
            if (!long.TryParse(val, out value))
            {
                VirtualMachine.ThrowVMException($"'{val}' cannot converted to long", VirtualMachine.position, ExceptionType.NumberFormatException);
            }
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
