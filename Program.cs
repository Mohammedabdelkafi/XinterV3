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
            bool debugMode = false;
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
                    case "debug":
                        debugMode = true;
                        Console.WriteLine("Debug mode activated");
                        continue;
                    case "nodebug":
                        debugMode = false;
                        Console.WriteLine("Debug mode deactivated");
                        continue;
                    case "run":
                        Console.WriteLine("Running all commands...");
                        foreach (var cmd in commands)
                        {
                            var innerLexer = new Lexer(cmd, debugMode);
                            var innerParser = new Parser(innerLexer.GetTokens(), calcMode, debugMode, variables);
                        }
                        break;
                    default:
                        commands.Add(text);
                        var lexer = new Lexer(text, debugMode);
                        var parser = new Parser(lexer.GetTokens(), calcMode, debugMode, variables);
                        break;
                }
            }
        }
    }
}
