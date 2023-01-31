using RiverScript.VM;
using System;
using System.Collections.Generic;

namespace RiverScript.StandardLibrary
{
    internal static partial class StandardLibrary
    {
        private static VMNativeFunction Stdlib_print = new VMNativeFunction(
        new List<string> { ("object") },
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

        private static VMNativeFunction Stdlib_exec = new VMNativeFunction(
        new List<string>() { ("string") },
        (List<VMObject> arguments) =>
        {
            if (arguments[0] is not VMString command)
                throw new Exception("exec expects a string.");

#if SPHEREOS
            SphereOS.Users.User user = SphereOS.Kernel.CurrentUser;

            SphereOS.Commands.ReturnCode returnCode = SphereOS.Shell.Shell.CurrentShell.Execute(command.Value);

            // Prevent the script continuing as another user.
            // (Security)
            if (SphereOS.Kernel.CurrentUser != user)
            {
                throw new Exception("Script halted.");
            }

            return new VMNumber((double)returnCode);
#else
            string[] split = command.Value.Split(' ', 2);

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(split[0], split[1]);

            return new VMNumber(process.ExitCode);
#endif
        });
    }
}
