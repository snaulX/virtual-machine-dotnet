using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolClass
    {
        public Dictionary<string, wolFunction> methods, constructors;
        public List<wolFunction> destructors;
        public Dictionary<string, Value> fields, constants;
        public Dictionary<string, wolClass> parents;
        public SecurityModifer security;
        public wolClassType classType;
        
        //for overriding
        public wolClass()
        {
            //pass
        }

        public wolClass(SecurityModifer securityModifer = SecurityModifer.PRIVATE, wolClassType type = wolClassType.DEFAULT, string ConstructorName = "init")
        {
            security = securityModifer;
            classType = type;
            switch (classType)
            {
                case wolClassType.DEFAULT:
                    methods = new Dictionary<string, wolFunction>();
                    constructors = new Dictionary<string, wolFunction>
                    {
                        { ConstructorName, new wolFunction() }
                    };
                    destructors = new List<wolFunction>
                    {
                        new wolFunction()
                    };
                    fields = new Dictionary<string, Value>();
                    parents = new Dictionary<string, wolClass>
                    {
                        { VirtualMachine.Void.Key, VirtualMachine.Void.Value }
                    };
                    break;
                case wolClassType.ENUM:
                    constants = new Dictionary<string, Value>();
                    parents = new Dictionary<string, wolClass>
                    {
                        { VirtualMachine.wolInt.Key, VirtualMachine.wolInt.Value }
                    };
                    break;
                case wolClassType.STATIC:
                    parents = new Dictionary<string, wolClass>
                    {
                        { VirtualMachine.Void.Key, VirtualMachine.Void.Value }
                    };
                    fields = new Dictionary<string, Value>();
                    methods = new Dictionary<string, wolFunction>();
                    break;
                case wolClassType.STRUCT:
                    constants = new Dictionary<string, Value>();
                    parents = new Dictionary<string, wolClass>
                    {
                        { VirtualMachine.Void.Key, VirtualMachine.Void.Value }
                    };
                    fields = new Dictionary<string, Value>();
                    methods = new Dictionary<string, wolFunction>();
                    constructors = new Dictionary<string, wolFunction>
                    {
                        { ConstructorName, new wolFunction() }
                    };
                    destructors = new List<wolFunction>
                    {
                        new wolFunction()
                    };
                    break;
                case wolClassType.ABSTRACT:
                    parents = new Dictionary<string, wolClass>
                    {
                        { VirtualMachine.Void.Key, VirtualMachine.Void.Value }
                    };
                    fields = new Dictionary<string, Value>();
                    methods = new Dictionary<string, wolFunction>();
                    break;
            }
        }

        /// <summary>
        /// Create World of Legends class how collection
        /// </summary>
        /// <param name="securityModifer"></param>
        /// <param name="ConstructorName"></param>
        /// <returns></returns>
        public static wolClass CreateCollection(SecurityModifer securityModifer = SecurityModifer.PRIVATE, string ConstructorName = "Collection")
        {
            return new wolClass
            {
                security = securityModifer,
                classType = wolClassType.DEFAULT,
                methods = new Dictionary<string, wolFunction>(),
                constructors = new Dictionary<string, wolFunction>
                {
                    { ConstructorName, new wolFunction() }
                },
                destructors = new List<wolFunction>
                {
                    new wolFunction()
                },
                fields = new Dictionary<string, Value>(),
                parents = new Dictionary<string, wolClass>
                {
                    { VirtualMachine.wolCollection.Key, VirtualMachine.wolCollection.Value }
                }
            };
        }
    }

    public enum wolClassType
    {
        DEFAULT,
        STATIC,
        STRUCT,
        ENUM,
        ABSTRACT
    }
}
