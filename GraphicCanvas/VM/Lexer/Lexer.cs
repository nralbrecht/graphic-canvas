using System;
using System.Collections.Generic;

using System.Drawing;
using GraphicCanvas.Graphic;

namespace GraphicCanvas.VM
{
    class Lexer
    {
        private int start = 0;
        private int current = 0;
        private int line = 1;
        private string source;
        private List<Token> tokens = new List<Token>();
        private Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>();

        public Lexer(string source)
        {
            this.source = source;
            
            keywords.Add("var", TokenType.VAR);
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case ',': AddToken(TokenType.COMMA); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '=': AddToken(TokenType.EQUAL); break;
                case '#':
                    if (IsHexadecimal(Peek()))
                    {
                        Color();
                    }
                    else
                    {
                        // A comment goes until the end of the line.
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    line++;
                    break;
                case '"': String(); break;
                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        VirtualMachine.Error(line, "Unexpected character.");
                    }
                    break;
            }
        }

        #region Controlls

        private bool IsAtEnd()
        {
            return current >= source.Length;
        }

        private char Advance()
        {
            current++;
            return source[current - 1];
        }

        private char Peek()
        {
            if (current >= source.Length) return '\0';
            return source[current];
        }

        private char PeekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }

        #endregion

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal)
        {
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }

        #region Group checks

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                    c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private bool IsHexadecimal(char c)
        {
            return IsDigit(c) ||
                   (c >= 'a' && c <= 'f') ||
                   (c >= 'A' && c <= 'F');
        }

        #endregion

        #region Literals

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            // See if the identifier is a reserved word.
            string text = source.Substring(start, current - start);
            TokenType type;

            if (keywords.ContainsKey(text))
            {
                type = keywords[text];
            }
            else
            {
                type = TokenType.IDENTIFIER;
            }
            
            AddToken(type);
        }

        private void Color()
        {
            while (IsHexadecimal(Peek())) Advance();

            try
            {
                Color color = ColorHelper.FromHEX(source.Substring(start, current - start));
                AddToken(TokenType.COLOR, color);
            }
            catch
            {
                VirtualMachine.Error(line, "Invalid Color.");
            }
        }

        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            // Look for a fractional part.
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume the "."
                Advance();

                while (IsDigit(Peek())) Advance();
            }

            AddToken(TokenType.NUMBER,
                Convert.ToInt32(source.Substring(start, current - start)));
        }

        private void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') line++;
                Advance();
            }

            // Unterminated string.
            if (IsAtEnd())
            {
                VirtualMachine.Error(line, "Unterminated string.");
                return;
            }

            // The closing ".
            Advance();

            // Trim the surrounding quotes.
            string value = source.Substring(start + 1, current - start - 2);
            AddToken(TokenType.STRING, value);
        }

        #endregion
    }
}
