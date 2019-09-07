using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    class BeepExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            Console.Beep();
            return Value.VoidValue;
        }
    }
}
