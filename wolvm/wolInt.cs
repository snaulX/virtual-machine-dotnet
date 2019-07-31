using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolInt : Void
    {
        public int value;

        public wolInt() : base()
        {
            strtype = "int";
            constants = new Dictionary<string, Value>();
            parents = new Dictionary<string, wolClass>
                    {
                        { "void", VirtualMachine.Void }
                    };
            fields = new Dictionary<string, Value>();
            methods = new Dictionary<string, wolFunction>();
            constructors = new Dictionary<string, wolFunction>
                    {
                        { "int", new wolFunction() }
                    };
            destructors = new List<wolFunction>
                    {
                        new wolFunction()
                    };
        }

        public wolInt(int val) : this()
        {
            value = val;
        }

        public bool TryParseInt(string val) => int.TryParse(val, out value);
        
        public void ParseInt(string val)
        {
            if (!int.TryParse(val, out value))
            {
                VirtualMachine.ThrowVMException("", VirtualMachine.position, ExceptionType.NumberFormatException);
            }
        }
    }
}
