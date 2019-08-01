using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class Void : wolClass
    {
        public Void() : base()
        {
            strtype = "void";
            methods = new Dictionary<string, wolFunction>
            {
                { "toString", new wolFunction
                    {
                        arguments = new Dictionary<string, wolClass>
                        {
                            { "this", this }
                        },
                        returnType = VirtualMachine.wolString,
                        body = "return ( AddString : ( Typeof : @this ; ), <^:string>; );"
                    }
                },
                { "getType", new wolFunction
                    {
                        arguments = new Dictionary<string, wolClass>
                        {
                            { "this", this }
                        },
                        returnType = VirtualMachine.wolType,
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
                        body = "return <null:void>;",
                        returnType = this
                    }
                }
            };
            fields = new Dictionary<string, Value>();
            parents = new Dictionary<string, wolClass>();
            destructors = new List<wolFunction>
            {
                new wolFunction(SecurityModifer.PUBLIC, "Destroy : &this ;")
            };
        }
    }
}
