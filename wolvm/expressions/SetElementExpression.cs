using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class SetElementExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            if (args[0].type is wolArray ty) ty.value.SetValue(args[1], ((wolInt)args[2].type).value);
            else VirtualMachine.ThrowVMException("Type of argument in setElem can be only Array", VirtualMachine.position, ExceptionType.InvalidTypeException);
            return Value.VoidValue;
        }
    }
}
