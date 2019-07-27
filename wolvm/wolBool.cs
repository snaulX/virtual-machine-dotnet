using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolBool : Void
    {
        public wolBool() : base()
        {
            constants = new Dictionary<string, Value>
            {
                { "false", new Value(VirtualMachine.wolInt.Value, "<0:int>") },
                { "true", new Value(VirtualMachine.wolInt.Value, "<1:int>") }
            };
            parents = new Dictionary<string, wolClass>
                    {
                        { VirtualMachine.Void.Key, VirtualMachine.Void.Value }
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
        }
    }
}
