using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public interface VMExpression
    {
        Value ParseExpression(params Value[] args);
    }
}
