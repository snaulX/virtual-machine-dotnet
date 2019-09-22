using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class ToStringExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            Value val = args[0];
            if (val.CheckType("double")) return new Value(new wolString(((wolDouble)val.type).value.ToString()));
            else if (val.CheckType("int")) return new Value(new wolString(((wolInt)val.type).value.ToString()));
            else return new Value(new wolString(System.Text.RegularExpressions.Regex.Escape(val.type.strtype)));
        }
    }
}
