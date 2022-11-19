namespace RiverScript.VM
{
    internal class VMObject
    {
        public override string ToString()
        {
            return "<VMObject>";
        }

        internal object Clone()
        {
            return MemberwiseClone();
        }

        internal virtual bool IsTruthy()
        {
            return true;
        }

        internal virtual bool PassByReference()
        {
            return false;
        }
    }
}
