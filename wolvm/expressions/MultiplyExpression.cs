using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class MultiplyExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            wolClass type = args[0].type;
            if (type is wolByte)
            {
                byte sum = 0;
                foreach (Value val in args)
                {
                    wolByte vald = (wolByte)val.type;
                    sum *= vald.value;
                }
                return new Value(new wolByte(sum));
            }
            else if (type is wolShort)
            {
                short sum = 0;
                foreach (Value val in args)
                {
                    wolShort vald = (wolShort)val.type;
                    sum *= vald.value;
                }
                return new Value(new wolShort(sum));
            }
            else if (type is wolInt)
            {
                int sum = 0;
                foreach (Value val in args)
                {
                    wolInt vald = (wolInt)val.type;
                    sum *= vald.value;
                }
                return new Value(new wolInt(sum));
            }
            else if (type is wolFloat)
            {
                float sum = 0f;
                foreach (Value val in args)
                {
                    wolFloat vald = (wolFloat)val.type;
                    sum *= vald.value;
                }
                return new Value(new wolFloat(sum));
            }
            else if (type is wolLong)
            {
                long sum = 0;
                foreach (Value val in args)
                {
                    wolLong vald = (wolLong)val.type;
                    sum *= vald.value;
                }
                return new Value(new wolLong(sum));
            }
            else if (type is wolDouble)
            {
                double sum = 0.0;
                foreach (Value val in args)
                {
                    wolDouble vald = (wolDouble)val.type;
                    sum *= vald.value;
                }
                return new Value(new wolDouble(sum));
            }
            else if (type is wolChar)
            {
                char result = '\0';
                foreach (Value val in args)
                {
                    wolChar valc = (wolChar)val.type;
                    result *= valc.value;
                }
                return new Value(new wolChar(result));
            }
            else if (type is wolString)
            {
                string result = "";
                wolInt vald = (wolInt)args[1].type;
                for (int i = 0; i < vald.value; i++)
                {
                    result += result;
                }
                return new Value(new wolString(result));
            }
            else
            {
                VirtualMachine.ThrowVMException($"Value with type {type.strtype} not support multiply", VirtualMachine.position, ExceptionType.TypeNotSupportedException);
                return Value.VoidValue;
            }
        }
    }
}
