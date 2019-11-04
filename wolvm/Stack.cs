using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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

        /// <summary>
        /// Parsing code of stack and return parsing stack
        /// </summary>
        /// <param name="stack_code">Code of stack with open bracket '{'</param>
        /// <returns></returns>
        public static Stack Parse(string stack_code)
        {
            Stack stack = new Stack();
            int position = 0;
            char current = stack_code[0];
            while (char.IsWhiteSpace(current)) //skip whitespaces
            {
                position++;
                if (position > stack_code.Length)
                {
                    return stack;
                }
                current = stack_code[position];
            }
            if (current == '{')
            {
                try //skip open bracket '{'
                {
                    current = stack_code[++position];
                }
                catch (IndexOutOfRangeException)
                {
                    VirtualMachine.ThrowVMException("End of block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                    return stack;
                }
                StringBuilder buffer = new StringBuilder();
                start: while (current != '}')
                {
                    while (char.IsWhiteSpace(current)) //skip whitespaces
                    {
                        try
                        {
                            current = stack_code[++position];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            VirtualMachine.ThrowVMException("End of block of stack not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                        }
                    }
                    while (!char.IsWhiteSpace(current)) //get keyword
                    {
                        try
                        {
                            buffer.Append(current);
                            current = stack_code[++position];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            return stack;
                            //VirtualMachine.ThrowVMException("End of block of stack not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                        }
                    }
                    if (buffer.ToString() == "class")
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
                                VirtualMachine.ThrowVMException("Class haven`t block", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                            }
                        }
                        if (current == '{')
                        {
                            while (current != '}')
                            {
                                if (char.IsWhiteSpace(current) || current == '{')
                                    current = stack_code[++position];
                                while (char.IsWhiteSpace(current)) //skip whitespaces
                                {
                                    try
                                    {
                                        current = stack_code[++position];
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                    }
                                }
                                while (!char.IsWhiteSpace(current)) //get name of class
                                {
                                    try
                                    {
                                        buffer.Append(current);
                                        current = stack_code[++position];
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                    }
                                }
                                string className = buffer.ToString();
                                while (char.IsWhiteSpace(current)) //skip whitespaces
                                {
                                    try
                                    {
                                        current = stack_code[++position];
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);

                                    }
                                }
                                buffer.Clear();
                                if ((current == '=') || (current == ':'))
                                {
                                    while (char.IsWhiteSpace(current))
                                    {
                                        try
                                        {
                                            current = stack_code[++position];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            VirtualMachine.ThrowVMException("Body of class not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                        }
                                    }
                                    while (!char.IsWhiteSpace(current)) //get '='
                                    {
                                        try
                                        {
                                            buffer.Append(current);
                                            current = stack_code[++position];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);

                                        }
                                    }
                                    while (char.IsWhiteSpace(current))
                                    {
                                        try
                                        {
                                            current = stack_code[++position];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            VirtualMachine.ThrowVMException("Body of class not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                        }
                                    }
                                    buffer.Clear();
                                    while (!char.IsWhiteSpace(current)) //get class type
                                    {
                                        try
                                        {
                                            buffer.Append(current);
                                            current = stack_code[++position];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);

                                        }
                                    }
                                    wolClass newWolClass = new wolClass(className, SecurityModifer.PUBLIC, wolClassType.DEFAULT, "init"); //create class
                                    try
                                    {
                                        newWolClass.classType = (wolClassType)Enum.Parse(typeof(wolClassType), buffer.ToString(), true); //give type to our class from buffer without case sensetive
                                    }
                                    catch (Exception)
                                    {
                                        VirtualMachine.ThrowVMException($"{buffer.ToString()} is not class type", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                    }
                                    while (char.IsWhiteSpace(current))
                                    {
                                        try
                                        {
                                            current = stack_code[++position];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            VirtualMachine.ThrowVMException("End of class not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                        }
                                    }
                                    buffer.Clear();
                                    while (!char.IsWhiteSpace(current)) //get security type
                                    {
                                        try
                                        {
                                            buffer.Append(current);
                                            current = stack_code[++position];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                        }
                                    }
                                    try
                                    {
                                        newWolClass.security = (SecurityModifer)Enum.Parse(typeof(SecurityModifer), buffer.ToString(), true);
                                    }
                                    catch (Exception)
                                    {
                                        VirtualMachine.ThrowVMException($"{buffer.ToString()} is not security modifer", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                    }
                                    while (char.IsWhiteSpace(current))
                                    {
                                        try
                                        {
                                            current = stack_code[++position];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            VirtualMachine.ThrowVMException("End of class not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                        }
                                    }
                                    if (current == '(')
                                    {
                                        buffer.Clear();
                                        while (current != ')') //get parents
                                        {
                                            try
                                            {
                                                buffer.Append(current);
                                                current = stack_code[++position];
                                            }
                                            catch (IndexOutOfRangeException)
                                            {
                                                VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                            }
                                        }
                                        foreach (string parent_name in buffer.ToString().Remove(0, 1).Split(','))
                                        {
                                            try
                                            {
                                                newWolClass.parents.Add(parent_name, VirtualMachine.GetWolClass(parent_name)); //add values in parens to parents of our class
                                            }
                                            catch (Exception)
                                            {
                                                //roflanochka
                                            }
                                        }
                                    }
                                    current = stack_code[++position]; //skip ')'
                                    while (char.IsWhiteSpace(current))
                                    {
                                        try
                                        {
                                            current = stack_code[++position];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            VirtualMachine.ThrowVMException("End of class not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                        }
                                    }
                                    if ((current == ':') || (current == '>')) //check start of class
                                    {
                                        while (true) //parse class body
                                        {
                                            current = stack_code[++position]; //skip start of class (':' or '>')
                                            while (char.IsWhiteSpace(current)) //skip whitespaces
                                            {
                                                try
                                                {
                                                    current = stack_code[++position];
                                                }
                                                catch (IndexOutOfRangeException)
                                                {
                                                    VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                }
                                            }
                                            buffer.Clear();
                                            while (!char.IsWhiteSpace(current)) //get class keyword
                                            {
                                                try
                                                {
                                                    buffer.Append(current);
                                                    current = stack_code[++position];
                                                }
                                                catch (IndexOutOfRangeException)
                                                {
                                                    VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                                                current = stack_code[++position]; //skip open bracket
                                                                while (char.IsWhiteSpace(current)) //skip whitespaces
                                                                {
                                                                    try
                                                                    {
                                                                        current = stack_code[++position];
                                                                    }
                                                                    catch (IndexOutOfRangeException)
                                                                    {
                                                                        VirtualMachine.ThrowVMException("Constructor`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                                                        VirtualMachine.ThrowVMException("Constructor`s name not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                    }
                                                                }
                                                                string constrnanme = buffer.ToString();
                                                                buffer.Clear();
                                                                while (char.IsWhiteSpace(current)) //skip whitespaces
                                                                {
                                                                    try
                                                                    {
                                                                        current = stack_code[++position];
                                                                    }
                                                                    catch (IndexOutOfRangeException)
                                                                    {
                                                                        VirtualMachine.ThrowVMException("Constructor`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                    }
                                                                }
                                                                if (current != '=') //check assignment operator
                                                                {
                                                                    VirtualMachine.ThrowVMException($"Assigment operator isn`t right in constructor {current}", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                                                            VirtualMachine.ThrowVMException("Constructor`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                                                        current = stack_code[++position];
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
                                                                        buffer.Clear();
                                                                        newWolClass.constructors.Add(constrnanme, constr);
                                                                        if (stack_code[++position] == ',') goto constructor;
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
                                                        VirtualMachine.ThrowVMException($"{newWolClass.classType.ToString().ToLower()} don`t support constructors", VirtualMachine.position - stack_code.Length + position, ExceptionType.TypeNotSupportedException);
                                                    }
                                                    break;
                                                case "func":
                                                    buffer.Clear();
                                                    if (newWolClass.classType == wolClassType.ENUM)
                                                    {
                                                        VirtualMachine.ThrowVMException("Enum don`t support methods", VirtualMachine.position - stack_code.Length + position, ExceptionType.TypeNotSupportedException);
                                                    }
                                                    else
                                                    {
                                                        while (char.IsWhiteSpace(current)) //skip whitespaces
                                                        {
                                                            try
                                                            {
                                                                current = stack_code[++position];
                                                            }
                                                            catch (IndexOutOfRangeException)
                                                            {
                                                                VirtualMachine.ThrowVMException("Start of methods not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                            }
                                                        }
                                                        if (current != '[') //check open bracket
                                                        {
                                                            VirtualMachine.ThrowVMException("Start of methods not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                        }
                                                        else
                                                        {
                                                            current = stack_code[++position]; //skip open bracket '['
                                                            function: while (current != ']')
                                                            {
                                                                while (char.IsWhiteSpace(current)) //skip whitespaces
                                                                {
                                                                    try
                                                                    {
                                                                        current = stack_code[++position];
                                                                    }
                                                                    catch (IndexOutOfRangeException)
                                                                    {
                                                                        VirtualMachine.ThrowVMException("Method`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                    }
                                                                }
                                                                while (!char.IsWhiteSpace(current)) //get method name
                                                                {
                                                                    try
                                                                    {
                                                                        buffer.Append(current);
                                                                        current = stack_code[++position];
                                                                    }
                                                                    catch (IndexOutOfRangeException)
                                                                    {
                                                                        VirtualMachine.ThrowVMException("Method`s name not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                    }
                                                                }
                                                                string func_name = buffer.ToString();
                                                                buffer.Clear();
                                                                current = stack_code[++position];
                                                                if (current != '=') //check assignment operator
                                                                {
                                                                    VirtualMachine.ThrowVMException($"Assigment operator isn`t right ('{current}') in method", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                                                            VirtualMachine.ThrowVMException("Method`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                        }
                                                                    }
                                                                    try
                                                                    {
                                                                        func.security = (SecurityModifer)Enum.Parse(typeof(SecurityModifer), buffer.ToString(), true); //write this modifer to our function
                                                                    }
                                                                    catch (ArgumentException)
                                                                    {
                                                                        VirtualMachine.ThrowVMException($"{buffer.ToString()} is not secutiry modifer", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                    }
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
                                                                            VirtualMachine.ThrowVMException("Method`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                                                    while (char.IsWhiteSpace(current)) //skip whitespaces
                                                                    {
                                                                        try
                                                                        {
                                                                            current = stack_code[++position];
                                                                        }
                                                                        catch (IndexOutOfRangeException)
                                                                        {
                                                                            VirtualMachine.ThrowVMException("Start of arguments or method not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                        }
                                                                    }
                                                                    if (current == ':')
                                                                    {
                                                                        //goto parse_block;
                                                                    }
                                                                    else if (current == '(')
                                                                    {
                                                                        //start parse function arguments
                                                                        buffer.Clear();
                                                                        current = stack_code[++position]; //skip open paren '('
                                                                        while (current != ')')
                                                                        {
                                                                            try
                                                                            {
                                                                                buffer.Append(current);
                                                                                current = stack_code[++position];
                                                                            }
                                                                            catch (IndexOutOfRangeException)
                                                                            {
                                                                                VirtualMachine.ThrowVMException("End of arguments not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                            }
                                                                        }
                                                                        string[] arguments = buffer.ToString().Split(',');
                                                                        foreach (string argument in arguments)
                                                                            func.arguments.Add(argument.Split(':')[0].Trim(), VirtualMachine.GetWolClass(argument.Split(':')[1])); //add argumrnt to method
                                                                        //current = stack_code[++position]; //skip ')'
                                                                    }
                                                                    else
                                                                    {
                                                                        VirtualMachine.ThrowVMException($"Arguments or start of method not found ('{current}')", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                    }
                                                                    current = stack_code[++position];
                                                                    while (char.IsWhiteSpace(current)) //skip whitespaces
                                                                    {
                                                                        try
                                                                        {
                                                                            current = stack_code[++position];
                                                                        }
                                                                        catch (IndexOutOfRangeException)
                                                                        {
                                                                            VirtualMachine.ThrowVMException("Start of method block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                        }
                                                                    }
                                                                    if (current == '[')
                                                                    {
                                                                        buffer.Clear();
                                                                        current = stack_code[++position];
                                                                        while (current != ']')
                                                                        {
                                                                            try
                                                                            {
                                                                                buffer.Append(current);
                                                                                current = stack_code[++position];
                                                                            }
                                                                            catch (IndexOutOfRangeException)
                                                                            {
                                                                                VirtualMachine.ThrowVMException("End of block of method not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                            }
                                                                        }
                                                                        func.body = buffer.ToString().Trim();
                                                                        newWolClass.methods.Add(func_name, func);
                                                                    }
                                                                    else
                                                                    {
                                                                        VirtualMachine.ThrowVMException("Start of block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                    }
                                                                }
                                                            }
                                                            if (stack_code[++position] == ',')
                                                            {
                                                                current = stack_code[++position];
                                                                goto function;
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case "var":
                                                    buffer.Clear();
                                                    if (newWolClass.classType == wolClassType.ENUM)
                                                    {
                                                        VirtualMachine.ThrowVMException("Enum don`t support variables", VirtualMachine.position - stack_code.Length + position, ExceptionType.TypeNotSupportedException);
                                                    }
                                                    else
                                                    {
                                                        current = stack_code[++position]; //skip whitespace
                                                        if (current == '[')
                                                        {
                                                            while (current != ']')
                                                            {
                                                                variable:
                                                                current = stack_code[++position]; //skip '[' or other character
                                                                while (char.IsWhiteSpace(current)) //skip whitespaces
                                                                {
                                                                    try
                                                                    {
                                                                        current = stack_code[++position];
                                                                    }
                                                                    catch (IndexOutOfRangeException)
                                                                    {
                                                                        VirtualMachine.ThrowVMException("Variable`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                                                        VirtualMachine.ThrowVMException("Variable`s name not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                    }
                                                                }
                                                                string var_name = buffer.ToString();
                                                                buffer.Clear();
                                                                current = stack_code[++position];
                                                                if (current != '=') //check assignment operator
                                                                {
                                                                    VirtualMachine.ThrowVMException($"Assigment operator isn`t right in field ('{current}')", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                }
                                                                else
                                                                {

                                                                    Value thisVar = Value.VoidValue; //create empty value with parent 'void'
                                                                    position += 2; //skip whitespace
                                                                    current = stack_code[position];
                                                                    while (!char.IsWhiteSpace(current)) //get type
                                                                    {
                                                                        try
                                                                        {
                                                                            buffer.Append(current);
                                                                            current = stack_code[++position];
                                                                        }
                                                                        catch (IndexOutOfRangeException)
                                                                        {
                                                                            VirtualMachine.ThrowVMException("Field`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                        }
                                                                    }
                                                                    try
                                                                    {
                                                                        thisVar.type = VirtualMachine.GetWolClass(buffer.ToString());
                                                                    }
                                                                    catch (NullReferenceException)
                                                                    {
                                                                        //don`t need now throw vm exception becouse in GetWolClass was throw exception
                                                                    }
                                                                    buffer.Clear();
                                                                    current = stack_code[++position]; //skip whitespace
                                                                    while (!char.IsWhiteSpace(current)) //get security modifer
                                                                    {
                                                                        try
                                                                        {
                                                                            buffer.Append(current);
                                                                            current = stack_code[++position];
                                                                        }
                                                                        catch (IndexOutOfRangeException)
                                                                        {
                                                                            VirtualMachine.ThrowVMException("Field`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                        }
                                                                    }
                                                                    SecurityModifer security = SecurityModifer.PRIVATE;
                                                                    try
                                                                    {
                                                                        security = (SecurityModifer)Enum.Parse(typeof(SecurityModifer), buffer.ToString(), true); //write this modifer to our variable
                                                                    }
                                                                    catch (ArgumentException)
                                                                    {
                                                                        VirtualMachine.ThrowVMException($"{buffer.ToString()} is not security modifer of setter (getter)", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                    }
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
                                                                            VirtualMachine.ThrowVMException("Field`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                        }
                                                                    }
                                                                    if (buffer.ToString() == "set")
                                                                    {
                                                                        thisVar.setter.security = security;
                                                                        current = stack_code[++position]; //skip whitespace
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
                                                                            current = stack_code[++position];
                                                                            while (current != ']')
                                                                            {
                                                                                try
                                                                                {
                                                                                    buffer.Append(current);
                                                                                    current = stack_code[++position];
                                                                                }
                                                                                catch (IndexOutOfRangeException)
                                                                                {
                                                                                    VirtualMachine.ThrowVMException("End of block of fields not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                                }
                                                                            }
                                                                            thisVar.setter.body = buffer.ToString();
                                                                        }
                                                                        else
                                                                        {
                                                                            VirtualMachine.ThrowVMException("Start of block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                        }
                                                                        current = stack_code[++position]; //skip ']'
                                                                        while (char.IsWhiteSpace(current))
                                                                        {
                                                                            try
                                                                            {
                                                                                current = stack_code[++position];
                                                                            }
                                                                            catch (IndexOutOfRangeException)
                                                                            {
                                                                                VirtualMachine.ThrowVMException("Getter security not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                            }
                                                                        }
                                                                        buffer.Clear();
                                                                        while (!char.IsWhiteSpace(current))
                                                                        {
                                                                            try
                                                                            {
                                                                                buffer.Append(current);
                                                                                current = stack_code[++position];
                                                                            }
                                                                            catch (IndexOutOfRangeException)
                                                                            {
                                                                                VirtualMachine.ThrowVMException("Getter security not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                            }
                                                                        }
                                                                        try
                                                                        {
                                                                            thisVar.getter.security = (SecurityModifer) Enum.Parse(typeof(SecurityModifer), buffer.ToString());
                                                                        }
                                                                        catch (ArgumentException)
                                                                        {
                                                                            VirtualMachine.ThrowVMException($"{buffer.ToString()} is not security modifer", VirtualMachine.position - stack_code.Length + position, ExceptionType.NotFoundException);
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
                                                                        buffer.Clear();
                                                                        while (!char.IsWhiteSpace(current))
                                                                        {
                                                                            try
                                                                            {
                                                                                buffer.Append(current);
                                                                                current = stack_code[++position];
                                                                            }
                                                                            catch (IndexOutOfRangeException)
                                                                            {
                                                                                VirtualMachine.ThrowVMException("Getter not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                            }
                                                                        }
                                                                        if (buffer.ToString() != "get")
                                                                        {
                                                                            VirtualMachine.ThrowVMException($"Unknown keyword {buffer.ToString()}", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                        }
                                                                    }
                                                                    else if (buffer.ToString() != "get")
                                                                    {
                                                                        VirtualMachine.ThrowVMException($"Unknown keyword {buffer.ToString()}", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                                                        current = stack_code[++position];
                                                                        while (current != ']')
                                                                        {
                                                                            try
                                                                            {
                                                                                buffer.Append(current);
                                                                                current = stack_code[++position];
                                                                            }
                                                                            catch (IndexOutOfRangeException)
                                                                            {
                                                                                VirtualMachine.ThrowVMException("End of block of fields not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                            }
                                                                        }
                                                                        thisVar.getter.body = buffer.ToString();
                                                                        buffer.Clear();
                                                                        newWolClass.fields.Add(var_name, thisVar);
                                                                        if (stack_code[++position] == ',') goto variable;
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
                                                            VirtualMachine.ThrowVMException("Start of fields not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                        }
                                                    }
                                                    break;
                                                case "const":
                                                    if ((newWolClass.classType == wolClassType.ENUM) || (newWolClass.classType == wolClassType.STRUCT))
                                                    {
                                                        while (char.IsWhiteSpace(current)) //skip whitespaces
                                                        {
                                                            try
                                                            {
                                                                current = stack_code[++position];
                                                            }
                                                            catch (IndexOutOfRangeException)
                                                            {
                                                                VirtualMachine.ThrowVMException("Start of constants not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                            }
                                                        }
                                                        if (current == '[')
                                                        {
                                                            while (current != ']')
                                                            {
                                                                try
                                                                {
                                                                    current = stack_code[++position];
                                                                }
                                                                catch (IndexOutOfRangeException)
                                                                {
                                                                    VirtualMachine.ThrowVMException("End of constants block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                } //it`s time solution
                                                            }
                                                        }
                                                        else
                                                        {
                                                            VirtualMachine.ThrowVMException("Open brackets in constants not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        VirtualMachine.ThrowVMException($"{newWolClass.classType.ToString().ToLower()} don`t support constants", VirtualMachine.position - stack_code.Length + position, ExceptionType.TypeNotSupportedException);
                                                    }
                                                    break;
                                                case "],": //костыль(
                                                    continue;
                                                case "];":
                                                case ";": //тоже костыли но без них никак :)
                                                    buffer.Clear();
                                                    goto out_cycle; 
                                                default:
                                                    VirtualMachine.ThrowVMException($"Unknown keyword {buffer.ToString()} in the class initilization", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                    break;
                                            }
                                        }
                                        out_cycle:
                                        try
                                        {
                                            stack.classes.Add(className, newWolClass); //add to return stack this class
                                        }
                                        catch (ArgumentException)
                                        {
                                            //pass
                                        }
                                    }
                                    else
                                    {
                                        VirtualMachine.ThrowVMException("Start of block operator is not valid", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                    }
                                }
                                else
                                {
                                    VirtualMachine.ThrowVMException($"Assigment operator in class initilization is not valid ('{current}')", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                }
                                while (char.IsWhiteSpace(current))
                                {
                                    try
                                    {
                                        current = stack_code[++position];
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        VirtualMachine.ThrowVMException("End of classes not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                    }
                                }
                            }
                        }
                        else
                        {
                            VirtualMachine.ThrowVMException("Classes`s start not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                        }
                        buffer.Clear();
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
                                VirtualMachine.ThrowVMException("Functions is empty", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                            }
                        }
                        if (current == '{')
                        {
                            while (current != '}')
                            {
                                function:
                                current = stack_code[++position]; //skip '{'
                                while (char.IsWhiteSpace(current)) //skip whitespaces
                                {
                                    try
                                    {
                                        current = stack_code[++position];
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        VirtualMachine.ThrowVMException("Function`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                        VirtualMachine.ThrowVMException("Function`s name not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                    }
                                }
                                string func_name = buffer.ToString();
                                buffer.Clear();
                                current = stack_code[++position];
                                if (current != '=') //check assignment operator
                                {
                                    VirtualMachine.ThrowVMException($"Assigment operator isn`t right in fucntion ('{current}')", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                            VirtualMachine.ThrowVMException("Function`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                            VirtualMachine.ThrowVMException("Function`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                        }
                                    }
                                    try
                                    {
                                        func.returnType = VirtualMachine.GetWolClass(buffer.ToString());
                                    }
                                    catch (NullReferenceException)
                                    {
                                        //don`t need now throw vm exception becouse in GetWolClass was throw exception
                                    }
                                    current = stack_code[++position]; //skip whitespace
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
                                        VirtualMachine.ThrowVMException($"Arguments or start of function not found ('{current}')", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                        current = stack_code[++position];
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
                                        stack.functions.Add(func_name, func);
                                        current = stack_code[++position]; //skip ']' (peek next char)
                                        buffer.Clear();
                                        if (current == ',')
                                        {
                                            current = stack_code[++position]; //skip ','
                                            goto function;
                                        }
                                        while (char.IsWhiteSpace(current))
                                        {
                                            try
                                            {
                                                current = stack_code[++position];
                                            }
                                            catch (IndexOutOfRangeException)
                                            {
                                                VirtualMachine.ThrowVMException("End of functions not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                            }
                                        }
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
                            VirtualMachine.ThrowVMException("Functions`s start not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                VirtualMachine.ThrowVMException("Variables is empty", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                            }
                        }
                        if (current == '{')
                        {
                            while (current != '}')
                            {
                                variable:
                                current = stack_code[++position]; //skip '{'
                                while (char.IsWhiteSpace(current)) //skip whitespaces
                                {
                                    try
                                    {
                                        current = stack_code[++position];
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        VirtualMachine.ThrowVMException("Variable`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                        VirtualMachine.ThrowVMException("Variable`s name not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                    }
                                }
                                string var_name = buffer.ToString();
                                buffer.Clear();
                                current = stack_code[++position];
                                if (current != '=') //check assignment operator
                                {
                                    VirtualMachine.ThrowVMException("Assigment operator isn`t right in variable", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                }
                                else
                                {

                                    Value thisVar = Value.VoidValue; //create empty value with parent 'void'
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
                                    buffer.Clear();
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
                                            VirtualMachine.ThrowVMException("Variable`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                        }
                                    }
                                    SecurityModifer security = SecurityModifer.PRIVATE;
                                    try
                                    {
                                        security = (SecurityModifer)Enum.Parse(typeof(SecurityModifer), buffer.ToString(), true); //write this modifer to our variable
                                    }
                                    catch (Exception)
                                    {
                                        VirtualMachine.ThrowVMException($"{buffer.ToString()} is not security modifer", VirtualMachine.position - stack_code.Length + position, ExceptionType.NotFoundException);
                                    }
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
                                            current = stack_code[++position];
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
                                            VirtualMachine.ThrowVMException("Start of setter block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                        }
                                        current = stack_code[++position]; //skip ']'
                                        while (char.IsWhiteSpace(current))
                                        {
                                            try
                                            {
                                                current = stack_code[++position];
                                            }
                                            catch (IndexOutOfRangeException)
                                            {
                                                VirtualMachine.ThrowVMException("Getter security not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                            }
                                        }
                                        buffer.Clear();
                                        while (!char.IsWhiteSpace(current))
                                        {
                                            try
                                            {
                                                buffer.Append(current);
                                                current = stack_code[++position];
                                            }
                                            catch (IndexOutOfRangeException)
                                            {
                                                VirtualMachine.ThrowVMException("Getter security not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                            }
                                        }
                                        try
                                        {
                                            thisVar.getter.security = (SecurityModifer)Enum.Parse(typeof(SecurityModifer), buffer.ToString());
                                        }
                                        catch (ArgumentException)
                                        {
                                            VirtualMachine.ThrowVMException($"{buffer.ToString()} is not security modifer", VirtualMachine.position - stack_code.Length + position, ExceptionType.NotFoundException);
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
                                        buffer.Clear();
                                        while (!char.IsWhiteSpace(current))
                                        {
                                            try
                                            {
                                                buffer.Append(current);
                                                current = stack_code[++position];
                                            }
                                            catch (IndexOutOfRangeException)
                                            {
                                                VirtualMachine.ThrowVMException("Getter not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                            }
                                        }
                                        if (buffer.ToString() != "get")
                                        {
                                            VirtualMachine.ThrowVMException($"Unknown keyword {buffer.ToString()}", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                        }
                                    }
                                    else if (buffer.ToString() != "get")
                                    {
                                        VirtualMachine.ThrowVMException($"Unknown keyword {buffer.ToString()}", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                        current = stack_code[++position];
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
                                        VirtualMachine.ThrowVMException("Start of getter block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                    }
                                    //Console.WriteLine("Var name: " + var_name);
                                    //Console.WriteLine("Setter: " + thisVar.setter.body);
                                    stack.values.Add(var_name, thisVar);
                                    current = stack_code[++position]; //skip ']'
                                    if (current == ',')
                                    {
                                        current = stack_code[++position];
                                        goto variable;
                                    }
                                    while (char.IsWhiteSpace(current))
                                    {
                                        try
                                        {
                                            current = stack_code[++position];
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            VirtualMachine.ThrowVMException("End of functions not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            VirtualMachine.ThrowVMException("Variables`s start not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                        }
                        buffer.Clear();
                    }
                    else
                    {
                        VirtualMachine.ThrowVMException($"Unknown keyword {buffer.ToString()} in stack initilization", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                    }
                    try
                    {
                        if (stack_code[++position] == ';')
                        {
                            current = stack_code[++position]; //skip semicolon
                            goto start;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        return stack;
                    }
                }
                //Console.WriteLine($"Classes length {stack.classes.Count}  funс {stack.functions.Count} vars {stack.values.Count}"); //test
                return stack;
            }
            else
            {
                VirtualMachine.ThrowVMException("Start of block is not '{'", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                return stack;
            }
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

        public void Remove(Value[] elems)
        {
            //pass
        }

        public void Dispose()
        {
            classes.Clear();
            functions.Clear();
            values.Clear();
        }

        public override string ToString()
        {
            string str = "";
            foreach (KeyValuePair<string, Value> keyValuePair in values)
            {
                str += keyValuePair.Key + ' ' + keyValuePair.Value + '\n';
            }
            foreach (KeyValuePair<string, wolClass> keyValuePair in classes)
            {
                str += keyValuePair.Key + ' ' + keyValuePair.Value + '\n';
            }
            foreach (KeyValuePair<string, wolFunction> keyValuePair in functions)
            {
                str += keyValuePair.Key + ' ' + keyValuePair.Value + '\n';
            }
            return str;
        }

        public static Stack operator +(Stack right, Stack left)
        {
            Stack ret = new Stack();
            ret.Add(right);
            ret.Add(left);
            return ret;
        }

        public static Stack operator +(Stack right, Dictionary<string, Value> left)
        {
            foreach (KeyValuePair<string, Value> vari in left)
                right.values.Add(vari.Key, vari.Value);
            return right;
        }

        public static Stack operator +(Stack right, Dictionary<string, wolClass> left)
        {
            foreach (KeyValuePair<string, wolClass> vari in left)
                right.classes.Add(vari.Key, vari.Value);
            return right;
        }

        public static Stack operator +(Stack right, Dictionary<string, wolFunction> left)
        {
            foreach (KeyValuePair<string, wolFunction> vari in left)
                right.functions.Add(vari.Key, vari.Value);
            return right;
        }

        public static Stack operator -(Stack right, Dictionary<string, Value> left)
        {
            foreach (string arg in left.Keys)
            {
                right.values.Remove(arg);
            }
            return right;
        }
    }
}
