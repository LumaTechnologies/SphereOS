namespace RiverScript.Tokens
{
    public class StringLiteral : Token
    {
        public override string ToString()
        {
            return Value;
        }

        public string Value { get; set; } = string.Empty;
    }
}
