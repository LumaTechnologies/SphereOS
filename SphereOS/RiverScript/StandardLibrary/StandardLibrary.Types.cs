using RiverScript.VM;
using System;
using System.Collections.Generic;

namespace RiverScript.StandardLibrary
{
    internal static partial class StandardLibrary
    {
        private static VMNativeFunction Stdlib_str = new VMNativeFunction(
        new List<string> { ("object") },
        (List<VMObject> arguments) =>
        {
            return new VMString(arguments[0].ToString());
        });

        private static VMNativeFunction Stdlib_num = new VMNativeFunction(
        new List<string> { ("string") },
        (List<VMObject> arguments) =>
        {
            if (arguments[0] is VMString)
            {
                if (double.TryParse(((VMString)arguments[0]).Value, out double result))
                {
                    return new VMNumber(result);
                }
                else
                {
                    throw new Exception("Failed to parse string to a number.");
                }
            }
            else
            {
                throw new Exception("num expects a string.");
            }
        });

        private static VMNativeFunction Stdlib_typeof = new VMNativeFunction(
        new List<string> { ("object") },
        (List<VMObject> arguments) =>
        {
            string typeStr = arguments[0] switch
            {
                VMBaseFunction => "function",
                VMBoolean => "boolean",
                VMNull => "null",
                VMNumber => "number",
                VMRange => "range",
                VMString => "string",
                _ => "object"
            };
            return new VMString(typeStr);
        });
    }
}
