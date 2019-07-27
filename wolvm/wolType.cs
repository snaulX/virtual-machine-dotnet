using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolType : Void
    {
        wolClass type;
        public wolType(string type_name) : base()
        {
            type = VirtualMachine.GetWolClass(type_name);
            classType = wolClassType.DEFAULT;
            constructors = new Dictionary<string, wolFunction>
            {
                { "Type", new wolFunction(SecurityModifer.PUBLIC,
                new KeyValuePair<string, wolClass>("type_name", VirtualMachine.wolString.Value)) }
            };
        }
    }
}
