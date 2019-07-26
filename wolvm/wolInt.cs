using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolInt : Void
    {
        public wolInt(SecurityModifer securityModifer = SecurityModifer.PUBLIC, wolClassType type = wolClassType.STRUCT, string ConstructorName = "int") 
            : base(securityModifer, type, ConstructorName)
        {
        }
    }
}
