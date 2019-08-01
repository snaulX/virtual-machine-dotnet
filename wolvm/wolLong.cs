using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolLong : wolInt
    {
        public new long value;

        public wolLong() : base()
        {
            strtype = "long";
        }

        public wolLong(long val) : this() => value = val;
    }
}
