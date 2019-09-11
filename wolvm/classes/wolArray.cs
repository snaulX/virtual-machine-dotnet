using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolArray : wolCollection
    {
        public wolArray() : base()
        {
            strtype = "Array";
            parents = new Dictionary<string, wolClass>
            {
                { "Collection", VirtualMachine.GetWolClass("Collection") }
            };
        }

        public wolArray(wolClass type) : this()
        {
            generic_type = type;
        }
    }
}
