using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolInt : Void
    {
        public wolInt() : base()
        {
            constants = new Dictionary<string, Value>();
            parents = new Dictionary<string, wolClass>
                    {
                        { VirtualMachine.Void.Key, VirtualMachine.Void.Value }
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
    }
}
