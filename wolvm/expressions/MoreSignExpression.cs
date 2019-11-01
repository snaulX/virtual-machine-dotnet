using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class MoreSignExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            Value left = args[0], right = args[1];
            switch (left.type.strtype)
            {
                case "byte":
                    return new Value(new wolBool(((wolByte)left.type).value > ((wolByte)right.type).value));
                case "short":
                    return new Value(new wolBool(((wolShort)left.type).value > ((wolShort)right.type).value));
                case "int":
                    return new Value(new wolBool(((wolInt)left.type).value > ((wolInt)right.type).value));
                case "float":
                    return new Value(new wolBool(((wolFloat)left.type).value > ((wolFloat)right.type).value));
                case "long":
                    return new Value(new wolBool(((wolLong)left.type).value > ((wolLong)right.type).value));
                case "double":
                    return new Value(new wolBool(((wolDouble)left.type).value > ((wolDouble)right.type).value));
                case "char":
                    return new Value(new wolBool(((wolChar)left.type).value > ((wolChar)right.type).value));
                case "string":
                    return new Value(new wolBool(((wolString)left.type).value.Length > ((wolString)right.type).value.Length));
                default:
                    return Value.VoidValue;
            }
        }
    }
}
