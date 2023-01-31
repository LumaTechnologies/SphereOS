using RiverScript.VM;
using System;

namespace RiverScript.Tokens
{
    public class GreaterEqualOperator : Operator
    {
        public override string ToString()
        {
            return ">=";
        }

        internal override VMObject Operate(VMObject a, VMObject b)
        {
            if (a is not VMNumber numA || b is not VMNumber numB)
                throw new Exception($"Cannot compare {a.ToString()} to {b.ToString()}.");

            return new VMBoolean(numA.Value >= numB.Value);
        }
    }
}
