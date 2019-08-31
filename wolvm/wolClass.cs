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
        public string strtype;
        
        //for overriding
        public wolClass()
        {
            //pass
        }

        public wolClass(string name, SecurityModifer securityModifer = SecurityModifer.PUBLIC, wolClassType type = wolClassType.DEFAULT, string ConstructorName = "init")
        {
            strtype = name;
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
                        { "void", VirtualMachine.Void }
                    };
                    break;
                case wolClassType.ENUM:
                    constants = new Dictionary<string, Value>();
                    parents = new Dictionary<string, wolClass>
                    {
                        { "int", VirtualMachine.wolInt }
                    };
                    break;
                case wolClassType.STATIC:
                    parents = new Dictionary<string, wolClass>
                    {
                        { "void", VirtualMachine.Void }
                    };
                    fields = new Dictionary<string, Value>();
                    methods = new Dictionary<string, wolFunction>();
                    break;
                case wolClassType.STRUCT:
                    constants = new Dictionary<string, Value>();
                    parents = new Dictionary<string, wolClass>
                    {
                        { "void", VirtualMachine.Void }
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
                        { "void", VirtualMachine.Void }
                    };
                    fields = new Dictionary<string, Value>();
                    methods = new Dictionary<string, wolFunction>();
                    break;
            }
        }

        public override string ToString() => strtype;

        /// <summary>
        /// Implement all parents
        /// </summary>
        public void Implements()
        {
            foreach (wolClass parent in parents.Values)
            {
                if (classType != wolClassType.ENUM)
                {
                    foreach (KeyValuePair<string, wolFunction> method in parent.methods)
                    {
                        if (!method.Value.close)
                        {
                            try
                            {
                                methods.Add(method.Key, method.Value);
                            }
                            catch (ArgumentException)
                            {
                                continue;
                            }
                        }
                    }
                    foreach (KeyValuePair<string, Value> field in parent.fields)
                    {
                        try
                        {
                            fields.Add(field.Key, field.Value);
                        }
                        catch (ArgumentException)
                        {
                            continue;
                        }
                    }
                }
                if ((classType == wolClassType.DEFAULT) || (classType == wolClassType.STRUCT))
                {
                    foreach (KeyValuePair<string, wolFunction> constructor in parent.constructors)
                    {
                        if (!constructor.Value.close)
                        {
                            try
                            {
                                constructors.Add(constructor.Key, constructor.Value);
                            }
                            catch (ArgumentException)
                            {
                                continue;
                            }
                        }
                    }
                    foreach (wolFunction destructor in parent.destructors)
                    {
                        if (!destructor.close)
                        {
                            try
                            {
                                destructors.Add(destructor);
                            }
                            catch (ArgumentException)
                            {
                                continue;
                            }
                        }
                    }
                }
                if ((classType == wolClassType.ENUM) || (classType == wolClassType.STRUCT))
                {
                    foreach (KeyValuePair<string, Value> constant in parent.constants)
                    {
                        try
                        {
                            constants.Add(constant.Key, constant.Value);
                        }
                        catch (ArgumentException)
                        {
                            continue;
                        }
                    }
                }
            }
        }

        public wolClass ToParentClass(string parent_name)
        {
            wolClass parent = null;
            try
            {
                parent = parents[parent_name];
            }
            catch (KeyNotFoundException)
            {
                VirtualMachine.ThrowVMException($"Parent by name '{parent_name}' not found of '{strtype}'", VirtualMachine.position, ExceptionType.NotFoundException);
            }
            if (parent.classType != wolClassType.ENUM)
            {
                foreach (KeyValuePair<string, wolFunction> method in methods)
                {
                    if (!method.Value.close)
                    {
                        try
                        {
                            parent.methods.Add(method.Key, method.Value);
                        }
                        catch (ArgumentException)
                        {
                            continue;
                        }
                    }
                }
                foreach (KeyValuePair<string, Value> field in fields)
                {
                    try
                    {
                        parent.fields.Add(field.Key, field.Value);
                    }
                    catch (ArgumentException)
                    {
                        continue;
                    }
                }
            }
            if ((parent.classType == wolClassType.DEFAULT) || (parent.classType == wolClassType.STRUCT))
            {
                foreach (KeyValuePair<string, wolFunction> constructor in constructors)
                {
                    if (!constructor.Value.close)
                    {
                        try
                        {
                            parent.constructors.Add(constructor.Key, constructor.Value);
                        }
                        catch (ArgumentException)
                        {
                            continue;
                        }
                    }
                }
                foreach (wolFunction destructor in destructors)
                {
                    if (!destructor.close)
                    {
                        try
                        {
                            parent.destructors.Add(destructor);
                        }
                        catch (ArgumentException)
                        {
                            continue;
                        }
                    }
                }
            }
            if ((parent.classType == wolClassType.ENUM) || (parent.classType == wolClassType.STRUCT))
            {
                foreach (KeyValuePair<string, Value> constant in parent.constants)
                {
                    try
                    {
                        parent.constants.Add(constant.Key, constant.Value);
                    }
                    catch (ArgumentException)
                    {
                        continue;
                    }
                }
            }
            return parent;
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
