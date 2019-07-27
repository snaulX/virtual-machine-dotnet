using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolType : Void
    {
        wolClass type;

        public wolType() : base()
        {
            classType = wolClassType.DEFAULT;
            constructors = new Dictionary<string, wolFunction>
            {
                { "Type", new wolFunction(SecurityModifer.PUBLIC,
                new KeyValuePair<string, wolClass>("type_name", VirtualMachine.wolString.Value)) }
            };
        }

        public wolType(string type_name) : this()
        {
            type = VirtualMachine.GetWolClass(type_name);
        }
    }
}
