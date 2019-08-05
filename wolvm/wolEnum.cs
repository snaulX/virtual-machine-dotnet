using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolEnum : Void
    {
        public wolEnum() : base()
        {
            strtype = "Enum";
            classType = wolClassType.STATIC;
            parents = new Dictionary<string, wolClass>
            {
                { "void", VirtualMachine.Void }
            };
        }
    }
}
