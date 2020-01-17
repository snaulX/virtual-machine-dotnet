using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolType : Void
    {
        public new wolClass value;

        public wolType() : base()
        {
            strtype = "Type";
            classType = wolClassType.DEFAULT;
            constructors.Add("Type", new wolFunction
                    {
                        returnType = this,
                        arguments = new Dictionary<string, wolClass>
                        {
                            { "name", new wolString() }
                        },
                        body = "set : &@this.@name, @name ;",
                        security = SecurityModifer.PUBLIC
                    }
                );
            fields.Add("name",  new Value(new wolString(), SecurityModifer.PUBLIC, true));
        }

        public wolType(string type_name) : this()
        {
            value = VirtualMachine.GetWolClass(type_name);
        }
    }
}
