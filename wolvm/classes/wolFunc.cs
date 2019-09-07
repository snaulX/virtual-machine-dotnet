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

        public Value Call(params Value[] args) => Script.Parse(value.body); //will be fill soon
    }
}
