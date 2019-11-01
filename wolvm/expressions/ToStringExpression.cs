namespace wolvm.expressions
{
    public class ToStringExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            Value val = args[0];
            if (val.CheckType("double")) return new Value(new wolString(((wolDouble)val.type).value.ToString()));
            else if (val.CheckType("int")) return new Value(new wolString(((wolInt)val.type).value.ToString()));
            else if (val.CheckType("float")) return new Value(new wolString(((wolFloat)val.type).value.ToString()));
            else if (val.CheckType("byte")) return new Value(new wolString(((wolByte)val.type).value.ToString()));
            else if (val.CheckType("short")) return new Value(new wolString(((wolShort)val.type).value.ToString()));
            else if (val.CheckType("long")) return new Value(new wolString(((wolLong)val.type).value.ToString()));
            else if (val.CheckType("bool")) return new Value(new wolString(((wolBool)val.type).value.ToString()));
            else if (val.CheckType("void")) return new Value(new wolString("null"));
            else if (val.CheckType("char")) return new Value(new wolString(((wolChar)val.type).value.ToString()));
            else return new Value(new wolString(System.Text.RegularExpressions.Regex.Escape(val.type.strtype)));
        }
    }
}
