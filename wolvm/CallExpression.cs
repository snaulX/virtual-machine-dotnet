using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class CallExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            try
            {
                if (args[0].CheckType("Link")) //check type of first argument
                { 
                    //valid
                }
                else
                {
                    VirtualMachine.ThrowVMException("First argument is not Link", VirtualMachine.position, ExceptionType.InvalidTypeException);
                }
                
            }
            catch (IndexOutOfRangeException)
            {
                VirtualMachine.ThrowVMException("Need more arguments", VirtualMachine.position, ExceptionType.ArgumentsOutOfRangeException);
            }
            return Value.VoidValue;
        }
    }
}
