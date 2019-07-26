using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    class Void : wolClass
    {
        public Void(SecurityModifer securityModifer = SecurityModifer.PUBLIC, wolClassType type = wolClassType.STRUCT, string ConstructorName = "void")
            : base(securityModifer, type, ConstructorName)
        {
        }

        public Void()
        {
            new Void();
            methods = new Dictionary<string, wolFunction>
            {
                { "toString", new wolFunction
                    {
                        arguments = new Dictionary<string, wolClass>
                        {
                            { "this", VirtualMachine.Void.Value }
                        },
                        returnType = VirtualMachine.Void.Value,
                        body = "Return : ( AddString : ( Typeof : @this ; ), <^:string>; );"
                    }
                }
            };
        }
    }
}
