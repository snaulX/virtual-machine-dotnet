using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class InputExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args) => new Value(new wolString(Console.ReadLine()));
    }
}
