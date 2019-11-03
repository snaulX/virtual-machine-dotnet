using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolFloat : wolInt
    {
        public new float value;

        public wolFloat() : base()
        {
            strtype = "float";
            parents = new Dictionary<string, wolClass>
            {
                { "int", VirtualMachine.GetWolClass("int") }
            };
        }

        public wolFloat(float val) : this() => value = val;

        public void ParseFloat(string val)
        {
            if (!float.TryParse(val, out value))
                VirtualMachine.ThrowVMException($"'{val}' is not float", VirtualMachine.position, ExceptionType.NumberFormatException);
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
