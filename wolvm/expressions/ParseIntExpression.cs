using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class ParseIntExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            wolInt type = new wolInt();
            type.ParseInt(((wolString)args[0].type).value);
            return new Value(type);
        }
    }
}
