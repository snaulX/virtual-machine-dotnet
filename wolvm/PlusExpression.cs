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
                wolDouble vald = (wolDouble) val.type;
                /*if (!double.TryParse(vald.fields["val"], out numb))
                {
                    VirtualMachine.ThrowVMException(vald.value + " isn`t double", VirtualMachine.position, ExceptionType.InvalidTypeException);
                }*/
                numb = vald.value;
                sum += numb;
            }
            Console.WriteLine(sum);
            return new Value(VirtualMachine.wolDouble, sum.ToString());
        }
    }
}
