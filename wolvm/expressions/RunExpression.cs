using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class RunExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            return Value.VoidValue;
        }
    }
}
