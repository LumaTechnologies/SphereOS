using System.Collections.Generic;

namespace RiverScript.Tokens
{
    public class ContainerToken : Token
    {
        private readonly List<Token> _tokens = new();

        public List<Token> Tokens
        {
            get
            {
                return _tokens;
            }
        }

        public void AddToken(Token token)
        {
            _tokens.Add(token);
            token.Parent = this;
        }

        public override string ToString()
        {
            return "Container";
        }

        /*private void Debug(Token token, int indent)
        {
            Console.Write(new string('\t', indent));
            Console.WriteLine(token.GetType().Name + ": " + token.ToString());
            if (token is ContainerToken containerToken)
            {
                foreach (Token innerToken in containerToken.Tokens)
                {
                    Debug(innerToken, indent + 1);
                }
            }
        }

        internal void Debug()
        {
            Debug(this, 0);
        }*/
    }
}
