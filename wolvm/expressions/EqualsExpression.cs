using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class EqualsExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            return new Value(new wolBool(args[0] == args[1]));
        }
    }
}
