using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class LengthExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            return new Value(new wolInt(((wolString)args[0].type).value.Length));
        }
    }
}
