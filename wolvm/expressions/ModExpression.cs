using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class ModExpression: VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            wolClass type = args[0].type;
            if (type is wolFloat)
            {
                int sum = 0;
                wolInt vald = (wolInt)args[1].type;
                sum %= vald.value;
                return new Value(new wolInt(sum));
            }
            else if (type is wolDouble)
            {
                long sum = 0;
                wolLong vald = (wolLong)args[1].type;
                sum %= vald.value;
                return new Value(new wolLong(sum));
            }
            else
            {
                VirtualMachine.ThrowVMException($"Value with type {type.strtype} not support mod", VirtualMachine.position, ExceptionType.TypeNotSupportedException);
                return Value.VoidValue;
            }
        }
    }
}
