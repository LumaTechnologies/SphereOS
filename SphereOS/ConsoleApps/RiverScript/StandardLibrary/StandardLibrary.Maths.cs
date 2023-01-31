using RiverScript.VM;
using SphereOS;
using System;
using System.Collections.Generic;

namespace RiverScript.StandardLibrary
{
    internal static partial class StandardLibrary
    {
        private static VMNativeFunction Stdlib_floor = new VMNativeFunction(
        new List<string> { ("string") },
        (List<VMObject> arguments) =>
        {
            if (arguments[0] is not VMNumber num)
                throw new Exception("floor expects a number.");

            return new VMNumber(Math.Floor(num.Value));
        });

        private static VMNativeFunction Stdlib_round = new VMNativeFunction(
        new List<string>() { "number" },
        (List<VMObject> arguments) =>
        {
            if (arguments[0] is not VMNumber num)
                throw new Exception("round expects a number.");

            return new VMNumber(Math.Round(num.Value));
        });

        private static VMNativeFunction Stdlib_ceil = new VMNativeFunction(
        new List<string>() { "number" },
        (List<VMObject> arguments) =>
        {
            if (arguments[0] is not VMNumber num)
                throw new Exception("ceil expects a number.");

            return new VMNumber(Math.Ceiling(num.Value));
        });

        private static VMNativeFunction Stdlib_rand = new VMNativeFunction(
        new List<string>() { "range" },
        (List<VMObject> arguments) =>
        {
            if (arguments[0] is not VMRange range)
                throw new Exception("rand expects a range.");

            return new VMNumber(Kernel.Random.Next((int)range.Lower, (int)range.Upper));
        });
    }
}
