using System;
using System.Collections.Generic;

namespace RiverScript.VM
{
    internal class VMNativeFunction : VMBaseFunction
    {
        internal VMNativeFunction(Func<List<VMObject>, VMObject> func)
        {
            Func = func;
        }

        internal VMNativeFunction(List<string> arguments, Func<List<VMObject>, VMObject> func)
        {
            Func = func;
            Arguments = arguments;
        }

        internal Func<List<VMObject>, VMObject> Func { get; private set; }

        public override string ToString()
        {
            return "<VMObject NativeFunction>";
        }
    }
}
