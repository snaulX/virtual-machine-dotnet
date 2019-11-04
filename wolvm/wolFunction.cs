using System;
using System.Collections.Generic;
using System.Text;

namespace wolvm
{
    public class wolFunction
    {
        public SecurityModifer security;
        public wolClass returnType;
        public Dictionary<string, wolClass> arguments;
        public string body;
        public bool close = false;

        public wolFunction(SecurityModifer sec = SecurityModifer.PRIVATE, string _body = "return <null:void>;")
        {
            security = sec;
            returnType = new Void();
            arguments = new Dictionary<string, wolClass>();
            body = _body;
        }

        public wolFunction(SecurityModifer sec = SecurityModifer.PRIVATE, params KeyValuePair<string, wolClass>[] args)
        {
            security = sec;
            arguments = new Dictionary<string, wolClass>(args.Length);
            for (int i = 0; i < args.Length; i++)
            {
                arguments.Add(args[i].Key, args[i].Value); //да я знаю про foreach, но for быстрее
            }
            body = "return <null:void>;";
            returnType = new Void();
        }

        /// <summary>
        /// Create function how constructor
        /// </summary>
        /// <param name="type">Type of constructor</param>
        /// <param name="args">Arguments of constructor</param>
        /// <returns></returns>
        public static wolFunction NewDefaultConstructor(wolClass type, params KeyValuePair<string, wolClass>[] args)
        {
            wolFunction constr = new wolFunction
            {
                returnType = type,
                body = "",
                security = SecurityModifer.PUBLIC
            };
            for (int i = 0; i < args.Length; i++)
            {
                constr.arguments.Add(args[i].Key, args[i].Value); //да я знаю про foreach, но for быстрее
                constr.body += "&@this." + args[i].Key + ".#set : <null:void>;\n";
            }
            return constr;
        }

        public void Call(params Value[] args)
        {
            Dictionary<string, Value> fullargs = new Dictionary<string, Value>(); //create dictionary who will full by args

            Script.Parse(body);
        }
    }
}
