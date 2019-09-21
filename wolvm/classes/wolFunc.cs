using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolFunc : Void
    {
        public wolFunction value;

        public wolFunc() : base()
        {
            strtype = "Func";
            classType = wolClassType.DEFAULT;
            parents = new Dictionary<string, wolClass>
            {
                { "void", VirtualMachine.GetWolClass("void") }
            };
            constructors.Add("Func", wolFunction.NewDefaultConstructor(this)); //add empty constructor
        }

        public wolFunc(wolFunction func): this()
        {
            value = func;
        }

        public Value Call(params Value[] args)
        {
            Dictionary<string, Value> arguments = new Dictionary<string, Value>();
            int i = 0;
            try
            {
                foreach (string name in value.arguments.Keys)
                {
                    arguments.Add(name, args[i]);
                    i++;
                }
                return Script.Parse(value.body, arguments);
            }
            catch (NullReferenceException)
            {
                VirtualMachine.ThrowVMException("Function not found in 'Func' structure", VirtualMachine.position, ExceptionType.NullRefrenceException);
                return Value.VoidValue;
            }
        }
    }
}
