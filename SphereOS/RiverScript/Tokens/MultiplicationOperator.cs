using RiverScript.VM;
using System;

namespace RiverScript.Tokens
{
    public class MultiplicationOperator : Operator
    {
        public override string ToString()
        {
            return "*";
        }

        internal override VMObject Operate(VMObject a, VMObject b)
        {
            if (a is VMNumber numA && b is VMNumber numB)
            {
                return new VMNumber(numA.Value * numB.Value);
            }
            throw new ArithmeticException($"Cannot multiply {a.ToString()} by {b.ToString()}.");
        }
    }
}
