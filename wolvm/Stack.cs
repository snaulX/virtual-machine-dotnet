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
                            VirtualMachine.ThrowVMException("End of block of stack not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
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
                                VirtualMachine.ThrowVMException("Class haven`t block", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                            }
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
                                while (!char.IsWhiteSpace(current)) //get open bracket '{'
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
                                if (buffer.ToString().Trim() == "{")
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
                                            VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
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
                                            VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
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
                                            VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);

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
                                                VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);

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
                                                VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);

                                            }
                                        }
                                        wolClass newWolClass = new wolClass { }; //create empty class
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
                                                VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                            }
                                        }
                                        try
                                        {
                                            newWolClass.security = (SecurityModifer) Enum.Parse(typeof(SecurityModifer), buffer.ToString(), true);
                                        }
                                        catch (Exception)
                                        {
                                            VirtualMachine.ThrowVMException(buffer.ToString() + " is not security modifer", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                                    VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
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
                                            while (current != ';') //parse class body
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
                                                        VirtualMachine.ThrowVMException("Classes`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                                    }
                                                }
                                                buffer.Clear();
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
                                                                    current = stack_code[++position]; //skip open bracket
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
                                                                            VirtualMachine.ThrowVMException("Constructor`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                                                        }
                                                                    }
                                                                    if (current != '=') //check assignment operator
                                                                    {
                                                                        VirtualMachine.ThrowVMException("Assigment operator isn`t right in constructor " + current, VirtualMachine.position, ExceptionType.BLDSyntaxException);
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
                                                                            else newWolClass.constructors.Add(constrnanme, constr);
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
                                                                VirtualMachine.ThrowVMException("Start of methods not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                                                            VirtualMachine.ThrowVMException("Method`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
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
                                                                            VirtualMachine.ThrowVMException("Method`s name not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                                                        }
                                                                    }
                                                                    string func_name = buffer.ToString();
                                                                    buffer.Clear();
                                                                    current = stack_code[++position];
                                                                    if (current != '=') //check assignment operator
                                                                    {
                                                                        VirtualMachine.ThrowVMException("Assigment operator isn`t right in method", VirtualMachine.position, ExceptionType.BLDSyntaxException);
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
                                                                                func.arguments.Add(name, type); //add argumrnt (null pointer) to method
                                                                            }
                                                                            //start parse block
                                                                        }
                                                                        else
                                                                        {
                                                                            VirtualMachine.ThrowVMException("Arguments or start of method not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                                                                VirtualMachine.ThrowVMException("Start of method block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                                                                    VirtualMachine.ThrowVMException("End of block of method not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                                }
                                                                            }
                                                                            func.body = buffer.ToString();
                                                                            newWolClass.methods.Add(func_name, func);
                                                                            if (stack_code[position + 1] == ',') goto function;
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
                                                                    VirtualMachine.ThrowVMException("Assigment operator isn`t right in field", VirtualMachine.position, ExceptionType.BLDSyntaxException);
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
                                                                            VirtualMachine.ThrowVMException("Field`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                                                            VirtualMachine.ThrowVMException("Field`s end not found", VirtualMachine.position, ExceptionType.BLDSyntaxException);
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
                                                                            VirtualMachine.ThrowVMException("Field`s end not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                                                                    VirtualMachine.ThrowVMException("End of block of fields not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                                                                VirtualMachine.ThrowVMException("End of block of fields not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                            }
                                                                        }
                                                                        thisVar.getter.body = buffer.ToString();
                                                                    }
                                                                    else
                                                                    {
                                                                        VirtualMachine.ThrowVMException("Start of block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                                    }
                                                                    newWolClass.fields.Add(var_name, thisVar);
                                                                    if (stack_code[++position] == ',') goto variable;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            VirtualMachine.ThrowVMException("Start of fields not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                                    case "],":
                                                        break; //костыль((
                                                    default:
                                                        VirtualMachine.ThrowVMException("Unknown keyword " + buffer.ToString() + " in the class initilization", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                                        break;
                                                }
                                                try
                                                {
                                                    stack.classes.Add(className, newWolClass); //add to return stack this class
                                                }
                                                catch (ArgumentException)
                                                {
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
                                        VirtualMachine.ThrowVMException("Equals operator is not valid " + current, VirtualMachine.position, ExceptionType.BLDSyntaxException);
                                    }

                                }
                                else
                                {
                                    VirtualMachine.ThrowVMException("Open bracket is not valid", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                    VirtualMachine.ThrowVMException("Assigment operator isn`t right in fucntion", VirtualMachine.position, ExceptionType.BLDSyntaxException);
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
                                        stack.functions.Add(func_name, func);
                                        if (stack_code[position + 1] == ',') goto function;
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
                                    VirtualMachine.ThrowVMException("Assigment operator isn`t right in variable", VirtualMachine.position, ExceptionType.BLDSyntaxException);
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
                                            VirtualMachine.ThrowVMException("Start of setter block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
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
                                        VirtualMachine.ThrowVMException("Start of getter block not found", VirtualMachine.position - stack_code.Length + position, ExceptionType.BLDSyntaxException);
                                    }
                                    stack.values.Add(var_name, thisVar);
                                    if (stack_code[position + 1] == ',')
                                    {
                                        goto variable;
                                    }
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
                Console.WriteLine($"Classes length {stack.classes.Count}  funs {stack.functions.Count} vars {stack.values.Count}"); //test
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
