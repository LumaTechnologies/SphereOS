using RiverScript.Tokens;
using System;
using System.Collections.Generic;

namespace RiverScript.VM
{
    internal class Interpreter
    {
        internal List<(string VariableName, VMObject Object, Block Scope)> Variables = new List<(string VariableName, VMObject Object, Block Scope)>();

        internal VMObject InterpretExpression(List<Token> tokens, Block scope)
        {
            if (tokens.Count == 0)
                return new VMNull();

            bool expectingOperator = false;
            bool expectingFunctionParentheses = false;
            string functionName = null;

            // The arrays will be laid out like so
            // for the expression 1+2+3:
            // objects: [1, 2, 3]
            // operators [+, +]

            // For unary operators, the left-hand side will
            // be null,for example the expression -5:
            // objects: [null, -5]
            // operators: [-]

            List<Operator> operators = new List<Operator>();
            List<VMObject> objects = new List<VMObject>();

            for (int i = 0; i < tokens.Count; i++)
            {
                Token token = tokens[i];
                Token nextToken = null;
                if (i < tokens.Count - 1)
                {
                    nextToken = tokens[i + 1];
                }
                if (!expectingFunctionParentheses && token is Operator @operator)
                {
                    if (i == tokens.Count - 1)
                    {
                        throw new Exception("Unexpected end of expression.");
                    }
                    if (!expectingOperator)
                    {
                        // The operator is unary, add null for the left-hand side.
                        objects.Add(null);
                    }
                    operators.Add(@operator);
                }
                else if (token is Parentheses parentheses)
                {
                    if (expectingFunctionParentheses)
                    {
                        List<VMObject> arguments = InterpretFunctionArguments(parentheses, scope);
                        objects.Add(CallFunctionByName(functionName!, arguments, scope));
                        expectingOperator = false;
                        expectingFunctionParentheses = false;
                    }
                    else
                    {
                        VMObject obj = InterpretExpression(parentheses, scope);
                        if (obj != null)
                            objects.Add(obj);
                    }
                }
                else
                {
                    switch (token)
                    {
                        case Keyword keyword:
                            switch (keyword.Name)
                            {
                                case "true":
                                    objects.Add(new VMBoolean(true));
                                    break;
                                case "false":
                                    objects.Add(new VMBoolean(false));
                                    break;
                                case "null":
                                    objects.Add(new VMNull());
                                    break;
                                default:
                                    throw new Exception($"Unexpected keyword '{keyword.Name}'.");
                            }
                            break;
                        case NumberLiteral numberLiteral:
                            objects.Add(new VMNumber(numberLiteral.Value));
                            break;
                        case StringLiteral stringLiteral:
                            objects.Add(new VMString(stringLiteral.Value));
                            break;
                        case Identifier identifier:
                            if (nextToken is Parentheses)
                            {
                                expectingFunctionParentheses = true;
                                functionName = identifier.Value;
                            }
                            else
                            {
                                objects.Add(GetVariableByName(identifier.Value, scope));
                            }
                            break;
                        default:
                            throw new Exception($"Unexpected {token.ToString()} in an expression.");
                    }
                }
                expectingOperator = !expectingOperator;
            }

            while (operators.Count > 0)
            {
                // To-do: BODMAS, optimisation.
                Operator @operator = operators[0];
                VMObject result;
                if (objects[objects.Count - 2] == null)
                {
                    if (@operator is IUnaryOperator unary)
                        result = unary.OperateUnary(objects[objects.Count - 1]!);
                    else
                        throw new Exception($"{@operator.ToString()} is not a unary operator.");
                }
                else
                {
                    result = @operator.Operate(objects[objects.Count - 2]!, objects[objects.Count - 1]!);
                }
                objects.RemoveAt(objects.Count - 1);
                objects.RemoveAt(objects.Count - 1);
                operators.RemoveAt(operators.Count - 1);
                objects.Add(result);
            }
            //instructions.Add(new CallFunction(identifier.Value));

            VMObject returnObj = (VMObject)objects[0];
            if (returnObj!.PassByReference())
                return returnObj;
            else
                return (VMObject)returnObj.Clone();
        }

        internal VMObject InterpretExpression(ContainerToken container, Block scope)
        {
            return InterpretExpression(container.Tokens, scope);
        }

        internal VMObject GetVariableByName(string name, Block scope)
        {
            //Console.WriteLine("getv " + name);
            foreach ((string VariableName, VMObject Object, Block Scope) variable in Variables)
            {
                if (variable.VariableName == name)
                {
                    if (variable.Scope == null || scope == null || scope == variable.Scope)
                        return variable.Object;
                    if (scope.IsDescendantOf(variable.Scope))
                        return variable.Object;
                }
            }
            throw new Exception($"Unknown variable {name}.");
        }

        internal void DefineVariable(string name, VMObject obj, Block scope)
        {
            //Console.WriteLine("defv " + name);
            for (int i = 0; i < Variables.Count; i++)
            {
                var item = Variables[i];
                bool inScope = item.Scope == scope || scope == null || item.Scope == null || scope.IsDescendantOf(item.Scope);
                if (item.VariableName == name && inScope)
                {
                    // Update the variable.
                    Variables[i] = (name, obj, item.Scope);
                    return;
                }
            }
            // Create the variable.
            Variables.Add((name, obj, scope));
            //Console.WriteLine($"VarDef {name} {obj}");
        }

        internal VMObject CallFunctionByName(string name, List<VMObject> arguments, Block scope)
        {
            VMObject obj = GetVariableByName(name, scope);
            if (obj is VMBaseFunction baseFunction)
            {
                if (baseFunction.Arguments.Count != arguments.Count)
                {
                    throw new Exception($"Function {name} expected {baseFunction.Arguments.Count} argument(s), but got {arguments.Count}.");
                }
                if (obj is VMFunction function)
                {
                    InterpretFunction(function, arguments);
                    return new VMNull();
                }
                else if (obj is VMNativeFunction nativeFunction)
                {
                    return nativeFunction.Func.Invoke(arguments);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new Exception($"Cannot call {obj.ToString()} like a function.");
            }
        }

        internal List<VMObject> InterpretFunctionArguments(Parentheses parentheses, Block scope)
        {
            List<Token> tokenStack = new List<Token>();
            List<VMObject> result = new List<VMObject>();
            for (int i = 0; i < parentheses.Tokens.Count; i++)
            {
                Token token = parentheses.Tokens[i];
                if (token is not Tokens.Comma)
                {
                    tokenStack.Add(token);
                }
                if (token is Tokens.Comma || i == parentheses.Tokens.Count - 1)
                {
                    if (tokenStack.Count == 0 && token is Tokens.Comma)
                    {
                        throw new Exception("Unexpected comma in function arguments.");
                    }
                    VMObject obj = InterpretExpression(tokenStack, scope);
                    tokenStack.Clear();
                    if (obj != null)
                        result.Add(obj);
                }
            }
            return result;
        }

        internal List<string> InterpretFunctionDefinitionArguments(Parentheses parentheses)
        {
            List<string> result = new List<string>();
            bool expectingComma = false;
            for (int i = 0; i < parentheses.Tokens.Count; i++)
            {
                Token token = parentheses.Tokens[i];
                if (token is Tokens.Comma)
                {
                    if (!expectingComma)
                        throw new Exception($"Expected an argument name in a function's parentheses, but got {token.ToString()}.");
                }
                else if (token is Tokens.Identifier identifier)
                {
                    if (expectingComma)
                        throw new Exception($"Expected a comma in a function's parentheses, but got {token.ToString()}.");

                    result.Add(identifier.Value);
                }
                else
                {
                    throw new Exception($"Unexpected {token.ToString()} in a function's arguments.");
                }
                expectingComma = !expectingComma;
            }
            return result;
        }

        private Token TokenAt(int index, Block block)
        {
            if (index >= block.Tokens.Count) return null;

            return block.Tokens[index];
        }

        internal VMObject InterpretBlock(Block block)
        {

            for (int i = 0; i < block.Tokens.Count; i++)
            {
                Token token = block.Tokens[i];

                Token nextToken = TokenAt(i + 1, block);

                switch (token)
                {
                    case Keyword keyword:
                        if (keyword.Name == "for")
                        {
                            if (nextToken is not Parentheses forParentheses)
                                throw new Exception("Expected parentheses to follow 'for'.");

                            if (TokenAt(i + 2, block) is not Block forBlock)
                                throw new Exception("Expected block to follow 'for (...)'.");

                            List<Token> parenthesesTokens = forParentheses.Tokens;
                            if (parenthesesTokens.Count < 3)
                                throw new Exception("The parentheses of a for loop expect at least 3 tokens.");

                            if (parenthesesTokens[0] is not Identifier forIdentifier)
                                throw new Exception("Expected an identifier as the first item in the parentheses of a for loop.");

                            if (parenthesesTokens[1] is not Keyword || ((Keyword)parenthesesTokens[1]).Name != "in")
                                throw new Exception("Expected keyword 'in' as the second item in the parentheses of a for loop.");

                            List<Token> exprTokens = new();
                            for (int j = 2; j < parenthesesTokens.Count; j++)
                            {
                                exprTokens.Add(parenthesesTokens[j]);
                            }
                            VMObject obj = InterpretExpression(exprTokens, block);
                            if (obj is not VMRange forRange)
                                throw new Exception("Expected a range as the third item in the parentheses of a for loop.");

                            DefineVariable(forIdentifier.Value, new VMNumber(forRange.Lower), forBlock);

                            VMObject index = GetVariableByName(forIdentifier.Value, forBlock);
                            if (index is not VMNumber indexNum)
                                throw new Exception("Index of a for loop is not a number.");

                            if (indexNum.Value < forRange.Upper)
                            {
                                while (true)
                                {
                                    InterpretBlock(forBlock);

                                    VMObject index1 = GetVariableByName(forIdentifier.Value, forBlock);
                                    if (index1 is not VMNumber indexNum1)
                                        throw new Exception("Index of a for loop is not a number.");
                                    if (indexNum1.Value >= forRange.Upper)
                                        break;
                                    DefineVariable(forIdentifier.Value, new VMNumber(indexNum1.Value + 1), forBlock);

#if SPHEREOS
                                    SphereOS.Core.ProcessManager.Yield();
#endif
                                }
                            }

                            i += 2;

                            break;
                        }
                        else if (keyword.Name == "def")
                        {
                            if (nextToken is not Identifier defIdentifier)
                                throw new Exception("Expected identifier to follow 'def'.");

                            if (TokenAt(i + 2, block) is not Parentheses defParentheses)
                                throw new Exception("Expected expression in parentheses to follow 'def ...'.");

                            if (TokenAt(i + 3, block) is not Block defBlock)
                                throw new Exception("Expected block to follow 'def ... (...)'.");

                            List<string> arguments = InterpretFunctionDefinitionArguments(defParentheses);

                            VMFunction function = new VMFunction(defBlock, arguments);

                            Block newFuncScope;
                            if (block.Parent == null)
                            {
                                // Functions at the root of the script have no scope.
                                newFuncScope = null;
                            }
                            else
                            {
                                newFuncScope = block;
                            }
                            DefineVariable(defIdentifier.Value, function, newFuncScope);

                            i += 3;
                        }
                        else if (keyword.Name == "if")
                        {
                            if (nextToken is not Parentheses ifParentheses)
                                throw new Exception("Expected expression in parentheses to follow 'if'.");

                            if (TokenAt(i + 2, block) is not Block ifBlock)
                                throw new Exception("Expected block to follow 'if (...)'.");

                            Block elseBlock = null;

                            if (TokenAt(i + 3, block) is Keyword keyword1)
                            {
                                if (keyword1.Name == "else")
                                {
                                    if (TokenAt(i + 4, block) is not Block)
                                        throw new Exception("Expected block to follow if ... else.");

                                    elseBlock = (Block)TokenAt(i + 4, block);
                                }
                            }

                            VMObject obj = InterpretExpression(ifParentheses, block);
                            if (obj != null && obj.IsTruthy())
                            {
                                InterpretBlock(ifBlock);
                            }
                            else if (elseBlock != null)
                            {
                                InterpretBlock(elseBlock);
                            }

                            if (elseBlock != null)
                                i += 4;
                            else
                                i += 2;

                            break;
                        }
                        else if (keyword.Name == "while")
                        {
                            if (nextToken is not Parentheses whileParentheses)
                                throw new Exception("Expected expression in parentheses to follow 'while'.");

                            if (TokenAt(i + 2, block) is not Block whileBlock)
                                throw new Exception("Expected block to follow 'while (...)'.");

                            while (true)
                            {
                                VMObject obj = InterpretExpression(whileParentheses, block);

                                if (obj == null || !obj.IsTruthy())
                                    break;

                                InterpretBlock(whileBlock);

#if SPHEREOS
                                SphereOS.Core.ProcessManager.Yield();
#endif
                            }

                            i += 2;

                            break;
                        }
                        else
                        {
                            throw new Exception($"Unexpected keyword '{keyword.Name}'.");
                        }
                        break;
                    case Identifier identifier:
                        if (nextToken is Parentheses nextParentheses)
                        {
                            List<VMObject> result = InterpretFunctionArguments(nextParentheses, block);
                            CallFunctionByName(identifier.Value, result, block);

                            // Skip the parentheses.
                            i++;
                        }
                        else if (nextToken is Assignment assignment)
                        {
                            if (assignment.Tokens.Count == 0)
                            {
                                throw new Exception($"Expected expression to assign variable '{identifier.Value}'.");
                            }

                            Block newVarScope;
                            if (block.Parent == null)
                            {
                                // Variables at the root of the script (i.e. global variables) have no scope.
                                newVarScope = null;
                            }
                            else
                            {
                                newVarScope = block;
                            }

                            DefineVariable(identifier.Value, InterpretExpression(assignment, block), newVarScope);

                            i++;
                        }
                        else
                        {
                            throw new Exception($"Expected parentheses or an assignment to follow '{token.ToString()}'.");
                        }
                        break;
                    default:
                        throw new Exception($"Unexpected {token.ToString()}.");
                }
            }

            return new VMNull();
        }

        internal VMObject InterpretFunction(VMFunction function, List<VMObject> arguments)
        {
            Block root = function.Root;

            for (int i = 0; i < arguments.Count; i++)
            {
                DefineVariable(function.Arguments[i], arguments[i], root);
            }

            return InterpretBlock(root);
        }

        internal VMObject InterpretScript(Script script)
        {
            return InterpretFunction(VMFunction.FromScript(script), new List<VMObject>());
        }
    }
}
