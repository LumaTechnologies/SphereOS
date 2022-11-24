using RiverScript.VM;

namespace RiverScript.Tokens
{
    public class NotEqualsOperator : EqualsOperator
    {
        public override string ToString()
        {
            return "!=";
        }

        internal override VMObject Operate(VMObject a, VMObject b)
        {
            return new VMBoolean(!CheckEquality(a, b));
        }
    }
}
