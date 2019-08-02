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
            constants = new Dictionary<string, Value>();
            //parents.Add("short", VirtualMachine.wolShort);
            fields = new Dictionary<string, Value>();
            methods = new Dictionary<string, wolFunction>();
            constructors = new Dictionary<string, wolFunction>
                    {
                        { "int", wolFunction.NewDefaultConstructor(this) }
                    };
            destructors = new List<wolFunction>
                    {
                        new wolFunction()
                    };
            Implements();
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
    }
}
