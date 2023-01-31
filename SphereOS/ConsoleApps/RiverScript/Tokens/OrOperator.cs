using RiverScript.VM;

namespace RiverScript.Tokens
{
    public class OrOperator : Operator
    {
        public override string ToString()
        {
            return "or";
        }

        internal override VMObject Operate(VMObject a, VMObject b)
        {
            if (a.IsTruthy())
                return a;
            else
                return b;
        }
    }
}
