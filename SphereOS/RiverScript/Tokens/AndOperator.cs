using RiverScript.VM;

namespace RiverScript.Tokens
{
    public class AndOperator : Operator
    {
        public override string ToString()
        {
            return "and";
        }

        internal override VMObject Operate(VMObject a, VMObject b)
        {
            if (!a.IsTruthy())
                return a;

            if (!b.IsTruthy())
                return b;

            return b;
        }
    }
}
