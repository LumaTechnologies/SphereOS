namespace RiverScript.VM
{
    internal class VMRange : VMObject
    {
        internal VMRange(double lower, double upper)
        {
            if (upper > lower)
            {
                Lower = lower;
                Upper = upper;
            }
            else
            {
                Lower = upper;
                Upper = lower;
            }
        }

        public override string ToString()
        {
            return $"<Range {Lower.ToString()}..{Upper.ToString()}>";
        }

        internal double Lower { get; private set; }
        internal double Upper { get; private set; }
    }
}
