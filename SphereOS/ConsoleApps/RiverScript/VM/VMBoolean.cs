namespace RiverScript.VM
{
    internal class VMBoolean : VMObject
    {
        internal VMBoolean(bool value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        internal override bool IsTruthy()
        {
            return Value;
        }

        internal bool Value { get; private set; }
    }
}
