using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class TypeofExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args) => new Value(VirtualMachine.wolType.Value, "Type", Value.GetValue($"<{args[0].type.ToString()}:string>"));
    }
}
