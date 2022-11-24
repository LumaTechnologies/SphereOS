using RiverScript.VM;

namespace RiverScript.Tokens
{
    public abstract class Operator : Token
    {
        internal abstract VMObject Operate(VMObject a, VMObject b);

        public override string ToString()
        {
            return "Operator";
        }
    }
}
