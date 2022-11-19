using RiverScript.VM;
using System;
using System.Collections.Generic;

namespace RiverScript.StandardLibrary
{
    internal static partial class StandardLibrary
    {
        private static VMNativeFunction Stdlib_print = new VMNativeFunction(
        new List<string> { ("string") },
        (List<VMObject> arguments) =>
        {
            Console.WriteLine(arguments[0]);
            return new VMNull();
        });

        private static VMNativeFunction Stdlib_read = new VMNativeFunction(
        new List<string>(),
        (List<VMObject> arguments) =>
        {
            return new VMString(Console.ReadLine()!);
        });
    }
}
