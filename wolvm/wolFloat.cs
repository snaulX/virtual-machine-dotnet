using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolFloat : wolInt
    {
        public new float value;

        public wolFloat() : base()
        {
            strtype = "float";
        }

        public wolFloat(float val) : this() => value = val;
    }
}
