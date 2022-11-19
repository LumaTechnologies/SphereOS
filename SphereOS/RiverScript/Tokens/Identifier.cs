namespace RiverScript.Tokens
{
    public class Identifier : Token
    {
        public Identifier(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public string Value { get; set; }
    }
}
