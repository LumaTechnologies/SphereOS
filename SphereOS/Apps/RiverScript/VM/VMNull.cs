namespace RiverScript.VM
{
    internal class VMNull : VMObject
    {
        internal VMNull()
        {
            //Console.WriteLine("New VMNull");
        }

        public override string ToString()
        {
            return "null";
        }

        internal override bool IsTruthy()
        {
            return false;
        }
    }
}
