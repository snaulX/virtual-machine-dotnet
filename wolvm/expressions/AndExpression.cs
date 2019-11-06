using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm.expressions
{
    public class AndExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            try
            {
                bool parent = ((wolBool)args[0].type).value;
                foreach (Value val in args)
                {
                    parent = parent && ((wolBool)val.type).value;
                }
                return new Value(new wolBool(parent));
            }
            catch (Exception e)
            {
                if (e is InvalidCastException) VirtualMachine.ThrowVMException("Some type is invalid. 'and' operation", VirtualMachine.position, ExceptionType.InvalidTypeException);
                else if (e is IndexOutOfRangeException) VirtualMachine.ThrowVMException("Not enough arguments. 'and' operation", VirtualMachine.position, ExceptionType.ArgumentsOutOfRangeException);
                else VirtualMachine.ThrowVMException("Unknown exception. 'and' operation", VirtualMachine.position, ExceptionType.NotFoundException);
                return new Value(new wolBool(false));
            }
        }
    }
}
