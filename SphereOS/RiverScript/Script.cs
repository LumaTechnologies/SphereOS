namespace RiverScript
{
    public class Script
    {
        public Script(string source)
        {
            Source = source;
        }

        public void Lex()
        {
            Lexer lexer = new Lexer();

            RootToken = lexer.Lex(Source);
        }

        public string Source { get; private set; }

        public Tokens.Block RootToken { get; private set; }
    }
}
