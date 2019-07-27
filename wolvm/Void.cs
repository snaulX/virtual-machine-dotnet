﻿using System;
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
                        body = "Return : ( AddString : ( Typeof : @this ; ), <^:string>; );"
                    }
                }
            };
            constants = new Dictionary<string, Value>
            {
                { "null", new Value(this) }
            };
        }
    }
}
