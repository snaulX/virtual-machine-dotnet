using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class Void : wolClass
    {
        public Void(SecurityModifer securityModifer = SecurityModifer.PUBLIC, wolClassType type = wolClassType.STRUCT, string ConstructorName = "void")
            : base(securityModifer, type, ConstructorName)
        {
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
