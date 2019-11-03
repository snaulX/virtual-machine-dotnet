using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolBool : Void
    {
        public bool value;

        public wolBool() : base()
        {
            value = true;
            strtype = "bool";
            constants = new Dictionary<string, Value>
            {
                { "false", new Value(new wolInt(0)) },
                { "true", new Value(new wolInt(1)) }
            };
            parents = new Dictionary<string, wolClass>
                    {
                        { "void", VirtualMachine.GetWolClass("void") }
                    };
            constructors = new Dictionary<string, wolFunction>
                    {
                        { "bool", wolFunction.NewDefaultConstructor(this) }
                    };
        }

        public wolBool(bool val): this()
        {
            value = val;
        }

        public void ParseBool(string val)
        {
            if (!bool.TryParse(val, out value))
                VirtualMachine.ThrowVMException($"'{val}' is not bool", VirtualMachine.position, ExceptionType.FormatException);
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
