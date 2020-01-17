using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolArray : Void
    {
        public new Array value;

        public wolArray() : base()
        {
            strtype = "Array";
        }

        public wolArray(Array val) : this()
        {
            value = val;
        }

        public override string ToString()
        {
            StringBuilder retVal = new StringBuilder("[");
            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0) retVal.Append(", "); 
                try { retVal.Append(((Value)value.GetValue(i)).type.ToString()); }
                catch { retVal.Append("null");  }
            }
            retVal.Append("]");
            return retVal.ToString();
        }
    }
}
