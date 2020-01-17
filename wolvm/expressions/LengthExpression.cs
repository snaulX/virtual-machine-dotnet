using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class LengthExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            wolClass type = args[0].type;
            if (type is wolString t)  return new Value(new wolInt(t.value.Length));
            else if (type is wolArray ty) return new Value(new wolInt(ty.value.Length));
            else
            {
                VirtualMachine.ThrowVMException("Type of argument in length can be only Array or string", VirtualMachine.position, ExceptionType.InvalidTypeException);
                return Value.VoidValue;
            }
        }
    }
}
