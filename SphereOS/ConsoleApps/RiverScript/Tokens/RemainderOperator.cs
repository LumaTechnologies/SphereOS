using RiverScript.VM;
using System;

namespace RiverScript.Tokens
{
    public class RemainderOperator : Operator
    {
        public override string ToString()
        {
            return "%";
        }

        internal override VMObject Operate(VMObject a, VMObject b)
        {
            if (a is VMNumber numA && b is VMNumber numB)
            {
                return new VMNumber(numA.Value % numB.Value);
            }
            throw new Exception($"Cannot find the remainder of {a.ToString()} and {b.ToString()}.");
        }
    }
}
