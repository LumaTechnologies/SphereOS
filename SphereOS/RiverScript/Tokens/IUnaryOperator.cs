using RiverScript.VM;

namespace RiverScript.Tokens
{
    internal interface IUnaryOperator
    {
        internal VMObject OperateUnary(VMObject x);
    }
}
