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
                { "void", VirtualMachine.Void }
            };
            constructors.Add("Func", wolFunction.NewDefaultConstructor(this)); //add empty constructor
        }

        public Value Call(params Value[] args) => Script.Parse(value.body); //will be fill soon
    }
}
