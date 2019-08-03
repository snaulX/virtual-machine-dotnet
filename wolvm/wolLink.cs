using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolLink : Void
    {
        // ** Fields for simpler work with this type ** //
        public Value LinkedValue;
        public string Address;
        public bool HasSetter;

        public wolLink() : base()
        {
            strtype = "Link";
            classType = wolClassType.DEFAULT;
            fields.Add("Address", new Value(VirtualMachine.wolString, SecurityModifer.PUBLIC, true));
            fields.Add("HasSetter", new Value(VirtualMachine.wolBool, SecurityModifer.PUBLIC, true));
            constructors = new Dictionary<string, wolFunction>
            {
                { "href", new wolFunction
                {
                    returnType = this,
                    arguments = new Dictionary<string, wolClass>
                    {
                        { "link_name", VirtualMachine.wolString }
                    },
                    body = "Set : &this.Address, @link_name ;\n",
                    security = SecurityModifer.PUBLIC
                }
                }
            };
            Implements();
        }
        
        public wolLink(string link_name) : this()
        {
            Address = link_name;
            LinkedValue = VirtualMachine.mainstack.values[link_name];
        }

        public void RefreshLink()
        {
            LinkedValue = VirtualMachine.mainstack.values[Address];
        }

        public Value GetValue()
        {
            RefreshLink();
            return LinkedValue;
        }
    }
}
