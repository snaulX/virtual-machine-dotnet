using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class GetElementExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            //this is a temporary version
            wolClass type = args[0].type;
            if (type is wolString) return new Value(new wolChar(((wolString)type).value[((wolInt)type).value]));
            else return Value.VoidValue;
        }
    }
}
