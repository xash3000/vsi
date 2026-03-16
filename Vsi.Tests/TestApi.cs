using System.Collections.Generic;
using Xunit;
using Vsi.Core;

namespace Vsi.Tests
{
    public class TestApi
    {
        [Fact]
        public void TestLexFunction()
        {
            string str = "x := 10;";
            var tokens = Api.Lex(str);
            var expectedTokens = new List<Token>
            {
                new Token("x", TokenType.Id, TokenSpecificType.None, 0),
                new Token(":=", TokenType.Reserved, TokenSpecificType.Operator, 0),
                new Token(10, TokenType.Number, TokenSpecificType.Int, 0),
                new Token(";", TokenType.Reserved, TokenSpecificType.Symbol, 0),
                new Token("EOF", TokenType.Reserved, TokenSpecificType.Symbol, 0)
            };

            Assert.Equal(expectedTokens, tokens);
        }

        [Fact]
        public void TestParseFunction()
        {
            string str = "x := 10;";
            var ast = Api.Parse(str);
            var expectedAst = new Ast(new List<AstNode> { new AssignStatement("x", new Integer(10)) });
            Assert.Equal(expectedAst, ast);
        }

        [Fact]
        public void TestInterpretFunction()
        {
            string str = "x := 10;";
            var env = new Dictionary<string, object?>();
            Api.Interpret(str, env);
            Assert.True(env.ContainsKey("x"));
            Assert.Equal(10, env["x"]);
        }
    }
}
