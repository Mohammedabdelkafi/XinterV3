using System;
using System.Collections.Generic;

namespace XinterV3
{
    class Program
    {
        static void Main()
        {
            bool running = true;
            bool calcMode = false;
            var variables = new Dictionary<string, double>();
            var commands = new List<string>();

            while (running)
            {
                Console.Write("Xinter ==>");
                string text = Console.ReadLine();
                switch (text)
                {
                    case "calc":
                        calcMode = true;
                        Console.WriteLine("Calc mode activated");
                        continue;
                    case "decalc":
                        calcMode = false;
                        Console.WriteLine("Calc mode deactivated");
                        continue;
                    case "run":
                        Console.WriteLine("Running all commands...");
                        foreach (var cmd in commands)
                        {
                            var innerLexer = new Lexer(cmd);
                            var innerParser = new Parser(innerLexer.GetTokens(), calcMode, variables);
                        }
                        continue;
                    case "exit":
                        Console.WriteLine("Exiting...");
                        running = false;
                        break;
                    default:
                        commands.Add(text);
                        var lexer = new Lexer(text);
                        var parser = new Parser(lexer.GetTokens(), calcMode, variables);
                        continue;
                }
            }
        }
    }
}
