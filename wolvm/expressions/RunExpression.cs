using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class RunExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            //for (int i = 0; i < args.Length; i++)
            //{
                ((wolBlock)args[0].type).Run();
            //}
            return Value.VoidValue;
        }
    }
}
