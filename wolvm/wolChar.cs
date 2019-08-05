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
            Enumerable.Range(Char.MinValue, Char.MaxValue); //generate constants of wolChar
            parents = new Dictionary<string, wolClass>
            {
                { "void", VirtualMachine.Void }
            };
        }
    }
}
