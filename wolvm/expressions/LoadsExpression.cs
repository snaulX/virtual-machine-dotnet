using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace wolvm.expressions
{
    public class LoadsExpression : VMExpression
    {
        public Value ParseExpression(params Value[] args)
        {
            Type mainType = typeof(VMLibrary);
            foreach (Value arg in args)
            {
                string dllName = ((wolString)arg.type).value;
                Assembly assembly = null;
                string full_path = System.IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + dllName.Trim() + ".dll");
                try
                {
                    assembly = Assembly.LoadFrom(full_path);
                }
                catch (Exception ex)
                {
                    VirtualMachine.ThrowVMException($"Library with info {full_path} not found.\n{ex.Message}", VirtualMachine.position, ExceptionType.FileNotFoundException);
                    break;
                }
                Type mainClass = assembly.GetTypes().FirstOrDefault(t => t != mainType && mainType.IsAssignableFrom(t));
                if (mainClass != null)
                {
                    if (Activator.CreateInstance(mainClass) is VMLibrary mainObj) mainObj.Load();
                    else VirtualMachine.ThrowVMException($"Main class in library by name {dllName} haven`t type VMLibrary and will cannot loaded", VirtualMachine.position, ExceptionType.LoadsException);
                }
                else
                {
                    VirtualMachine.ThrowVMException($"Library by name {dllName} haven`t main class and will cannot loaded", VirtualMachine.position, ExceptionType.LoadsException);
                }

            }
            return Value.VoidValue;
        }
    }
}
