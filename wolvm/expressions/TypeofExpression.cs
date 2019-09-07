using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class TypeofExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args) => new Value(new wolType(), "Type", Value.GetValue($"<{args[0].type.ToString()}:string>"));
    }
}
