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
            fields.Add("Address", new Value(new wolString(), SecurityModifer.PUBLIC, true));
            fields.Add("HasSetter", new Value(new wolBool(), SecurityModifer.PUBLIC, true));
            constructors.Add("href", new wolFunction
            {
                returnType = this,
                arguments = new Dictionary<string, wolClass>
                    {
                        { "link_name", new wolString() }
                    },
                body = "set : &this.Address, @link_name ;\n",
                security = SecurityModifer.PUBLIC
            });
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
