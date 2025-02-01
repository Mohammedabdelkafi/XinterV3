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
        private bool debugMode;

        public Lexer(string text, bool debugMode = false)
        {
            this.src = text;
            this.tokens = new List<Token>();
            this.pos = -1;
            this.debugMode = debugMode;
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
                if (char.IsWhiteSpace((char)this.currChar))
                {
                    Advance(); // Skip whitespace
                }
                else if (char.IsDigit((char)this.currChar))
                {
                    ParseNum();
                }
                else if (char.IsLetter((char)this.currChar))
                {
                    ParseVarOrBool();
                }
                else
                {
                    ParseOperator(tokenMap);
                }
            }

            if (debugMode)
            {
                Console.WriteLine("Tokens:");
                foreach (var token in tokens)
                {
                    Console.WriteLine(token);
                }
            }
        }

        private void ParseNum()
        {
            string numStr = "";
            while (this.currChar != null && char.IsDigit((char)this.currChar))
            {
                numStr += this.currChar;
                Advance();
            }
            this.tokens.Add(new Token("NUMBER", numStr));
        }

        private void ParseVarOrBool()
        {
            string varStr = "";
            while (this.currChar != null && (char.IsLetterOrDigit((char)this.currChar) || this.currChar == '_'))
            {
                varStr += this.currChar;
                Advance();
            }

            if (varStr == "true" || varStr == "false")
            {
                this.tokens.Add(new Token("BOOLEAN", varStr));
            }
            else
            {
                this.tokens.Add(new Token("IDENTIFIER", varStr));
            }
        }

        private void ParseOperator(Dictionary<char, string> tokenMap)
        {
            char curr = (char)this.currChar;
            Advance();
            if (this.currChar != null)
            {
                string twoCharOp = curr.ToString() + this.currChar.ToString();

                switch (twoCharOp)
                {
                    case "==":
                        this.tokens.Add(new Token("EQ", twoCharOp));
                        Advance();
                        return;
                    case "!=":
                        this.tokens.Add(new Token("NEQ", twoCharOp));
                        Advance();
                        return;
                    case "<=":
                        this.tokens.Add(new Token("LE", twoCharOp));
                        Advance();
                        return;
                    case ">=":
                        this.tokens.Add(new Token("GE", twoCharOp));
                        Advance();
                        return;
                    case "&&":
                        this.tokens.Add(new Token("AND", twoCharOp));
                        Advance();
                        return;
                    case "||":
                        this.tokens.Add(new Token("OR", twoCharOp));
                        Advance();
                        return;
                }
            }

            if (tokenMap.ContainsKey(curr))
            {
                this.tokens.Add(new Token(tokenMap[curr], curr.ToString()));
            }
            else
            {
                switch (curr)
                {
                    case '<':
                        this.tokens.Add(new Token("LT", curr.ToString()));
                        break;
                    case '>':
                        this.tokens.Add(new Token("GT", curr.ToString()));
                        break;
                    case '!':
                        this.tokens.Add(new Token("NOT", curr.ToString()));
                        break;
                    default:
                        throw new Exception("Unexpected character: " + curr);
                }
            }
        }

        public List<Token> GetTokens()
        {
            return this.tokens;
        }
    }
}
