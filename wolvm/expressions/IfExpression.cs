using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class IfExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            for (int i = 0; i < args.Length; i += 2)
            {
                if (i == args.Length - 1)
                    return Script.Parse(((wolBlock)args[i].type).body);
                if (((wolBool)args[i].type).value)
                    return Script.Parse(((wolBlock)args[i + 1].type).body);
            }
            return Value.VoidValue; //for dont throwing compiler error
        }
    }
}
