using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using wolvm.expressions;

namespace wolvm
{
    public static class VirtualMachine
    {
        public static Stack mainstack = new Stack();
        public static int position = 0;
        public static Dictionary<string, VMExpression> expressions = new Dictionary<string, VMExpression>
        {
            { "plus", new PlusExpression() },
            { "_loads", new LoadsExpression() },
            { "typeof", new TypeofExpression() },
            { "ifelse", new IfExpression() },
            { "run", new RunExpression() },
            { "parseInt", new ParseIntExpression() },
            { "set", new SetExpression() },
            { "parseDouble", new ParseDoubleExpression() },
            { "toString", new ToStringExpression() },
            { "ls", new LessSignExpression() },
            { "ms", new MoreSignExpression() },
            { "equals", new EqualsExpression() },
            { "not", new InversionExpression() },
            { "length", new LengthExpression() },
            { "minus", new MinusExpression() },
            { "multiply", new MultiplyExpression() },
            { "div", new DivExpression() },
            { "mod", new ModExpression() },
            { "and", new AndExpression() },
            { "or", new OrExpression() },
            { "getByIndex", new GetElementExpression() }
        };
        public static bool test = false;    

        static void Main(string[] args)
        {
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            if (args.Length == 0)
            {
                Console.Write("World of Legends Virtual Machine v{0}\nCopyright snaulX 2019\nType \"dotnet wolvm.dll -help\" in command line to get helper",
                    version);
                Console.ReadKey();
            }
            else
            {
                switch (args[0])
                {
                    case "-info":
                        Console.Write("World of Legends Virtual Machine v{0}\nCopyright snaulX 2019", version);
                        break;
                    case "-help":
                        Console.WriteLine("World of Legends Virtual Machine v{0} Helper", version);
                        Console.WriteLine();
                        Console.WriteLine("Arguments:");
                        Console.WriteLine($"-help ; call World of Legends Virtual Machine v{version} Helper");
                        Console.WriteLine("-info ; print info about this vm");
                        Console.WriteLine("-encode <full file name> ; encode and run build-file");
                        Console.WriteLine("-test <full file name> ; run build-file and in the end print main stack (last in the end) and time of program run");
                        Console.WriteLine("<full file name> ; run build-file");
                        break;
                    case "-encode":
                        Run(Encoding.UTF8.GetString(File.ReadAllBytes(args[1])));
                        break;
                    case "-test":
                        test = true;
                        Run(new StreamReader(File.OpenRead(args[1])).ReadToEnd());
                        break;
                    default:
                        Run(new StreamReader(File.OpenRead(args[0])).ReadToEnd());
                        break;
                }
            }
        }

        /// <summary>
        /// Get WoL class from main stack by name
        /// -----------------------------------------
        /// World of Legends Reflection ¯\_(ツ)_/¯
        /// </summary>
        /// <param name="name">Name for search</param>
        /// <returns></returns>
        public static wolClass GetWolClass(string name)
        {
            try
            {
                return mainstack.classes[name.Trim()];
            }
            catch (KeyNotFoundException)
            {
                ThrowVMException($"Class by name {name.Trim()} not found", position, ExceptionType.NotFoundException);
                return null;
            }
        }

        public static Value FindBlock(string name)
        {
            Value value = Value.VoidValue;
            try
            {
                value = mainstack.values[name];
            }
            catch (KeyNotFoundException)
            {
                ThrowVMException($"Variable by name {name} not found", position, ExceptionType.NotFoundException);
            }
            if (!value.CheckType("Block"))
                ThrowVMException($"Type of variable with name {name} is not Block", position, ExceptionType.InvalidTypeException);
            return value;
        }

        public static Value FindFunc(string name)
        {
            Value value = Value.VoidValue;
            try
            {
                value = new Value(new wolFunc(mainstack.functions[name]));
            }
            catch (KeyNotFoundException)
            {
                try
                {
                    value = mainstack.values[name];
                    if (!value.CheckType("Func"))
                        ThrowVMException($"Type of variable by name {name} is not Func", position, ExceptionType.InvalidTypeException);
                }
                catch (KeyNotFoundException)
                {
                    ThrowVMException($"Function by name {name} not found in main stack", position, ExceptionType.NotFoundException);
                }
            }
            return value;
        }

        public static void Run(string input)
        {
            //add base classes to stack                 parents:
            mainstack.classes.Add("void", new Void()); //no
            mainstack.classes.Add("byte", new wolByte()); //void
            mainstack.classes.Add("short", new wolShort()); //byte
            mainstack.classes.Add("string", new wolString()); //void
            mainstack.classes.Add("int", new wolInt()); //short
            mainstack.classes.Add("float", new wolFloat()); //int
            mainstack.classes.Add("long", new wolLong()); //int
            mainstack.classes.Add("double", new wolDouble()); //float
            mainstack.classes.Add("Type", new wolType()); //void
            mainstack.classes.Add("Func", new wolFunc()); //void
            mainstack.classes.Add("Enum", new wolEnum()); //void
            mainstack.classes.Add("char", new wolChar()); //void
            mainstack.classes.Add("Block", new wolBlock()); //void
            mainstack.classes.Add("Collection", new wolCollection()); //void
            mainstack.classes.Add("Array", new wolArray()); //Collection
            mainstack.classes.Add("Link", new wolLink()); //void
            mainstack.classes.Add("bool", new wolBool()); //void

            //main cycle
            position = 0;
            char current = input[0];
            int time = Environment.TickCount;
            while (position < input.Length)
            {
                while (char.IsWhiteSpace(current)) //skip whitespaces
                {
                    position++;
                    if (position > input.Length)
                    {
                        ThrowVMException("Build-file have only whitespaces", position, ExceptionType.BLDSyntaxException);
                        return;
                    }
                    current = input[position];
                }
                StringBuilder buffer = new StringBuilder();
                while (!char.IsWhiteSpace(current)) //get word
                {
                    buffer.Append(current);
                    position++;
                    try
                    {
                        current = input[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        ThrowVMException("Build-file have only one word", position, ExceptionType.BLDSyntaxException);
                        return;
                    }
                }
                if (buffer.ToString() == "_loads")
                {
                    buffer.Clear();
                    while (char.IsWhiteSpace(current))
                    {
                        position++;
                        if (position > input.Length)
                        {
                            ThrowVMException("Start of loads struct not found", position, ExceptionType.BLDSyntaxException);
                            return;
                        }
                        current = input[position];
                    }
                    if (current == '{')
                    {
                        buffer.Clear();
                        while (current != '}') //get loads body
                        {
                            buffer.Append(current);
                            position++;
                            if (position > input.Length)
                            {
                                ThrowVMException("End of loads struct not found", position, ExceptionType.BLDSyntaxException);
                                return;
                            }
                            current = input[position];
                        }
                        buffer.Remove(0, 1);
                        //start parse loads
                        string dllSource = buffer.ToString();
                        Type mainType = typeof(VMLibrary);
                        List<string> dllNames = dllSource.Split(';').ToList();
                        foreach (string dllName in dllNames)
                        {
                            Assembly assembly = null;
                            string full_path = AppDomain.CurrentDomain.BaseDirectory + dllName.Trim() + ".dll";
                            try
                            {
                                assembly = Assembly.LoadFrom(full_path);
                            } 
                            catch (Exception ex)
                            {
                                ThrowVMException($"Library with info {full_path} not found.\n{ex.Message}", position, ExceptionType.FileNotFoundException);
                                break;
                            }
                            Type mainClass = assembly.GetTypes().FirstOrDefault(t => t != mainType && mainType.IsAssignableFrom(t));
                            if (test)
                            {
                                Console.WriteLine("Framework Info: " + assembly);
                                Console.WriteLine("Full path to framework: " + full_path);
                                Console.WriteLine(string.Join<Type>(' ', assembly.GetTypes()));
                            }
                            if (mainClass != null)
                            {
                                if (Activator.CreateInstance(mainClass) is VMLibrary mainObj) mainObj.Load();
                                else ThrowVMException($"Main class in library by name {dllName} haven`t type VMLibrary and will cannot loaded", position, ExceptionType.LoadsException);
                            }
                            else
                            {
                                ThrowVMException($"Library by name {dllName} haven`t main class and will cannot loaded", position, ExceptionType.LoadsException);
                            }
                            
                        } 
                        //end parse loads
                    }
                    else
                    {
                        ThrowVMException("Start of loads struct not found", position, ExceptionType.BLDSyntaxException);
                    }
                }
                else if (buffer.ToString() == "stack")
                {
                    buffer.Clear();
                    while (char.IsWhiteSpace(current))
                    {
                        position++;
                        if (position > input.Length)
                        {
                            ThrowVMException("Start of stack not found", position, ExceptionType.BLDSyntaxException);
                            return;
                        }
                        current = input[position];
                    }
                    if (current == '{')
                    {
                        cycle: while (current != '}') //get stack body
                        {
                            buffer.Append(current);
                            position++;
                            if (position > input.Length)
                            {
                                ThrowVMException("End of stack not found", position, ExceptionType.BLDSyntaxException);
                                return;
                            }
                            current = input[position];
                        }
                        if (input[++position] == ';')
                        {
                            buffer.Append(current);
                            current = input[position];
                            goto cycle;
                        }
                        mainstack.Add(Stack.Parse(buffer.ToString().Trim()));
                    }
                    else
                    {
                        ThrowVMException("Start of stack not found", position, ExceptionType.BLDSyntaxException);
                    }
                    position--;
                }
                else if (buffer.ToString() == "main")
                {
                    buffer.Clear();
                    while (char.IsWhiteSpace(current))
                    {
                        position++;
                        if (position > input.Length)
                        {
                            ThrowVMException("Start of script not found", position, ExceptionType.BLDSyntaxException);
                            return;
                        }
                        current = input[position];
                    }
                    if (current == '{')
                    {
                        while (current != '}') //get script
                        {
                            buffer.Append(current);
                            position++;
                            if (position > input.Length)
                            {
                                ThrowVMException("End of script not found", position, ExceptionType.BLDSyntaxException);
                                return;
                            }
                            current = input[position];
                        }
                        Script.Parse(buffer.ToString().Trim().Remove(0, 1));
                    }
                    else
                    {
                        ThrowVMException("Start of script not found", position, ExceptionType.BLDSyntaxException);
                    }
                }
                else if (buffer.ToString() == "end")
                {
                    if (test)
                    {
                        //test stack
                        Console.WriteLine("Info about program in the end.\nMain stack:");
                        Console.WriteLine(mainstack.ToString());
                        Console.WriteLine("Expressions:");
                        foreach (string expr_name in expressions.Keys)
                        {
                            Console.WriteLine(expr_name);
                        }
                        Console.WriteLine($"Time of program: {Environment.TickCount - time}");
                    }
                    return;
                }
                else if (buffer.ToString() == "}")
                {
                    position++;
                    continue;
                }
                else
                {
                    ThrowVMException($"Unknown keyword {buffer.ToString()}", position, ExceptionType.BLDSyntaxException);
                }
            }
        }

        /// <summary>
        /// Throw Virtual Machine Exception (RUNError)
        /// </summary>
        /// <param name="message">Exception message with big first letter</param>
        /// <param name="position">Position where throwed exception</param>
        /// <param name="type">Type of exception</param>
        public static void ThrowVMException(string message, int position, ExceptionType type)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Error.WriteLine($"{type.ToString()}. Exception in position {position}. {message}");
            Console.BackgroundColor = ConsoleColor.Black;
            //Environment.Exit(((int) type) + 2); //for try-catch feauture
        }
    }

    public abstract class VMLibrary
    {
        public Stack stack = new Stack();
        public Dictionary<string, VMExpression> expressions = new Dictionary<string, VMExpression>();
        public void Load()
        {
            foreach (KeyValuePair<string, VMExpression> pair in expressions)
            {
                VirtualMachine.expressions.Add(pair.Key, pair.Value);
            }
            VirtualMachine.mainstack.Add(stack);
        }
    }

    public enum ExceptionType
    {
        //this type throws when ...
        TypeNotSupportedException, //... when in enum init method and etc.
        StackOverflowException, //... size of stack is bigger then memory
        InvalidTypeException, //... get type doesn`t fits
        NotFoundException, //... 'anything' not found
        InitilizateException, //... problems made on initilization
        BLDSyntaxException, //... wrong syntax of build-file
        LoadsException, //... framework can`t load or other troubles with him
        FileNotFoundException, //... file not found
        ArgumentsNullException, //... argument have 'null'
        IndexOutOfRangeException, //... get index who bigger than length of collection
        NullRefrenceException, //... operation work with 'null'
        ChildException, //... class or functions hasn`t child
        SecurityException, //... call private property
        ArgumentsOutOfRangeException, //... arguments bigger or lower than need
        NumberFormatException, //... parsing not valid string (to any number)
        FormatException, //... parsing not valid string (to any type)
        ValueException //... value call with parents or any more
    }
}
