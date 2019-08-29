using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class LoadsExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            return Value.VoidValue;
        }
    }
}
