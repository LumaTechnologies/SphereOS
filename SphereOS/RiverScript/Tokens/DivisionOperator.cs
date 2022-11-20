using RiverScript.VM;
using System;

namespace RiverScript.Tokens
{
    public class DivisionOperator : Operator
    {
        public override string ToString()
        {
            return "/";
        }

        internal override VMObject Operate(VMObject a, VMObject b)
        {
            if (a is VMNumber numA && b is VMNumber numB)
            {
                if (numB.Value == 0)
                {
                    throw new DivideByZeroException($"Cannot divide {a.ToString()} by zero.");
                }
                return new VMNumber(numA.Value / numB.Value);
            }
            throw new Exception($"Cannot divide {a.ToString()} by {b.ToString()}.");
        }
    }
}
