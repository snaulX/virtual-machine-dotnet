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
            wolInt False = new wolInt(), True = new wolInt();
            False.ParseInt("0");
            True.ParseInt("1");
            constants = new Dictionary<string, Value>
            {
                { "false", new Value(False) },
                { "true", new Value(True) }
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

        public void ParseBool(string val)
        {
            if (!bool.TryParse(val, out value))
                VirtualMachine.ThrowVMException($"'{val}' is not bool", VirtualMachine.position, ExceptionType.FormatException);
        }
    }
}
