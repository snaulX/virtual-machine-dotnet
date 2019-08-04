using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wolvm
{
    public class wolChar : Void
    {
        public wolChar() : base()
        {
            strtype = "char";
            Enumerable.Range(Char.MinValue, Char.MaxValue).ToDictionary(x => x); //generate constants of wolChar
        }
    }
}
