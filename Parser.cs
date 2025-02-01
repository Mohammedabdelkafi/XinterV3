using System;
using System.Collections.Generic;

namespace XinterV3
{
    class Parser
    {
        private List<Token> tokens;
        private int idx;
        private Token currTok;
        private bool calcMode;
        private bool debugMode;
        private Dictionary<string, double> variables;

        public Parser(List<Token> tokens, bool calcMode, bool debugMode, Dictionary<string, double> variables)
        {
            this.tokens = tokens;
            this.idx = -1;
            this.calcMode = calcMode;
            this.debugMode = debugMode;
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
            if (debugMode)
            {
                Console.WriteLine($"Assigned {varName} = {value}");
            }
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
            if (calcMode)
            {
                Console.WriteLine("Result: " + result);
            }
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
}
