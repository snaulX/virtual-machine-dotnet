using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolClass
    {
        public Dictionary<string, wolFunction> methods, constructors;
        public List<wolFunction> destructors;
        public Dictionary<string, Value> fields, constants, static_fields;
        public Dictionary<string, wolClass> parents;
        public SecurityModifer security;
        public wolClassType classType;
        public string strtype;

        public wolClass()
        {
            //for overriding
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
                    static_fields = new Dictionary<string, Value>();
                    parents = new Dictionary<string, wolClass>
                    {
                        { "void", VirtualMachine.GetWolClass("void") }
                    };
                    break;
                case wolClassType.ENUM:
                    constants = new Dictionary<string, Value>();
                    parents = new Dictionary<string, wolClass>
                    {
                        { "int", VirtualMachine.GetWolClass("int") }
                    };
                    break;
                case wolClassType.STATIC:
                    parents = new Dictionary<string, wolClass>
                    {
                        { "void", VirtualMachine.GetWolClass("void") }
                    };
                    static_fields = new Dictionary<string, Value>();
                    methods = new Dictionary<string, wolFunction>();
                    break;
                case wolClassType.STRUCT:
                    constants = new Dictionary<string, Value>();
                    parents = new Dictionary<string, wolClass>
                    {
                        { "void", VirtualMachine.GetWolClass("void") }
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
                        { "void", VirtualMachine.GetWolClass("void") }
                    };
                    fields = new Dictionary<string, Value>();
                    static_fields = new Dictionary<string, Value>();
                    methods = new Dictionary<string, wolFunction>();
                    break;
            }
        }

        public override string ToString() => "wolvm::mainstack::" + strtype;

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
                else
                {
                    foreach (KeyValuePair<string, Value> stfield in parent.static_fields)
                    {
                        try
                        {
                            static_fields.Add(stfield.Key, stfield.Value);
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
            else
            {
                foreach (KeyValuePair<string, Value> stfield in static_fields)
                {
                    try
                    {
                        parent.static_fields.Add(stfield.Key, stfield.Value);
                    }
                    catch (ArgumentException)
                    {
                        continue;
                    }
                }
            }
            return parent;
        }

        public wolFunction GetStaticMethod(string name)
        {
            wolFunction func = null;
            try
            {
                func = methods[name];
            }
            catch (KeyNotFoundException)
            {
                VirtualMachine.ThrowVMException($"Method by name {name} not found in {strtype}", VirtualMachine.position, ExceptionType.NotFoundException);
            }
            try
            {
                if (func.arguments["this"] == this)
                {
                    VirtualMachine.ThrowVMException($"Method by name {name} in {strtype} is not static", VirtualMachine.position, ExceptionType.InitilizateException);
                }
            }
            catch (KeyNotFoundException)
            {
                //so ok
            }
            return func;
        }

        public Value GetStaticField(string name)
        {
            Value field = null;
            try
            {
                field = static_fields[name];
            }
            catch (KeyNotFoundException)
            {
                VirtualMachine.ThrowVMException($"In class '{strtype}' not found static field by name '{name}'", VirtualMachine.position, ExceptionType.NotFoundException);
            }
            return field;
        }

        public void CallDestructor(int index, params Value[] args)
        {
            try
            {
                destructors[index].Call(args);
            }
            catch (IndexOutOfRangeException)
            {
                VirtualMachine.ThrowVMException("Index in stack of destructor is bigger than count of destructors", VirtualMachine.position, ExceptionType.IndexOutOfRangeException);
            }
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
