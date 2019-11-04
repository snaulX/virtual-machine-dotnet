using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolBlock : Void
    {
        public string body;

        public wolBlock() : base()
        {
            strtype = "Block";
            classType = wolClassType.DEFAULT;
        }

        public wolBlock(string _body) : this()
        {
            body = _body;
        }

        public void Run() => Script.Parse(body);

        public override string ToString()
        {
            return "wolvm::mainstack::Block:" + body + ":end";
        }
    }
}
