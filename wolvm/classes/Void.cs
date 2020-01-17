using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class Void : wolClass
    {
        public Void()
        {
            classType = wolClassType.STRUCT;
            strtype = "void";
            methods = new Dictionary<string, wolFunction>();
            constants = new Dictionary<string, Value>();
            constructors = new Dictionary<string, wolFunction>();
            fields = new Dictionary<string, Value>();
            parents = new Dictionary<string, wolClass>();
            destructors = new List<wolFunction>();
        }

        public Void(object val) : this()
        {
            value = val;
        }

        public override string ToString() => "null";
    }
}
