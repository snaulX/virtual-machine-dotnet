using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wolvm
{
    public class wolChar : Void
    {
        public char value;
        public wolChar() : base()
        {
            strtype = "char";
            parents = new Dictionary<string, wolClass>
            {
                { "void", VirtualMachine.GetWolClass("void") }
            };
        }

        public wolChar(char val) : this() => value = val;

        public void ParseChar(string val)
        {
            if (!char.TryParse(val, out value))
                VirtualMachine.ThrowVMException($"'{val}' is not char", VirtualMachine.position, ExceptionType.FormatException);
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
