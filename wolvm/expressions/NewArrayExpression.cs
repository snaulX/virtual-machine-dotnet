using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class NewArrayExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            return new Value(new wolArray(Array.CreateInstance(typeof(Value), ((wolInt)args[0].type).value)));
        }
    }
}
