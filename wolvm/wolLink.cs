using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolLink : Void
    {
        public wolLink() : base()
        {
            strtype = "Link";
            classType = wolClassType.DEFAULT;
            fields.Add("Address", new Value(VirtualMachine.wolString, SecurityModifer.PUBLIC, true));
            constructors = new Dictionary<string, wolFunction>
            {
                { "href", new wolFunction
                {
                    arguments = new Dictionary<string, wolClass>
                    {
                        { "link_name", VirtualMachine.wolString }
                    },
                    body = "Set : &this.Address, @link_name ;"
                }
                }
            };
            Implements();
        }
    }
}
