using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
//using System.Runtime.Loader;
using System.Text;

namespace wolvm
{
    public static class VirtualMachine
    {
        public static Stack mainstack = new Stack();
        public static string[] wol_args;
        public static int position = 0;
        public static List<VMExpression> expressions = new List<VMExpression>
        {
            new BeepExpression(),
            new PlusExpression(),
            new LoadsExpression(),
            new TypeofExpression()
        };
        //initilizate base classes
        public static KeyValuePair<string, wolClass> Void = new KeyValuePair<string, wolClass>("void", new Void()),
            wolBool = new KeyValuePair<string, wolClass>("bool", new wolClass
            {
                security = SecurityModifer.PUBLIC,
                classType = wolClassType.STRUCT,
                constants = new Dictionary<string, Value>
                {
                    { "false", new Value(wolInt.Value, "<0:int>") },
                    { "true", new Value(wolInt.Value, "<1:int>") }
                }
            }),
            wolString = new KeyValuePair<string, wolClass>("string", new wolClass(SecurityModifer.PUBLIC, wolClassType.DEFAULT, "string")),
            wolDouble = new KeyValuePair<string, wolClass>("double", new wolClass(SecurityModifer.PUBLIC, wolClassType.STRUCT, "double")),
            wolInt = new KeyValuePair<string, wolClass>("int", new wolClass(SecurityModifer.PUBLIC, wolClassType.STRUCT, "int")),
            wolType = new KeyValuePair<string, wolClass>("Type", new wolClass(SecurityModifer.PUBLIC, wolClassType.DEFAULT, "Type")),
            wolFunc = new KeyValuePair<string, wolClass>("Func", new wolClass(SecurityModifer.PUBLIC, wolClassType.DEFAULT, "Function")),
            wolEnum = new KeyValuePair<string, wolClass>("Enum", new wolClass(SecurityModifer.PUBLIC, wolClassType.STATIC)),
            wolChar = new KeyValuePair<string, wolClass>("char", new wolClass(SecurityModifer.PUBLIC, wolClassType.STRUCT, "char")),
            wolBlock = new KeyValuePair<string, wolClass>("Block", new wolClass(SecurityModifer.PUBLIC, wolClassType.DEFAULT, "Virtual")),
            wolCollection = new KeyValuePair<string, wolClass>("Collection", new wolClass(SecurityModifer.PUBLIC, wolClassType.ABSTRACT, "Collection")),
            wolArray = new KeyValuePair<string, wolClass>("Array", wolClass.CreateCollection(SecurityModifer.PUBLIC, "Array")),
            wolLink = new KeyValuePair<string, wolClass>("Link", new wolClass
            {
                classType = wolClassType.DEFAULT,
                security = SecurityModifer.PUBLIC,
                constructors = new Dictionary<string, wolFunction>
                {
                    { "href", wolFunction.NewDefaultConstructor(wolLink.Value) }
                },
                fields = new Dictionary<string, Value>
                {
                    { "HasSetter", new Value(VirtualMachine.wolBool.Value) },
                    { "Address", new Value(VirtualMachine.wolString.Value) }
                },
                methods = new Dictionary<string, wolFunction>
                {
                    { "Call", new wolFunction
                        {
                            security = SecurityModifer.PUBLIC,
                            arguments = new Dictionary<string, wolClass>
                            {
                                { "method_name", wolString.Value },
                                { "arguments", wolArray.Value }
                            },
                            body = "Call : &this, @method_name, @arguments ;"
                        }
                    }
                }
            }
            );  

        static void Main(string[] args)
        {
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            if (args.Length == 0)
            {
                Console.Write("World of Legends Virtual Machine v{0}\nCopyright snaulX 2019\nType \"-help\" to get helper", version);
            }
            else
            {
                //create and start 'context of thread'

                //add base classes to stack
                mainstack.classes.Add(Void.Key, Void.Value);
                mainstack.classes.Add(wolString.Key, wolString.Value);
                mainstack.classes.Add(wolDouble.Key, wolDouble.Value);
                mainstack.classes.Add(wolInt.Key, wolInt.Value);
                mainstack.classes.Add(wolType.Key, wolType.Value);
                mainstack.classes.Add(wolFunc.Key, wolFunc.Value);
                mainstack.classes.Add(wolEnum.Key, wolEnum.Value);
                mainstack.classes.Add(wolChar.Key, wolChar.Value);
                mainstack.classes.Add(wolBlock.Key, wolBlock.Value);
                mainstack.classes.Add(wolCollection.Key, wolCollection.Value);
                mainstack.classes.Add(wolArray.Key, wolArray.Value);
                mainstack.classes.Add(wolLink.Key, wolLink.Value);

                switch (args[0])
                {
                    case "-info":
                        Console.Write("World of Legends Virtual Machine v{0}\nCopyright snaulX 2019", version);
                        break;
                    case "-help":
                        Console.Write("World of Legends Virtual Machine v{0} Helper not found in files." +
                            " Please, wait or download new versions", version);
                        break;
                    case "-run":
                        TextReader reader = new StreamReader(File.OpenRead(args[1]));
                        Run(reader.ReadToEnd());
                        break;
                    default:
                        reader = new StreamReader(File.OpenRead(args[0]));
                        Run(reader.ReadToEnd());
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
                ThrowVMException("Class by name " + name.Trim() + " not found", position, ExceptionType.NotFoundException);
                return null;
            }
        }
        
        public static void Run(string input)
        {
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
                            try
                            {
                                assembly = Assembly.Load(new AssemblyName("vmlib/" + dllName.Trim() + ".dll"));
                            } 
                            catch (Exception ex)
                            {
                                ThrowVMException("Library with info vmlib/" + dllName.Trim() + " not found " + ex.GetType(), position, ExceptionType.FileNotFoundException);
                                break;
                            }
                            Type mainClass = assembly.GetTypes().FirstOrDefault(t => t != mainType && mainType.IsAssignableFrom(t));
                            if (mainClass != null)
                            {
                                if (Activator.CreateInstance(mainClass) is VMLibrary mainObj) mainObj.Load();
                                else ThrowVMException("Main class in library by name " + dllName + " haven`t type VMLibrary and will cannot loaded", position, ExceptionType.LoadsException);
                            }
                            else
                            {
                                ThrowVMException("Library by name " + dllName + "haven`t main class and will cannot loaded", position, ExceptionType.LoadsException);
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
                        Script.Parse(buffer.ToString().Trim().Remove(0, 1), mainstack);
                    }
                    else
                    {
                        ThrowVMException("Start of script not found", position, ExceptionType.BLDSyntaxException);
                    }
                }
                else if (buffer.ToString() == "end")
                {
                    //test stack
                    foreach (KeyValuePair<string, Value> keyValuePair in mainstack.values)
                    {
                        Console.WriteLine(keyValuePair.Key + ' ' + keyValuePair.Value);
                    }
                    foreach (KeyValuePair<string, wolClass> keyValuePair in mainstack.classes)
                    {
                        Console.WriteLine(keyValuePair.Key + ' ' + keyValuePair.Value);
                    }
                    foreach (KeyValuePair<string, wolFunction> keyValuePair in mainstack.functions)
                    {
                        Console.WriteLine(keyValuePair.Key + ' ' + keyValuePair.Value);
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
            Console.WriteLine($"Time of program: {time - Environment.TickCount}");
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
        List<VMExpression> expressions = new List<VMExpression>();
        public void Load()
        {
            VirtualMachine.expressions = expressions;
            VirtualMachine.mainstack.Add(stack);
        }
    }

    public enum ExceptionType
    {
        //this type throws when ...
        TypeNotSupportedException, //... when in enum init method and more
        StackOverflowException, //... size of stack is bigger then memory
        InvalidTypeException, //... get type doesn`t fits
        NotFoundException, //... 'anything' not found
        InitilizateException,
        BLDSyntaxException, //... wrong syntax of build-file
        LoadsException, //... framework can`t load or other troubles with him
        FileNotFoundException, //... file not found
        ArgumentsNullException, //... argument have 'null'
        IndexOutOfRangeException, //... get index who bigger than length of collection
        NullRefrenceException, //... operation work with 'null'
        ChildException, //... class or functions hasn`t child
        SecurityException, //... call private property
        ArgumentsOutOfRangeException //... arguments bigger or lower than need
    }
}
