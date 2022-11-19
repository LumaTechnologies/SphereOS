using RiverScript.VM;
using System;

namespace RiverScript.Tokens
{
    public class AdditionOperator : Operator
    {
        public override string ToString()
        {
            return "+";
        }

        internal override VMObject Operate(VMObject a, VMObject b)
        {
            if (a is VMNumber numA && b is VMNumber numB)
            {
                return new VMNumber(numA.Value + numB.Value);
            }
            if (a is VMString strA && b is VMString strB)
            {
                return new VMString(strA.Value + strB.Value);
            }
            throw new ArithmeticException($"Cannot add {a.ToString()} and {b.ToString()}.");
        }
    }
}
