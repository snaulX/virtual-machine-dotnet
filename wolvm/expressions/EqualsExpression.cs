using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class EqualsExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            wolClass type = args[0].type;
            if (type is wolBool) return new Value(new wolBool(((wolBool)type).value == ((wolBool)args[1].type).value));
            else if (type is wolByte) return new Value(new wolBool(((wolByte)type).value == ((wolByte)args[1].type).value));
            else if (type is wolShort) return new Value(new wolBool(((wolShort)type).value == ((wolShort)args[1].type).value));
            else if (type is wolInt) return new Value(new wolBool(((wolInt)type).value == ((wolInt)args[1].type).value));
            else if (type is wolFloat) return new Value(new wolBool(((wolFloat)type).value == ((wolFloat)args[1].type).value));
            else if (type is wolLong) return new Value(new wolBool(((wolLong)type).value == ((wolLong)args[1].type).value));
            else if (type is wolDouble) return new Value(new wolBool(((wolDouble)type).value == ((wolDouble)args[1].type).value));
            else if (type is wolString) return new Value(new wolBool(((wolString)type).value == ((wolString)args[1].type).value));
            else if (type is wolChar) return new Value(new wolBool(((wolChar)type).value == ((wolChar)args[1].type).value));
            else if (type is wolBlock) return new Value(new wolBool(((wolBlock)type).body == ((wolBlock)args[1].type).body));
            else if (type is wolLink) return new Value(new wolBool(((wolLink)type).Address == ((wolLink)args[1].type).Address));
            else return new Value(new wolBool(args[0] == args[1]));
        }
    }
}
