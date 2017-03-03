using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

namespace GraphicCanvas.VM
{
    class Parser
    {
        private int current = 0;
        private List<Token> tokens;
        private List<byte> bytecode;
        private Dictionary<string, int> varLookup;
        private Dictionary<string, Instruction> funLookup;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            bytecode = new List<byte>();
            varLookup = new Dictionary<string, int>();

            funLookup = new Dictionary<string, Instruction>();
            funLookup.Add("fill", Instruction.FILL);
            funLookup.Add("setpixel", Instruction.SET_PIXEL);
            funLookup.Add("setrect", Instruction.SET_RECT);
            funLookup.Add("write", Instruction.WRITE_TEXT);
            funLookup.Add("rgb", Instruction.RGB);
            funLookup.Add("cmyk", Instruction.CMYK);
            funLookup.Add("hsv", Instruction.HSV);
            funLookup.Add("lab", Instruction.LAB);
            funLookup.Add("getpixel", Instruction.GET_PIXEL);
            funLookup.Add("getnoise", Instruction.GET_NOISE);
            funLookup.Add("getwidth", Instruction.GET_WIDTH);
            funLookup.Add("getheight", Instruction.GET_HEIGHT);
            funLookup.Add("getrand", Instruction.GET_RAND);
        }

        public byte[] ParseTokens()
        {
            OptimiseTokens();

            while (!IsAtEnd())
            {
                ParseToken();
            }

            OptimiseBytecode();

            return bytecode.ToArray();
        }

        private void ParseToken()
        {
            Token t = Advance();

            switch (t.type)
            {
                case TokenType.VAR: Variable(); break;
                case TokenType.IDENTIFIER: FunctionCall(t.lexeme); break;
                default: break;
            }
        }
        
        #region Code Generators

        private void AddByte(Instruction instruction)
        {
            bytecode.Add((byte)instruction);
        }

        private void AddByte(params byte[] bytes)
        {
            bytecode.AddRange(bytes);
        }

        private void AddLiteral(TokenType type, int line, object literal)
        {
            if (!IsLiteral(type))
            {
                VirtualMachine.Error(line, "Unexpected Token.");
            }
                        
            switch (type)
            {
                case TokenType.STRING:
                    AddByte(Instruction.STRING_LITERAL);
                    string text = (string)literal;

                    if (Encoding.ASCII.GetByteCount(text) > 256)
                    {
                        VirtualMachine.Error(line, "String is to long(max 256).");
                    }

                    AddByte((byte)Encoding.ASCII.GetByteCount(text));
                    AddByte(Encoding.ASCII.GetBytes(text));
                    break;

                case TokenType.NUMBER:
                    AddByte(Instruction.INTEGER_LITERAL);
                    AddByte(BitConverter.GetBytes((int)literal));
                    break;

                case TokenType.COLOR:
                    AddByte(Instruction.COLOR_LITERAL);
                    Color c = (Color)literal;
                    AddByte(c.A, c.R, c.G, c.B);
                    break;
                default:
                    break;
            }
        }

        private void AddLiteral(Token token)
        {
            AddLiteral(token.type, token.line, token.literal);
        }
        
        private void Variable()
        {
            Token identifier = Advance();
            if (identifier.type != TokenType.IDENTIFIER || Peek().type != TokenType.EQUAL)
            {
                VirtualMachine.Error(identifier.line, "Unexpected token.");
                return;
            }
            Advance();
            
            Token literal = Advance();

            if (literal.line != identifier.line)
            {
                VirtualMachine.Error(identifier.line, "Invalid assignment.");
                return;
            }

            if (literal.type == TokenType.IDENTIFIER && Peek().type == TokenType.LEFT_PAREN)
            {
                int randId = new SecureRandom().Next();
                varLookup.Add(identifier.lexeme, randId);

                AddLiteral(TokenType.NUMBER, literal.line, randId);
                FunctionCall(literal.lexeme);
                AddByte(Instruction.WRITE);
            }
            else if (IsLiteral(literal.type))
            {
                int randId = new SecureRandom().Next();
                varLookup.Add(identifier.lexeme, randId);

                AddLiteral(TokenType.NUMBER, literal.line, randId);
                AddLiteral(literal);
                AddByte(Instruction.WRITE);
            }
            else
            {
                VirtualMachine.Error(identifier.line, "Invalid assignment.");
                return;
            }
        }

        private void FunctionCall(string lexme)
        {
            Token curr = Advance();

            if (curr.type != TokenType.LEFT_PAREN)
            {
                VirtualMachine.Error(curr.line, "Unexpected Token.");
                return;
            }

            if (Peek().type == TokenType.RIGHT_PAREN)
            {
                // without parameter

                Advance();
                if (funLookup.ContainsKey(lexme.ToLower()))
                {
                    AddByte(funLookup[lexme.ToLower()]);
                }
                else
                {
                    VirtualMachine.Error(curr.line, "The function '" + lexme + "' ist not defined.");
                }
            }
            else if (IsParameter(Peek().type))
            {
                // with parameter
                
                curr = Advance();

                while (curr.type != TokenType.RIGHT_PAREN)
                {
                    if (IsLiteral(curr.type))
                    {
                        // literal
                        AddLiteral(curr);
                    }
                    else if (curr.type == TokenType.IDENTIFIER && Peek().type != TokenType.LEFT_PAREN)
                    {
                        // variable
                        if (!varLookup.ContainsKey(curr.lexeme))
                        {
                            VirtualMachine.Error(curr.line, "Uninitalised Variable.");
                            return;
                        }

                        AddLiteral(TokenType.NUMBER, curr.line, varLookup[curr.lexeme]);
                        AddByte(Instruction.READ);
                    }
                    else if (curr.type == TokenType.IDENTIFIER && Peek().type == TokenType.LEFT_PAREN)
                    {
                        // function call
                        FunctionCall(curr.lexeme);
                    }
                    else if (curr.type == TokenType.COMMA) { }
                    else
                    {
                        VirtualMachine.Error(curr.line, "Unexpected Token.");
                        return;
                    }

                    curr = Advance();
                }

                if (funLookup.ContainsKey(lexme.ToLower()))
                {
                    AddByte(funLookup[lexme.ToLower()]);
                }
                else
                {
                    VirtualMachine.Error(curr.line, "The function '" + lexme + "' ist not defined.");
                }
            }
            else
            {
                VirtualMachine.Error(curr.line, "Unexpected Token.");
                return;
            }
        }

        #endregion

        #region Optimisations

        private void OptimiseBytecode()
        {
            // dup stack if same literal two time after each other
        }

        private void OptimiseTokens()
        {
            // find unused variables
            // move variable declarations to beginning
            // preprocess colors
        }

        #endregion

        #region Controlls

        private bool IsAtEnd()
        {
            return current >= tokens.Count;
        }

        private Token Advance()
        {
            current++;
            return tokens[current - 1];
        }

        private Token Peek()
        {
            if (current >= tokens.Count) return new Token();
            return tokens[current];
        }

        private Token PeekOver()
        {
            if (current + 1 >= tokens.Count) return new Token();
            return tokens[current + 1];
        }

        #endregion

        #region Group Lookups

        private bool IsLiteral(TokenType t)
        {
            return t == TokenType.NUMBER ||
                   t == TokenType.COLOR ||
                   t == TokenType.STRING;
        }

        private bool IsParameter(TokenType t)
        {
            return IsLiteral(t) || t == TokenType.IDENTIFIER;
        }

        #endregion
    }
}
