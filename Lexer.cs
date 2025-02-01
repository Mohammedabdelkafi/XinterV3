using System;
using System.Collections.Generic;

namespace XinterV3
{
    class Lexer
    {
        private string src;
        private List<Token> tokens;
        private int pos;
        private char? currChar;
        private string digits = "1234567890";
        private string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";

        public Lexer(string text)
        {
            this.src = text;
            this.tokens = new List<Token>();
            this.pos = -1;
            Advance();
            Tokenize();
        }

        private void Advance()
        {
            this.currChar = ++this.pos < this.src.Length ? this.src[this.pos] : (char?)null;
        }

        private void Tokenize()
        {
            var tokenMap = new Dictionary<char, string>
            {
                { '+', "PLUS" },
                { '-', "MINUS" },
                { '*', "MULTIPLY" },
                { '/', "DIVIDE" },
                { '=', "EQUALS" },
                { '(', "LPAREN" },
                { ')', "RPAREN" }
            };

            while (this.currChar != null)
            {
                if (tokenMap.ContainsKey((char)this.currChar))
                {
                    this.tokens.Add(new Token(tokenMap[(char)this.currChar], this.currChar.ToString()));
                    Advance();
                }
                else if (this.currChar == ' ')
                {
                    Advance(); // Skip whitespace
                }
                else if (this.digits.Contains(this.currChar.ToString()))
                {
                    ParseNum();
                }
                else if (this.letters.Contains(this.currChar.ToString()))
                {
                    ParseVar();
                }
                else
                {
                    throw new Exception("Unexpected character: " + this.currChar);
                }
            }
        }

        private void ParseNum()
        {
            string numStr = "";
            while (this.currChar != null && this.digits.Contains(this.currChar.ToString()))
            {
                numStr += this.currChar;
                Advance();
            }
            this.tokens.Add(new Token("NUMBER", numStr));
        }

        private void ParseVar()
        {
            string varStr = "";
            while (this.currChar != null && (this.letters + this.digits).Contains(this.currChar.ToString()))
            {
                varStr += this.currChar;
                Advance();
            }
            this.tokens.Add(new Token("IDENTIFIER", varStr));
        }

        public List<Token> GetTokens()
        {
            return this.tokens;
        }
    }
}
