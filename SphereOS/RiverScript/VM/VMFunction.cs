using RiverScript.Tokens;
using System.Collections.Generic;

namespace RiverScript.VM
{
    internal class VMFunction : VMBaseFunction
    {
        internal VMFunction(Block root)
        {
            Root = root;
        }

        internal VMFunction(Block root, List<string> arguments)
        {
            Root = root;
            Arguments = arguments;
        }

        internal static VMFunction FromScript(Script script)
        {
            return new VMFunction(script.RootToken);
        }

        internal Block Root { get; set; }

        public override string ToString()
        {
            return "<VMObject Function>";
        }
    }
}
