using RiverScript.Tokens;
using System;
using System.Collections.Generic;

namespace RiverScript
{
    internal class Lexer
    {
        // Lexer state.
        private Tokens.Block rootToken;
        private Token current;
        private List<Token> stack;
        private string sequence;

        private string[] keywords = new string[]
        {
            "if",
            "else",
            "while",
            "true",
            "false",
            "null",
            "def",
            "for",
            "in"
        };

        // Operators should be longest to shortest here.
        // Excludes alphabetical operators. (i.e. 'and', 'or'.)
        private string[] operators = new string[]
        {
            "==",
            "!=",
            ">=",
            "<=",
            "..",
            ">",
            "<",
            "+",
            "-",
            "*",
            "/",
            "%"
        };

        private Token GetSequenceToken(string sequence)
        {
            foreach (var key in keywords)
            {
                if (key == sequence)
                {
                    return new Keyword(key);
                }
            }
            switch (sequence)
            {
                case ",":
                    return new Tokens.Comma();
                case "==":
                    return new Tokens.EqualsOperator();
                case "!=":
                    return new Tokens.NotEqualsOperator();
                case ">=":
                    return new Tokens.GreaterEqualOperator();
                case "<=":
                    return new Tokens.LessEqualOperator();
                case "..":
                    return new Tokens.RangeOperator();
                case ">":
                    return new Tokens.GreaterOperator();
                case "<":
                    return new Tokens.LessOperator();
                case "+":
                    return new Tokens.AdditionOperator();
                case "-":
                    return new Tokens.SubtractionOperator();
                case "*":
                    return new Tokens.MultiplicationOperator();
                case "/":
                    return new Tokens.DivisionOperator();
                case "%":
                    return new Tokens.RemainderOperator();
                case "and":
                    return new Tokens.AndOperator();
                case "or":
                    return new Tokens.OrOperator();
                case "not":
                    return new Tokens.NotOperator();
            }
            if (double.TryParse(sequence, out double n))
            {
                return new Tokens.NumberLiteral(n);
            }
            return new Tokens.Identifier(sequence);
        }

        private void GoUp()
        {
            if (current is Tokens.ContainerToken container)
            {
                if (current.Parent == null)
                {
                    throw new Exception("Mismatched braces.");
                }
                current = current.Parent;
            }
            else
            {
                current = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
            }
        }

        private void Nest(Token newToken)
        {
            if (current is Tokens.ContainerToken currentContainer)
            {
                currentContainer.AddToken(newToken);
                if (newToken is not Tokens.ContainerToken)
                {
                    stack.Add(current);
                }
                current = newToken;
            }
            else
            {
                throw new Exception("Cannot nest within non-container tokens.");
            }
        }

        private void EndSequence()
        {
            if (!string.IsNullOrWhiteSpace(sequence) && current is ContainerToken containerToken)
            {
                Token token = GetSequenceToken(sequence);
                ((ContainerToken)current).AddToken(token);
            }
            sequence = string.Empty;
        }

        private void CloseAssignment()
        {
            if (current is Tokens.Assignment)
            {
                GoUp();
            }
        }

        internal Tokens.Block Lex(string source)
        {
            rootToken = new Tokens.Block();
            current = rootToken;
            stack = new();
            sequence = string.Empty;

            bool inComment = false;
            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                char nextChar = '\0';
                if (i < source.Length - 1)
                {
                    nextChar = source[i + 1];
                }

                if (inComment)
                {
                    if (c == '\n' || c == '\r')
                    {
                        inComment = false;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (current is not Tokens.StringLiteral)
                {
                    string substr = source.Substring(i);
                    bool shouldContinue = false;
                    foreach (string @operator in operators)
                    {
                        if (substr.StartsWith(@operator))
                        {
                            EndSequence();
                            sequence += @operator;
                            EndSequence();
                            i += @operator.Length - 1;
                            shouldContinue = true;
                            break;
                        }
                    }
                    if (shouldContinue)
                    {
                        continue;
                    }
                }

                switch (c)
                {
                    case '#' when current is not Tokens.StringLiteral:
                        inComment = true;
                        break;
                    case '=' when current is not Tokens.StringLiteral && current is not Tokens.Parentheses:
                        EndSequence();
                        Nest(new Tokens.Assignment());
                        break;
                    case ',' when current is not Tokens.StringLiteral:
                        EndSequence();
                        sequence += c;
                        EndSequence();
                        break;
                    case '(' when current is not Tokens.StringLiteral:
                        EndSequence();
                        Nest(new Tokens.Parentheses());
                        break;
                    case ')' when current is not Tokens.StringLiteral:
                        EndSequence();
                        if (current is Tokens.Parentheses)
                        {
                            GoUp();
                        }
                        else
                        {
                            throw new Exception("Cannot close parentheses outside of parentheses.");
                        }
                        break;
                    case '{' when current is not Tokens.StringLiteral:
                        EndSequence();
                        Nest(new Tokens.Block());
                        break;
                    case '}' when current is not Tokens.StringLiteral:
                        EndSequence();
                        CloseAssignment();
                        if (current is Tokens.Block)
                        {
                            GoUp();
                        }
                        else
                        {
                            throw new Exception("Too many closing braces.");
                        }
                        break;
                    case '"':
                        if (current is Tokens.StringLiteral stringLiteral)
                        {
                            stringLiteral.Value = sequence;
                            sequence = string.Empty;
                            GoUp();
                        }
                        else if (current is Tokens.ContainerToken containerToken)
                        {
                            EndSequence();
                            Nest(new Tokens.StringLiteral());
                        }
                        break;
                    case char whitespace when (char.IsWhiteSpace(c) && current is Tokens.ContainerToken containerToken1):
                        EndSequence();
                        if (c == '\n' || c == '\r')
                        {
                            CloseAssignment();
                        }
                        break;
                    default:
                        sequence += c;
                        break;
                }
            }

            EndSequence();
            CloseAssignment();

            if (current != rootToken)
            {
                throw new Exception($"Unclosed {current.ToString()}.");
            }

            return rootToken;
        }
    }
}
