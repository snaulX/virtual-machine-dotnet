using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolInt : wolShort
    {
        public new int value;

        public wolInt() : base()
        {
            strtype = "int";
            parents = new Dictionary<string, wolClass>
            {
                { "short", VirtualMachine.GetWolClass("short") }
            };
            constructors.Add("int", wolFunction.NewDefaultConstructor(this));
        }

        public wolInt(int val) : this() => value = val;

        public bool TryParseInt(string val) => int.TryParse(val, out value);
        
        public void ParseInt(string val)
        {
            if (!int.TryParse(val, out value))
            {
                VirtualMachine.ThrowVMException($"'{val}' cannot convert to int", VirtualMachine.position, ExceptionType.NumberFormatException);
            }
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
