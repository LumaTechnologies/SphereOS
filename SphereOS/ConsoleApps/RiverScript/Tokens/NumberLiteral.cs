namespace RiverScript.Tokens
{
    public class NumberLiteral : Token
    {
        public NumberLiteral(double value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public double Value { get; set; }
    }
}
