using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolBool : Void
    {
        public wolBool(SecurityModifer securityModifer = SecurityModifer.PUBLIC, wolClassType type = wolClassType.STRUCT, string ConstructorName = "bool") 
            : base(securityModifer, type, ConstructorName)
        {
            constants = new Dictionary<string, Value>
            {
                { "false", new Value(VirtualMachine.wolInt.Value, "<0:int>") },
                { "true", new Value(VirtualMachine.wolInt.Value, "<1:int>") }
            };
        }
    }
}
