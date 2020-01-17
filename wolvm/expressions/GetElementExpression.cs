using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class GetElementExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            //this is a temporary version
            wolClass type = args[0].type;
            int index = ((wolInt)args[1].type).value;
            if (type is wolString t) return new Value(new wolChar(t.value[index]));
            else if (type is wolArray ty) return new Value(new Void(ty.value.GetValue(index)));
            else
            {
                VirtualMachine.ThrowVMException("Type of argument in getByIndex can be only Array or string", VirtualMachine.position, ExceptionType.InvalidTypeException);
                return Value.VoidValue;
            }
        }
    }
}
