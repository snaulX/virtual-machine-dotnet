using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolFunc : Void
    {
        public wolFunc() : base()
        {
            strtype = "Func";
            classType = wolClassType.DEFAULT;
            parents = new Dictionary<string, wolClass>
            {
                { "void", VirtualMachine.Void }
            };
        }
    }
}
