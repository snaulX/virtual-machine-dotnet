using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class Stack : IDisposable
    {
        public Dictionary<string, wolClass> classes;
        public Dictionary<string, wolFunction> functions;
        public Dictionary<string, Value> values;

        public Stack()
        {
            classes = new Dictionary<string, wolClass>();
            functions = new Dictionary<string, wolFunction>();
            values = new Dictionary<string, Value>();
        }

        public static Stack Parse(string stack_code)
        {
            Stack stack = new Stack();
            int position = 0;
            char current = stack_code[0];
            while (char.IsWhiteSpace(current))
            {
                position++;
                if (position > stack_code.Length)
                {
                    return stack;
                }
                current = stack_code[position];
            }
            try
            {
                current = stack_code[++position];
            }
            catch (IndexOutOfRangeException)
            {
                return stack;
            }
            StringBuilder buffer = new StringBuilder();
            start: while (current != '}')
            {
                if (buffer.ToString() == "class")
                {
                    buffer.Clear();
                    while (char.IsWhiteSpace(current))
                    {
                        position++;
                        if (position > stack_code.Length)
                        {
                            VirtualMachine.ThrowVMException("Classes is empty", VirtualMachine.position - position, ExceptionType.BLDSyntaxException);

                        }
                        current = stack_code[position];
                    }
                    try
                    {
                        current = stack_code[++position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        VirtualMachine.ThrowVMException("Classes is empty", VirtualMachine.position, ExceptionType.BLDSyntaxException);

                    }
                    if (current == '{')
                    {
                        while (current != '}')
                        {
                            while (char.IsWhiteSpace(current)) //skip whitespaces
                            {
                                try
                                {
                                    current = stack_code[++position];
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);

                                }
                            }
                            while (!char.IsWhiteSpace(current)) //get class name
                            {
                                try
                                {
                                    buffer.Append(current);
                                    current = stack_code[++position];
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);

                                }
                            }
                            string className = buffer.ToString();
                            current = stack_code[++position]; //skip whitespace
                            if ((current == '=') || (current == ':'))
                            {
                                current = stack_code[++position]; //skip whitespace
                                while (!char.IsWhiteSpace(current)) //get class type
                                {
                                    try
                                    {
                                        buffer.Append(current);
                                        current = stack_code[++position];
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);

                                    }
                                }
                                wolClass newWolClass = new wolClass { }; //create empty class
                                newWolClass.classType = (wolClassType)Enum.Parse(typeof(wolClassType), buffer.ToString(), true); //give type to our class from buffer without case sensetive
                                current = stack_code[++position];
                                if (current == '(')
                                {
                                    while (current != ')') //get parents
                                    {
                                        try
                                        {
                                            buffer.Append(current);
                                            current = stack_code[++position];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);

                                        }
                                    }
                                    foreach (string parent_name in buffer.ToString().Split(','))
                                    {
                                        try
                                        {
                                            newWolClass.parents.Add(parent_name, VirtualMachine.GetWolClass(parent_name)); //add values in parens to parents of our class
                                        }
                                        catch (NullReferenceException)
                                        {
                                            //roflanochka
                                        }
                                    }
                                }
                                if ((current == ':') || (current == '>')) //check start of class
                                {
                                    while (current != ';') //parse class body
                                    {
                                        while (char.IsWhiteSpace(current)) //skip whitespaces
                                        {
                                            try
                                            {
                                                current = stack_code[++position];
                                            }
                                            catch (IndexOutOfRangeException)
                                            {
                                                VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                            }
                                        }
                                        while (!char.IsWhiteSpace(current)) //get class name
                                        {
                                            try
                                            {
                                                buffer.Append(current);
                                                current = stack_code[++position];
                                            }
                                            catch (IndexOutOfRangeException)
                                            {
                                                VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                            }
                                        }
                                        switch (buffer.ToString())
                                        {
                                            case "constructor":
                                                buffer.Clear();
                                                if ((newWolClass.classType == wolClassType.DEFAULT) || newWolClass.classType == wolClassType.STRUCT)
                                                {
                                                    current = stack_code[++position];
                                                    if (current != '[') //check open bracket
                                                    {
                                                        VirtualMachine.ThrowVMException("Start of constructors not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                    }
                                                    else
                                                    {
                                                        while (current != ']')
                                                        {
                                                            constructor:
                                                            while (char.IsWhiteSpace(current)) //skip whitespaces
                                                            {
                                                                try
                                                                {
                                                                    current = stack_code[++position];
                                                                }
                                                                catch (IndexOutOfRangeException)
                                                                {
                                                                    VirtualMachine.ThrowVMException("Constructor`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                                                }
                                                            }
                                                            while (!char.IsWhiteSpace(current)) //get constructor name
                                                            {
                                                                try
                                                                {
                                                                    buffer.Append(current);
                                                                    current = stack_code[++position];
                                                                }
                                                                catch (IndexOutOfRangeException)
                                                                {
                                                                    VirtualMachine.ThrowVMException("Constructor`s name not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                                                }
                                                            }
                                                            string costr_name = buffer.ToString();
                                                            buffer.Clear();
                                                            current = stack_code[++position];
                                                            if (current != '=') //check assignment operator
                                                            {
                                                                VirtualMachine.ThrowVMException("Assigment operator isn`t right", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                                            }
                                                            else
                                                            {
                                                                wolFunction constr = wolFunction.NewDefaultConstructor(newWolClass); //create empty constructor template
                                                                position += 2; //skip whitespace
                                                                current = stack_code[position];
                                                                while (!char.IsWhiteSpace(current)) //get security modifer
                                                                {
                                                                    try
                                                                    {
                                                                        buffer.Append(current);
                                                                        current = stack_code[++position];
                                                                    }
                                                                    catch (IndexOutOfRangeException)
                                                                    {
                                                                        VirtualMachine.ThrowVMException("Constructor`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                                                    }
                                                                }
                                                                constr.security = (SecurityModifer)Enum.Parse(typeof(SecurityModifer), buffer.ToString(), true); //write this modifer to our function
                                                                current = stack_code[++position]; //skip whitespace
                                                                if (current == ':')
                                                                {
                                                                    //start parse block
                                                                }
                                                                else if (current == '(')
                                                                {
                                                                    //start parse constructor arguments
                                                                    buffer.Clear();
                                                                    current = stack_code[++position];
                                                                    while (current != ')')
                                                                    {
                                                                        buffer.Append(current);
                                                                        try
                                                                        {
                                                                            current = stack_code[++position];
                                                                        }
                                                                        catch (IndexOutOfRangeException)
                                                                        {
                                                                            VirtualMachine.ThrowVMException("End of arguments not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                        }
                                                                    }
                                                                    string[] arguments = buffer.ToString().Split(',');
                                                                    foreach (string argument in arguments)
                                                                    {
                                                                        string name = argument.Split(':')[0].Trim();
                                                                        wolClass type = VirtualMachine.GetWolClass(argument.Split(':')[1].Trim());
                                                                        constr.arguments.Add(name, type); //add argumrnt (null pointer) to constructor
                                                                    }
                                                                    //start parse block
                                                                }
                                                                else
                                                                {
                                                                    VirtualMachine.ThrowVMException("Arguments or start of constructor not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                }
                                                                current = stack_code[++position];
                                                                //parse block
                                                                while (char.IsWhiteSpace(current)) //skip whitespaces
                                                                {
                                                                    try
                                                                    {
                                                                        current = stack_code[++position];
                                                                    }
                                                                    catch (IndexOutOfRangeException)
                                                                    {
                                                                        VirtualMachine.ThrowVMException("Start of constructor block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                    }
                                                                }
                                                                if (current == '[')
                                                                {
                                                                    buffer.Clear();
                                                                    while (current != ']')
                                                                    {
                                                                        try
                                                                        {
                                                                            buffer.Append(current);
                                                                            current = stack_code[++position];
                                                                        }
                                                                        catch (IndexOutOfRangeException)
                                                                        {
                                                                            VirtualMachine.ThrowVMException("End of block of constructor not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                        }
                                                                    }
                                                                    constr.body = buffer.ToString();
                                                                    if (stack_code[position + 1] == ',') goto constructor;
                                                                    else newWolClass.constructors.Add(costr_name, constr);
                                                                }
                                                                else
                                                                {
                                                                    VirtualMachine.ThrowVMException("Start of block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    VirtualMachine.ThrowVMException(newWolClass.classType.ToString().ToLower() + " don`t support constructors", VirtualMachine.position, ExceptionType.TypeNotSupportedException);
                                                }
                                                break;
                                            case "func":
                                                if (newWolClass.classType == wolClassType.ENUM)
                                                {
                                                    VirtualMachine.ThrowVMException("Enum don`t support methods", VirtualMachine.position, ExceptionType.TypeNotSupportedException);
                                                }
                                                else
                                                {
                                                    buffer.Clear();
                                                    current = stack_code[++position];
                                                    if (current != '[') //check open bracket
                                                    {
                                                        VirtualMachine.ThrowVMException("Start of functions not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                    }
                                                    else
                                                    {
                                                        while (current != ']')
                                                        {
                                                            function:
                                                            while (char.IsWhiteSpace(current)) //skip whitespaces
                                                            {
                                                                try
                                                                {
                                                                    current = stack_code[++position];
                                                                }
                                                                catch (IndexOutOfRangeException)
                                                                {
                                                                    VirtualMachine.ThrowVMException("Function`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                                                }
                                                            }
                                                            while (!char.IsWhiteSpace(current)) //get function name
                                                            {
                                                                try
                                                                {
                                                                    buffer.Append(current);
                                                                    current = stack_code[++position];
                                                                }
                                                                catch (IndexOutOfRangeException)
                                                                {
                                                                    VirtualMachine.ThrowVMException("Function`s name not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                                                }
                                                            }
                                                            string func_name = buffer.ToString();
                                                            buffer.Clear();
                                                            current = stack_code[++position];
                                                            if (current != '=') //check assignment operator
                                                            {
                                                                VirtualMachine.ThrowVMException("Assigment operator isn`t right", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                                            }
                                                            else
                                                            {

                                                                wolFunction func = new wolFunction(); //create empty function
                                                                position += 2; //skip whitespace
                                                                current = stack_code[position];
                                                                while (!char.IsWhiteSpace(current)) //get security modifer
                                                                {
                                                                    try
                                                                    {
                                                                        buffer.Append(current);
                                                                        current = stack_code[++position];
                                                                    }
                                                                    catch (IndexOutOfRangeException)
                                                                    {
                                                                        VirtualMachine.ThrowVMException("Function`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                                                    }
                                                                }
                                                                func.security = (SecurityModifer)Enum.Parse(typeof(SecurityModifer), buffer.ToString(), true); //write this modifer to our function
                                                                current = stack_code[++position]; //skip whitespace
                                                                buffer.Clear();
                                                                while (!char.IsWhiteSpace(current)) //get return type
                                                                {
                                                                    try
                                                                    {
                                                                        buffer.Append(current);
                                                                        current = stack_code[++position];
                                                                    }
                                                                    catch (IndexOutOfRangeException)
                                                                    {
                                                                        VirtualMachine.ThrowVMException("Function`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                                                    }
                                                                }
                                                                try
                                                                {
                                                                    func.returnType = VirtualMachine.GetWolClass(buffer.ToString());
                                                                }
                                                                catch (NullReferenceException)
                                                                {
                                                                    //VirtualMachine.ThrowVMException("")
                                                                    //don`t need now throw vm exception becouse in GetWolClass was throw exception
                                                                }
                                                                if (current == ':')
                                                                {
                                                                    //start parse block
                                                                }
                                                                else if (current == '(')
                                                                {
                                                                    //start parse function arguments
                                                                    buffer.Clear();
                                                                    current = stack_code[++position];
                                                                    while (current != ')')
                                                                    {
                                                                        buffer.Append(current);
                                                                        try
                                                                        {
                                                                            current = stack_code[++position];
                                                                        }
                                                                        catch (IndexOutOfRangeException)
                                                                        {
                                                                            VirtualMachine.ThrowVMException("End of arguments not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                        }
                                                                    }
                                                                    string[] arguments = buffer.ToString().Split(',');
                                                                    foreach (string argument in arguments)
                                                                    {
                                                                        string name = argument.Split(':')[0].Trim();
                                                                        wolClass type = VirtualMachine.GetWolClass(argument.Split(':')[1].Trim());
                                                                        func.arguments.Add(name, type); //add argumrnt (null pointer) to function
                                                                    }
                                                                    //start parse block
                                                                }
                                                                else
                                                                {
                                                                    VirtualMachine.ThrowVMException("Arguments or start of function not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                }
                                                                current = stack_code[++position];
                                                                //parse block
                                                                while (char.IsWhiteSpace(current)) //skip whitespaces
                                                                {
                                                                    try
                                                                    {
                                                                        current = stack_code[++position];
                                                                    }
                                                                    catch (IndexOutOfRangeException)
                                                                    {
                                                                        VirtualMachine.ThrowVMException("Start of function block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                    }
                                                                }
                                                                if (current == '[')
                                                                {
                                                                    buffer.Clear();
                                                                    while (current != ']')
                                                                    {
                                                                        try
                                                                        {
                                                                            buffer.Append(current);
                                                                            current = stack_code[++position];
                                                                        }
                                                                        catch (IndexOutOfRangeException)
                                                                        {
                                                                            VirtualMachine.ThrowVMException("End of block of function not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                        }
                                                                    }
                                                                    func.body = buffer.ToString();
                                                                    if (stack_code[position + 1] == ',') goto function;
                                                                    else newWolClass.methods.Add(func_name, func);
                                                                }
                                                                else
                                                                {
                                                                    VirtualMachine.ThrowVMException("Start of block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            case "var":
                                                if (newWolClass.classType == wolClassType.ENUM)
                                                {
                                                    VirtualMachine.ThrowVMException("Enum don`t support variables", VirtualMachine.position, ExceptionType.TypeNotSupportedException);
                                                }
                                                current = stack_code[++position];
                                                if (current == '[')
                                                {
                                                    while (current != ']')
                                                    {
                                                        variable:
                                                        while (char.IsWhiteSpace(current)) //skip whitespaces
                                                        {
                                                            try
                                                            {
                                                                current = stack_code[++position];
                                                            }
                                                            catch (IndexOutOfRangeException)
                                                            {
                                                                VirtualMachine.ThrowVMException("Variable`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                                            }
                                                        }
                                                        while (!char.IsWhiteSpace(current)) //get variable name
                                                        {
                                                            try
                                                            {
                                                                buffer.Append(current);
                                                                current = stack_code[++position];
                                                            }
                                                            catch (IndexOutOfRangeException)
                                                            {
                                                                VirtualMachine.ThrowVMException("Variable`s name not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                                            }
                                                        }
                                                        string var_name = buffer.ToString();
                                                        buffer.Clear();
                                                        current = stack_code[++position];
                                                        if (current != '=') //check assignment operator
                                                        {
                                                            VirtualMachine.ThrowVMException("Assigment operator isn`t right", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                                        }
                                                        else
                                                        {

                                                            Value thisVar = new Value(VirtualMachine.Void.Value); //create empty value with parent 'void'
                                                            position += 2;
                                                            current = stack_code[position]; //skip whitespace
                                                            buffer.Clear();
                                                            while (!char.IsWhiteSpace(current)) //get type
                                                            {
                                                                try
                                                                {
                                                                    buffer.Append(current);
                                                                    current = stack_code[++position];
                                                                }
                                                                catch (IndexOutOfRangeException)
                                                                {
                                                                    VirtualMachine.ThrowVMException("Variable`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                }
                                                            }
                                                            try
                                                            {
                                                                thisVar.type = VirtualMachine.GetWolClass(buffer.ToString());
                                                            }
                                                            catch (NullReferenceException)
                                                            {
                                                                //VirtualMachine.ThrowVMException("")
                                                                //don`t need now throw vm exception becouse in GetWolClass was throw exception
                                                            }
                                                            current = stack_code[++position];
                                                            while (!char.IsWhiteSpace(current)) //get security modifer
                                                            {
                                                                try
                                                                {
                                                                    buffer.Append(current);
                                                                    current = stack_code[++position];
                                                                }
                                                                catch (IndexOutOfRangeException)
                                                                {
                                                                    VirtualMachine.ThrowVMException("Variable`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                                                }
                                                            }
                                                            SecurityModifer security = (SecurityModifer)Enum.Parse(typeof(SecurityModifer), buffer.ToString(), true); //write this modifer to our variable
                                                            current = stack_code[++position]; //skip whitespace
                                                            buffer.Clear();
                                                            while (!char.IsWhiteSpace(current)) //get name - 'set' or 'get'
                                                            {
                                                                try
                                                                {
                                                                    buffer.Append(current);
                                                                    current = stack_code[++position];
                                                                }
                                                                catch (IndexOutOfRangeException)
                                                                {
                                                                    VirtualMachine.ThrowVMException("Variable`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                }
                                                            }
                                                            if (buffer.ToString() == "set")
                                                            {
                                                                thisVar.setter.security = security;
                                                                current = stack_code[++position];
                                                                if (current == '(')
                                                                {
                                                                    //start parse setter arguments
                                                                    buffer.Clear();
                                                                    current = stack_code[++position];
                                                                    while (current != ')')
                                                                    {
                                                                        buffer.Append(current);
                                                                        try
                                                                        {
                                                                            current = stack_code[++position];
                                                                        }
                                                                        catch (IndexOutOfRangeException)
                                                                        {
                                                                            VirtualMachine.ThrowVMException("End of arguments not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                        }
                                                                    }
                                                                    string argument = buffer.ToString();
                                                                    string name = argument.Split(':')[0].Trim();
                                                                    wolClass type = VirtualMachine.GetWolClass(argument.Split(':')[1].Trim());
                                                                    thisVar.setter.arguments.Add(name, type); //add argumrnt (null pointer) to setter
                                                                    //start parse block
                                                                }
                                                                else if (current == ':')
                                                                {
                                                                    //start parse block
                                                                }
                                                                else
                                                                {
                                                                    VirtualMachine.ThrowVMException("Arguments or start of setter not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                }
                                                                current = stack_code[++position];
                                                                //parse block
                                                                while (char.IsWhiteSpace(current)) //skip whitespaces
                                                                {
                                                                    try
                                                                    {
                                                                        current = stack_code[++position];
                                                                    }
                                                                    catch (IndexOutOfRangeException)
                                                                    {
                                                                        VirtualMachine.ThrowVMException("Start of setter block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                    }
                                                                }
                                                                if (current == '[')
                                                                {
                                                                    buffer.Clear();
                                                                    while (current != ']')
                                                                    {
                                                                        try
                                                                        {
                                                                            buffer.Append(current);
                                                                            current = stack_code[++position];
                                                                        }
                                                                        catch (IndexOutOfRangeException)
                                                                        {
                                                                            VirtualMachine.ThrowVMException("End of block of variables not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                        }
                                                                    }
                                                                    thisVar.setter.body = buffer.ToString();
                                                                }
                                                                else
                                                                {
                                                                    VirtualMachine.ThrowVMException("Start of block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                }
                                                                while (char.IsWhiteSpace(current))
                                                                {
                                                                    try
                                                                    {
                                                                        current = stack_code[++position];
                                                                    }
                                                                    catch (IndexOutOfRangeException)
                                                                    {
                                                                        VirtualMachine.ThrowVMException("Getter not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                    }
                                                                }
                                                            }
                                                            else if (buffer.ToString() != "get")
                                                            {
                                                                VirtualMachine.ThrowVMException("Unknown keyword " + buffer.ToString(), VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                            }
                                                            current = stack_code[++position];
                                                            //parse block
                                                            while (char.IsWhiteSpace(current)) //skip whitespaces
                                                            {
                                                                try
                                                                {
                                                                    current = stack_code[++position];
                                                                }
                                                                catch (IndexOutOfRangeException)
                                                                {
                                                                    VirtualMachine.ThrowVMException("Start of getter block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                }
                                                            }
                                                            if (current == '[')
                                                            {
                                                                buffer.Clear();
                                                                while (current != ']')
                                                                {
                                                                    try
                                                                    {
                                                                        buffer.Append(current);
                                                                        current = stack_code[++position];
                                                                    }
                                                                    catch (IndexOutOfRangeException)
                                                                    {
                                                                        VirtualMachine.ThrowVMException("End of block of variables not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                    }
                                                                }
                                                                thisVar.getter.body = buffer.ToString();
                                                            }
                                                            else
                                                            {
                                                                VirtualMachine.ThrowVMException("Start of block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                            }
                                                            if (stack_code[position + 1] == ',') goto variable;
                                                            else newWolClass.fields.Add(var_name, thisVar);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    VirtualMachine.ThrowVMException("Start of variables not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                }
                                                break;
                                            case "const":
                                                if ((newWolClass.classType == wolClassType.ENUM) || (newWolClass.classType == wolClassType.STRUCT))
                                                {
                                                    //valid
                                                }
                                                else
                                                {
                                                    VirtualMachine.ThrowVMException(newWolClass.classType.ToString().ToLower() + " don`t support constants", VirtualMachine.position, ExceptionType.TypeNotSupportedException);
                                                }
                                                break;
                                            default:
                                                VirtualMachine.ThrowVMException("Unknown keyword " + buffer.ToString() + " in the class initilization", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    VirtualMachine.ThrowVMException("Start of block operator is not valid", VirtualMachine.position, ExceptionType.BLDSyntaxException);

                                }
                            }
                            else
                            {
                                VirtualMachine.ThrowVMException("Equals operator is not valid", VirtualMachine.position, ExceptionType.BLDSyntaxException);

                            }

                        }
                    }
                    else
                    {
                        VirtualMachine.ThrowVMException("Classes`s start not found", VirtualMachine.position - position, ExceptionType.BLDSyntaxException);

                    }
                }
                else if (buffer.ToString() == "func")
                {
                    buffer.Clear();
                    while (char.IsWhiteSpace(current)) //skip whitespaces
                    {
                        try
                        {
                            current = stack_code[++position];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            VirtualMachine.ThrowVMException("Functions is empty", VirtualMachine.position - position, ExceptionType.BLDSyntaxException);
                        }
                    }
                    if (current == '{')
                    {
                        while (current != '}')
                        {
                            function:
                            while (char.IsWhiteSpace(current)) //skip whitespaces
                            {
                                try
                                {
                                    current = stack_code[++position];
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    VirtualMachine.ThrowVMException("Function`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                }
                            }
                            while (!char.IsWhiteSpace(current)) //get function name
                            {
                                try
                                {
                                    buffer.Append(current);
                                    current = stack_code[++position];
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    VirtualMachine.ThrowVMException("Function`s name not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                }
                            }
                            string func_name = buffer.ToString();
                            buffer.Clear();
                            current = stack_code[++position];
                            if (current != '=') //check assignment operator
                            {
                                VirtualMachine.ThrowVMException("Assigment operator isn`t right", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                            }
                            else
                            {

                                wolFunction func = new wolFunction(); //create empty function
                                position += 2; //skip whitespace
                                current = stack_code[position];
                                while (!char.IsWhiteSpace(current)) //get security modifer
                                {
                                    try
                                    {
                                        buffer.Append(current);
                                        current = stack_code[++position];
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        VirtualMachine.ThrowVMException("Function`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                    }
                                }
                                func.security = (SecurityModifer)Enum.Parse(typeof(SecurityModifer), buffer.ToString(), true); //write this modifer to our function
                                current = stack_code[++position]; //skip whitespace
                                buffer.Clear();
                                while (!char.IsWhiteSpace(current)) //get return type
                                {
                                    try
                                    {
                                        buffer.Append(current);
                                        current = stack_code[++position];
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        VirtualMachine.ThrowVMException("Function`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                    }
                                }
                                try
                                {
                                    func.returnType = VirtualMachine.GetWolClass(buffer.ToString());
                                }
                                catch (NullReferenceException)
                                {
                                    //VirtualMachine.ThrowVMException("")
                                    //don`t need now throw vm exception becouse in GetWolClass was throw exception
                                }
                                if (current == ':')
                                {
                                    //start parse block
                                }
                                else if (current == '(')
                                {
                                    //start parse function arguments
                                    buffer.Clear();
                                    current = stack_code[++position];
                                    while (current != ')')
                                    {
                                        buffer.Append(current);
                                        try
                                        {
                                            current = stack_code[++position];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            VirtualMachine.ThrowVMException("End of arguments not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                        }
                                    }
                                    string[] arguments = buffer.ToString().Split(',');
                                    foreach (string argument in arguments)
                                    {
                                        string name = argument.Split(':')[0].Trim();
                                        wolClass type = VirtualMachine.GetWolClass(argument.Split(':')[1].Trim());
                                        func.arguments.Add(name, type); //add argumrnt (null pointer) to function
                                    }
                                    //start parse block
                                }
                                else
                                {
                                    VirtualMachine.ThrowVMException("Arguments or start of function not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                }
                                current = stack_code[++position];
                                //parse block
                                while (char.IsWhiteSpace(current)) //skip whitespaces
                                {
                                    try
                                    {
                                        current = stack_code[++position];
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        VirtualMachine.ThrowVMException("Start of function block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                    }
                                }
                                if (current == '[')
                                {
                                    buffer.Clear();
                                    while (current != ']')
                                    {
                                        try
                                        {
                                            buffer.Append(current);
                                            current = stack_code[++position];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            VirtualMachine.ThrowVMException("End of block of function not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                        }
                                    }
                                    func.body = buffer.ToString();
                                    if (stack_code[position + 1] == ',') goto function;
                                    else VirtualMachine.mainstack.functions.Add(func_name, func);
                                }
                                else
                                {
                                    VirtualMachine.ThrowVMException("Start of block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                }
                            }
                        }
                    }
                    else
                    {
                        VirtualMachine.ThrowVMException("Functions`s start not found", VirtualMachine.position - position, ExceptionType.BLDSyntaxException);

                    }
                }
                else if (buffer.ToString() == "var")
                {
                    buffer.Clear();
                    while (char.IsWhiteSpace(current)) //skip whitespaces
                    {
                        try
                        {
                            current = stack_code[++position];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            VirtualMachine.ThrowVMException("Variables is empty", VirtualMachine.position - position, ExceptionType.BLDSyntaxException);
                        }
                    }
                    if (current == '{')
                    {
                        while (current != '}')
                        {
                            variable:
                            while (char.IsWhiteSpace(current)) //skip whitespaces
                            {
                                try
                                {
                                    current = stack_code[++position];
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    VirtualMachine.ThrowVMException("Variable`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                }
                            }
                            while (!char.IsWhiteSpace(current)) //get variable name
                            {
                                try
                                {
                                    buffer.Append(current);
                                    current = stack_code[++position];
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    VirtualMachine.ThrowVMException("Variable`s name not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                }
                            }
                            string var_name = buffer.ToString();
                            buffer.Clear();
                            current = stack_code[++position];
                            if (current != '=') //check assignment operator
                            {
                                VirtualMachine.ThrowVMException("Assigment operator isn`t right", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                            }
                            else
                            {

                                Value thisVar = new Value(VirtualMachine.Void.Value); //create empty value with parent 'void'
                                position += 2;
                                current = stack_code[position]; //skip whitespace
                                buffer.Clear();
                                while (!char.IsWhiteSpace(current)) //get type
                                {
                                    try
                                    {
                                        buffer.Append(current);
                                        current = stack_code[++position];
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        VirtualMachine.ThrowVMException("Variable`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                    }
                                }
                                try
                                {
                                    thisVar.type = VirtualMachine.GetWolClass(buffer.ToString());
                                }
                                catch (NullReferenceException)
                                {
                                    //VirtualMachine.ThrowVMException("")
                                    //don`t need now throw vm exception becouse in GetWolClass was throw exception
                                }
                                current = stack_code[++position];
                                while (!char.IsWhiteSpace(current)) //get security modifer
                                {
                                    try
                                    {
                                        buffer.Append(current);
                                        current = stack_code[++position];
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        VirtualMachine.ThrowVMException("Variable`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                    }
                                }
                                SecurityModifer security = (SecurityModifer)Enum.Parse(typeof(SecurityModifer), buffer.ToString(), true); //write this modifer to our variable
                                current = stack_code[++position]; //skip whitespace
                                buffer.Clear();
                                while (!char.IsWhiteSpace(current)) //get name - 'set' or 'get'
                                {
                                    try
                                    {
                                        buffer.Append(current);
                                        current = stack_code[++position];
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        VirtualMachine.ThrowVMException("Variable`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                    }
                                }
                                if (buffer.ToString() == "set")
                                {
                                    thisVar.setter.security = security;
                                    current = stack_code[++position];
                                    if (current == '(')
                                    {
                                        //start parse setter arguments
                                        buffer.Clear();
                                        current = stack_code[++position];
                                        while (current != ')')
                                        {
                                            buffer.Append(current);
                                            try
                                            {
                                                current = stack_code[++position];
                                            }
                                            catch (IndexOutOfRangeException)
                                            {
                                                VirtualMachine.ThrowVMException("End of arguments not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                            }
                                        }
                                        string argument = buffer.ToString();
                                        string name = argument.Split(':')[0].Trim();
                                        wolClass type = VirtualMachine.GetWolClass(argument.Split(':')[1].Trim());
                                        thisVar.setter.arguments.Add(name, type); //add argumrnt (null pointer) to setter
                                        //start parse block
                                    }
                                    else if (current == ':')
                                    {
                                        //start parse block
                                    }
                                    else
                                    {
                                        VirtualMachine.ThrowVMException("Arguments or start of setter not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                    }
                                    current = stack_code[++position];
                                    //parse block
                                    while (char.IsWhiteSpace(current)) //skip whitespaces
                                    {
                                        try
                                        {
                                            current = stack_code[++position];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            VirtualMachine.ThrowVMException("Start of setter block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                        }
                                    }
                                    if (current == '[')
                                    {
                                        buffer.Clear();
                                        while (current != ']')
                                        {
                                            try
                                            {
                                                buffer.Append(current);
                                                current = stack_code[++position];
                                            }
                                            catch (IndexOutOfRangeException)
                                            {
                                                VirtualMachine.ThrowVMException("End of block of variables not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                            }
                                        }
                                        thisVar.setter.body = buffer.ToString();
                                    }
                                    else
                                    {
                                        VirtualMachine.ThrowVMException("Start of block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                    }
                                    while (char.IsWhiteSpace(current))
                                    {
                                        try
                                        {
                                            current = stack_code[++position];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            VirtualMachine.ThrowVMException("Getter not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                        }
                                    }
                                }
                                else if (buffer.ToString() != "get")
                                {
                                    VirtualMachine.ThrowVMException("Unknown keyword " + buffer.ToString(), VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                }
                                current = stack_code[++position];
                                //parse block
                                while (char.IsWhiteSpace(current)) //skip whitespaces
                                {
                                    try
                                    {
                                        current = stack_code[++position];
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        VirtualMachine.ThrowVMException("Start of getter block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                    }
                                }
                                if (current == '[')
                                {
                                    buffer.Clear();
                                    while (current != ']')
                                    {
                                        try
                                        {
                                            buffer.Append(current);
                                            current = stack_code[++position];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            VirtualMachine.ThrowVMException("End of block of variables not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                        }
                                    }
                                    thisVar.getter.body = buffer.ToString();
                                }
                                else
                                {
                                    VirtualMachine.ThrowVMException("Start of block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                }
                                if (stack_code[position + 1] == ',') goto variable;
                                else VirtualMachine.mainstack.values.Add(var_name, thisVar);
                            }
                        }
                    }
                    else
                    {
                        VirtualMachine.ThrowVMException("Variables`s start not found", VirtualMachine.position - position, ExceptionType.BLDSyntaxException);

                    }
                }
                try
                {
                    buffer.Append(current);
                    current = stack_code[++position];
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
            }
            try
            {
                if (stack_code[++position] == ';')
                {
                    buffer.Append(current);
                    current = stack_code[position];
                    goto start;
                }
            }
            catch (IndexOutOfRangeException)
            {
                //pass
            }
            return stack;
        }

        public void Add(Stack stack)
        {
            if (stack.classes.Count != 0)
            {
                foreach (KeyValuePair<string, wolClass> cl in stack.classes)
                {
                    classes.Add(cl.Key, cl.Value);
                }
            }
            if (stack.functions.Count != 0)
            {
                foreach (KeyValuePair<string, wolFunction> fn in stack.functions)
                {
                    functions.Add(fn.Key, fn.Value);
                }
            }
            if (stack.values.Count != 0)
            {
                foreach (KeyValuePair<string, Value> vl in stack.values)
                {
                    values.Add(vl.Key, vl.Value);
                }
            }
        }

        public void Dispose()
        {
            classes.Clear();
            functions.Clear();
            values.Clear();
        }

        public override string ToString()
        {
            string classes_str = "", func_str = "", var_str = "";
            if (classes.Count != 0)
            {
                classes_str += "class {\n\t";
                classes_str += "}";
            }
            if (functions.Count != 0)
            {
                func_str += "func {\n";
            }
            if (values.Count != 0)
            {
                var_str += "var {\n";
            }
            return " {\n\t" + classes_str + func_str + var_str + "\n};";
        }

        public static Stack operator +(Stack right, Stack left)
        {
            Stack ret = new Stack();
            ret.Add(right);
            ret.Add(left);
            return ret;
        }
    }
}
