using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class ParseDoubleExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            return new Value(new wolString(((wolDouble)args[0].type).value.ToString()));
        }
    }
}
