using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class AddStringExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            string result = "";
            foreach (Value arg in args)
            {
                if (arg.type is wolString)
                    result += ((wolString)arg.type).value;
                else
                    VirtualMachine.ThrowVMException("Argument in AddString haven`t type 'string'", VirtualMachine.position, ExceptionType.InvalidTypeException);
            }
            return new Value(new wolString(result));
        }
    }
}
