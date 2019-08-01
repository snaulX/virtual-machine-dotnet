using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolShort : wolByte
    {
        public new short value;

        public wolShort() : base()
        {
            strtype = "short";
        }

        public wolShort(short val) : this() => value = val;
    }
}
