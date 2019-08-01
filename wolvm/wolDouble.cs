using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolDouble : Void
    {
        public double value;

        public wolDouble() : base()
        {
            strtype = "double";
            constants = new Dictionary<string, Value>();
            methods = new Dictionary<string, wolFunction>
            {

            };
            parents = new Dictionary<string, wolClass>
            {
                { "void", VirtualMachine.Void }
            };
            constructors = new Dictionary<string, wolFunction>
            {

            };
        }

        public void ParseDouble(string val)
        {
            if (!double.TryParse(val, out value))
            {
                VirtualMachine.ThrowVMException($"'{val}' cannot parsing to double", VirtualMachine.position, ExceptionType.NumberFormatException);
            }
        }
    }
}
