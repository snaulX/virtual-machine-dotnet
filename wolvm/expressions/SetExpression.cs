using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class SetExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            Console.WriteLine(args[0].type);
            ((wolLink)args[0].type).SetValue(args[1]);
            return Value.VoidValue;
        }
    }
}
