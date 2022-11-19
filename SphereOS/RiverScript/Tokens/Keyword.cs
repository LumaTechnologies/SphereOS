namespace RiverScript.Tokens
{
    internal class Keyword : Token
    {
        internal Keyword(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        internal string Name { get; private set; }
    }
}
