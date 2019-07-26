using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class TypeofExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            Value retval = new Value(VirtualMachine.wolType.Value);
            return retval;
        }
    }
}
