using RiverScript.VM;
using System;

namespace RiverScript.Tokens
{
    public class RangeOperator : Operator
    {
        public override string ToString()
        {
            return "..";
        }

        internal override VMObject Operate(VMObject a, VMObject b)
        {
            if (a is VMNumber numA && b is VMNumber numB)
            {
                return new VMRange(numA.Value, numB.Value);
            }
            throw new ArithmeticException($"Cannot instantiate a new range from {a.ToString()} and {b.ToString()}.");
        }
    }
}
