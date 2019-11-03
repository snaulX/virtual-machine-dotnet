using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolDouble : wolFloat
    {
        public new double value;

        public wolDouble() : base()
        {
            strtype = "double";
            parents = new Dictionary<string, wolClass>
            {
                { "float", VirtualMachine.GetWolClass("float") }
            };
        }

        public wolDouble(double val) : this() => value = val;

        public void ParseDouble(string val)
        {
            if (!double.TryParse(val, out value))
            {
                VirtualMachine.ThrowVMException($"'{val}' cannot parsing to double", VirtualMachine.position, ExceptionType.NumberFormatException);
            }
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
