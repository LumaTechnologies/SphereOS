using RiverScript.VM;
using System;

namespace RiverScript.Tokens
{
    public class NotOperator : Operator, IUnaryOperator
    {
        public override string ToString()
        {
            return "not";
        }

        VMObject IUnaryOperator.OperateUnary(VMObject x)
        {
            return new VMBoolean(!x.IsTruthy());
        }

        internal override VMObject Operate(VMObject a, VMObject b)
        {
            throw new Exception("'not' cannot be used as a binary operator.");
        }
    }
}
