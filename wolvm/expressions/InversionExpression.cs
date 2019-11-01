using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class InversionExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            return new Value(new wolBool(!((wolBool)args[0].type).value));
        }
    }
}
