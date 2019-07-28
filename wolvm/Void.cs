using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class Void : wolClass
    {
        public Void() : base()
        {
            methods = new Dictionary<string, wolFunction>
            {
                { "toString", new wolFunction
                    {
                        arguments = new Dictionary<string, wolClass>
                        {
                            { "this", this }
                        },
                        returnType = VirtualMachine.wolString.Value,
                        body = "return ( AddString : ( Typeof : @this ; ), <^:string>; );"
                    }
                },
                { "getType", new wolFunction
                    {
                        arguments = new Dictionary<string, wolClass>
                        {
                            { "this", this }
                        },
                        returnType = VirtualMachine.wolType.Value,
                        body = "return ( Typeof : @this ; );"
                    }
                }
            };
            constants = new Dictionary<string, Value>
            {
                { "null", new Value(this) }
            };
            constructors = new Dictionary<string, wolFunction>
            {
                { "void", new wolFunction
                {

                }
                }
            };
        }
    }
}
