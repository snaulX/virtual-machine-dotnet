using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolBool : Void
    {
        public wolBool() : base()
        {
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
                        { "void", VirtualMachine.Void }
                    };
            fields = new Dictionary<string, Value>();
            methods = new Dictionary<string, wolFunction>();
            constructors = new Dictionary<string, wolFunction>
                    {
                        { "bool", new wolFunction() }
                    };
            destructors = new List<wolFunction>
                    {
                        new wolFunction()
                    };
            Implements();
        }
    }
}
