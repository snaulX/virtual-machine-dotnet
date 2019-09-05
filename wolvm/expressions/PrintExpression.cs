using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class PrintExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            string result = "";
            foreach (Value arg in args)
                result += ((wolString) arg.type).value;
            Console.Write(result);
            return Value.VoidValue;
        }
    }
}
