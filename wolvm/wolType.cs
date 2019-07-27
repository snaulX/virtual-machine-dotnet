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
                { "Type", new wolFunction
                    {
                        arguments = new Dictionary<string, wolClass>
                        {
                            { "name", VirtualMachine.wolString.Value }
                        },
                        body = "@this.name#set : @name ;"
                    }
                }
            };
            fields = new Dictionary<string, Value>
            {
                { "name",  new Value(VirtualMachine.wolString.Value, "<null:void>", SecurityModifer.PUBLIC) }
            };
        }

        public wolType(string type_name) : this()
        {
            type = VirtualMachine.GetWolClass(type_name);
        }
    }
}
