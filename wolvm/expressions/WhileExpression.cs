using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class WhileExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            while (((wolBool) args[0].type).value)
            {
                Script.Parse(((wolBlock) args[1].type).body);
            }
            return Value.VoidValue;
        }
    }
}
