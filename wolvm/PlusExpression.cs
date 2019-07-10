using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class PlusExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            double sum = 0.0;
            foreach (Value val in args)
            {
                double numb = 0.0;
                if (!double.TryParse(val.value, out numb))
                {
                    VirtualMachine.ThrowVMException(val.value + " isn`t double", VirtualMachine.position, ExceptionType.InvalidTypeException);
                }
                sum += numb;
            }
            Console.WriteLine(sum);
            return new Value(VirtualMachine.wolDouble.Value, sum.ToString());
        }
    }
}
