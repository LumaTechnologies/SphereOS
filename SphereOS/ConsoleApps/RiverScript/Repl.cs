using RiverScript.Tokens;
using RiverScript.VM;
using System;
using System.Collections.Generic;

namespace RiverScript
{
    internal class Repl
    {
        internal Interpreter Interpreter { get; } = new Interpreter();

        internal void Start()
        {
            StandardLibrary.StandardLibrary.LoadStandardLibrary(Interpreter);

            Console.Clear();
            Console.WriteLine("RiverScript v0.1.1. Copyright (c) 2023. All rights reserved.");
            Console.WriteLine("Type exit() to exit.");

            bool exit = false;
            VMNativeFunction replcmd_exit = new VMNativeFunction(
            new List<string>(),
            (List<VMObject> arguments) =>
            {
                exit = true;
                return new VMNull();
            });
            Interpreter.DefineVariable("exit", replcmd_exit, scope: null);

            while (!exit)
            {
                Console.Write(">>> ");

                string input = Console.ReadLine();
                if (input == null)
                    continue;

                if (input.Trim().EndsWith("{"))
                {
                    input += "\n";
                    while (true)
                    {
                        Console.Write("... ");

                        string input1 = Console.ReadLine();

                        if (input1 == null)
                            continue;

                        if (input1 != string.Empty)
                        {
                            input += input1 + "\n";
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                try
                {
                    Script script = new Script(input);

                    script.Lex();

                    //script.RootToken.Debug();
                    bool isExpression = true;
                    foreach (Token token in script.RootToken.Tokens)
                    {
                        if (token is Tokens.Assignment || token is Tokens.Block)
                        {
                            isExpression = false;
                        }
                    }

                    if (isExpression)
                    {
                        VMObject result = Interpreter.InterpretExpression(script.RootToken, script.RootToken);
                        if (result != null && result is not VMNull)
                        {
                            Console.WriteLine(result.ToString());
                        }
                    }
                    else
                    {
                        Interpreter.InterpretScript(script);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

#if SPHEREOS
                SphereOS.Core.ProcessManager.Yield();
#endif
            }
        }
    }
}
