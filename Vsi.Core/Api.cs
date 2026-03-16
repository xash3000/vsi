using System.Collections.Generic;

namespace Vsi.Core
{
    public static class Api
    {
        public static List<Token> Lex(string input)
        {
            var lexer = new Lexer(input);
            return lexer.Lex();
        }

        public static Ast Parse(string input)
        {
            var tokens = Lex(input);
            var parser = new Parser(tokens);
            return parser.Parse();
        }

        public static void Interpret(string input, Dictionary<string, object?>? env = null)
        {
            var ast = Parse(input);
            var interpreter = new Interpreter(ast, env);
            interpreter.Interpret();
        }
    }
}
