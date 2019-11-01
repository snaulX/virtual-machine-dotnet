using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class ParseDoubleExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            wolDouble type = new wolDouble();
            type.ParseDouble(((wolString)args[0].type).value);
            return new Value(type);
        }
    }
}
