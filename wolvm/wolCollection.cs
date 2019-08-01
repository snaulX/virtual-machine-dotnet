using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolCollection : Void
    {
        public wolClass generic_type;

        public wolCollection(wolClass type) : this()
        {
            generic_type = type;
        }

        /// <summary>
        /// Constructor of Collection for overriding
        /// </summary>
        public wolCollection() : base()
        {
            classType = wolClassType.DEFAULT;
            strtype = "Collection";
            Implements();
        }
    }
}
