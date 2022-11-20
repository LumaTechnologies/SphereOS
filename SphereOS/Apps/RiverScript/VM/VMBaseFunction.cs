using System.Collections.Generic;

namespace RiverScript.VM
{
    internal class VMBaseFunction : VMObject
    {
        internal VMBaseFunction()
        {
            Arguments = new();
        }

        internal VMBaseFunction(List<string> arguments)
        {
            Arguments = arguments;
        }

        internal List<string> Arguments { get; set; }

        public override string ToString()
        {
            return "<VMObject BaseFunction>";
        }

        internal override bool PassByReference()
        {
            return true;
        }
    }
}
