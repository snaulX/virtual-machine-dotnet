using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class PlusExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            wolClass type = args[0].type;
            if (type is wolInt)
            {
                int sum = 0;
                foreach (Value val in args)
                {
                    wolInt vald = (wolInt)val.type;
                    sum += vald.value;
                }
                return new Value(new wolInt(sum));
            }
            else if (type is wolFloat)
            {
                float sum = 0f;
                foreach (Value val in args)
                {
                    wolFloat vald = (wolFloat)val.type;
                    sum += vald.value;
                }
                return new Value(new wolFloat(sum));
            }
            else if (type is wolDouble)
            {
                double sum = 0.0;
                foreach (Value val in args)
                {
                    wolDouble vald = (wolDouble)val.type;
                    sum += vald.value;
                }
                return new Value(new wolDouble(sum));
            }
            else if (type is wolString)
            {
                string result = "";
                foreach (Value val in args)
                {
                    wolString vald = (wolString)val.type;
                    result += vald.value;
                }
                return new Value(new wolString(result));
            }
            else
            {
                VirtualMachine.ThrowVMException($"Value with type {type.strtype} not support plus", VirtualMachine.position, ExceptionType.TypeNotSupportedException);
                return Value.VoidValue;
            }
        }
    }
}
