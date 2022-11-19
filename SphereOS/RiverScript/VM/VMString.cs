namespace RiverScript.VM
{
    internal class VMString : VMObject
    {
        internal VMString(string value)
        {
            //Console.WriteLine("New VMString " + value);
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        internal string Value { get; private set; }
    }
}
