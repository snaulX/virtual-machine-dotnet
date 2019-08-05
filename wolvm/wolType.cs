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
            strtype = "Type";
            classType = wolClassType.DEFAULT;
            constructors.Add("Type", new wolFunction
                    {
                        returnType = this,
                        arguments = new Dictionary<string, wolClass>
                        {
                            { "name", VirtualMachine.wolString }
                        },
                        body = "Set : &this.name, @name ;",
                        security = SecurityModifer.PUBLIC
                    }
                );
            fields.Add("name",  new Value(VirtualMachine.wolString, SecurityModifer.PUBLIC, true));
        }

        public wolType(string type_name) : this()
        {
            type = VirtualMachine.GetWolClass(type_name);
        }
    }
}
