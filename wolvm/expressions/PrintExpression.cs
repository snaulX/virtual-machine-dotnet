using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class PrintExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            foreach (Value arg in args)
            {
                try
                {
                    Console.Write(((wolString)arg.type).value);
                }
                catch (InvalidCastException)
                {
                    VirtualMachine.ThrowVMException("One of arguments of System.print expression haven`t type string", VirtualMachine.position, ExceptionType.InvalidTypeException);
                }
            }
            Console.WriteLine();
            return Value.VoidValue;
        }
    }
}
