using System;
using System.Collections.Generic;

namespace XinterV3
{
    class Token
    {
        public string Type { get; }
        public string Value { get; }

        public Token(string type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        public override string ToString()
        {
            return $"{Type}({Value})";
        }
    }

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

    class Parser
    {
        private List<Token> tokens;
        private int idx;
        private Token currTok;
        private Dictionary<string, double> variables;

        public Parser(List<Token> tokens, Dictionary<string, double> variables)
        {
            this.tokens = tokens;
            this.idx = -1;
            this.variables = variables;
            Advance();
            Parse();
        }

        private void Advance()
        {
            this.currTok = ++this.idx < this.tokens.Count ? this.tokens[this.idx] : null;
        }

        private void Parse()
        {
            while (this.currTok != null)
            {
                if (this.currTok.Type == "IDENTIFIER")
                {
                    if (this.tokens[this.idx + 1]?.Type == "EQUALS")
                    {
                        Assign();
                    }
                    else if (this.currTok.Value == "log")
                    {
                        Advance();
                        Print();
                    }
                    else
                    {
                        Expr();
                    }
                }
                else
                {
                    Expr();
                }
            }
        }

        private void Assign()
        {
            string varName = this.currTok.Value;
            Advance(); // skip var name
            Advance(); // skip EQUALS
            double value = Expr();
            this.variables[varName] = value;
        }

        private double Expr()
        {
            double result = Term();
            while (this.currTok != null && (this.currTok.Type == "PLUS" || this.currTok.Type == "MINUS"))
            {
                string op = this.currTok.Type;
                Advance();
                if (op == "PLUS")
                {
                    result += Term();
                }
                else if (op == "MINUS")
                {
                    result -= Term();
                }
            }
            Console.WriteLine("Result: " + result);
            return result;
        }

        private double Term()
        { 
            double result = Factor();
            while (this.currTok != null && (this.currTok.Type == "MULTIPLY" || this.currTok.Type == "DIVIDE"))
            {
                string op = this.currTok.Type;
                Advance();
                if (op == "MULTIPLY")
                {
                    result *= Factor();
                }
                else if (op == "DIVIDE")
                {
                    result /= Factor();
                }
            }
            return result;
        }

        private double Factor()
        {
            double result;
            switch (this.currTok.Type)
            {
                case "NUMBER":
                    result = double.Parse(this.currTok.Value);
                    Advance();
                    break;
                case "IDENTIFIER":
                    if (this.variables.ContainsKey(this.currTok.Value))
                    {
                        result = this.variables[this.currTok.Value];
                        Advance();
                    }
                    else
                    {
                        throw new Exception($"Undefined variable: {this.currTok.Value}");
                    }
                    break;
                case "LPAREN":
                    Advance();
                    result = Expr();
                    if (this.currTok.Type == "RPAREN")
                    {
                        Advance();
                    }
                    else
                    {
                        throw new Exception("Expected ')'");
                    }
                    break;
                default:
                    throw new Exception("Unexpected token: " + this.currTok);
            }
            return result;
        }

        private void Print()
        {
            double value = Expr();
            Console.WriteLine(value);
        }
    }

    class Program
    {
        static void Main()
        {
            bool running = true;
            var variables = new Dictionary<string, double>();
            var commands = new List<string>();

            while (running)
            {
                Console.Write("Xinter ==>");
                string text = Console.ReadLine();
                switch (text)
                {
                    case "run":
                        Console.WriteLine("Running all commands...");
                        foreach (var cmd in commands)
                        {
                            var innerLexer = new Lexer(cmd);
                            var innerParser = new Parser(innerLexer.GetTokens(), variables);
                        }
                        break;
                    default:
                        commands.Add(text);
                        var lexer = new Lexer(text);
                        var parser = new Parser(lexer.GetTokens(), variables);
                        break;
                }
            }
        }
    }
}
