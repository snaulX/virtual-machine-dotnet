using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class PrintExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            foreach (Value arg in args)
                Console.Write(((wolString) arg.type).value);
            Console.WriteLine();
            return Value.VoidValue;
        }
    }
}
