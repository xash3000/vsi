using System.Collections.Generic;
using Xunit;
using Vsi.Core;

namespace Vsi.Tests
{
    public class TestLexer
    {
        [Fact]
        public void TestLexerReprMethod()
        {
            var lexer = new Lexer("10");
            var repr = lexer.ToString();
            Assert.Contains("Lexer(10, ", repr);
        }

        [Fact]
        public void TestPrepareTokensMethod()
        {
            var lexer = new Lexer("");
            lexer.Tokens.Add(new Token("10", TokenType.Number, TokenSpecificType.Int));
            lexer.Tokens.Add(new Token("1.2", TokenType.Number, TokenSpecificType.Float));
            lexer.PrepareTokens();

            Assert.Contains(new Token("EOF", TokenType.Reserved, TokenSpecificType.Symbol), lexer.Tokens);
            Assert.Equal(10, lexer.Tokens[0].Value);
            Assert.Equal(1.2, lexer.Tokens[1].Value);
        }

        [Fact]
        public void TestLexMethodWithWhitespacesAndComments()
        {
            var eof = new Token("EOF", TokenType.Reserved, TokenSpecificType.Symbol, 0);

            var tokens1 = new Lexer("#comment").Lex();
            Assert.Equal(new List<Token> { eof }, tokens1);

            var tokens2 = new Lexer("\n      \n         \n").Lex();
            Assert.Equal(new List<Token> { eof }, tokens2);

            var tokens3 = new Lexer("# comment \n        122").Lex();
            Assert.Equal(new List<Token> { new Token(122, TokenType.Number, TokenSpecificType.Int), eof }, tokens3);
        }

        [Fact]
        public void TestLexMethodSimple()
        {
            var eof = new Token("EOF", TokenType.Reserved, TokenSpecificType.Symbol);

            var tokens1 = new Lexer("x").Lex();
            Assert.Equal(new List<Token> { new Token("x", TokenType.Id), eof }, tokens1);

            var tokens2 = new Lexer("11").Lex();
            Assert.Equal(new List<Token> { new Token(11, TokenType.Number, TokenSpecificType.Int), eof }, tokens2);

            var tokens3 = new Lexer(";").Lex();
            Assert.Equal(new List<Token> { new Token(";", TokenType.Reserved, TokenSpecificType.Symbol), eof }, tokens3);
        }

        [Fact]
        public void TestLexMethodWithoutWhitespaces()
        {
            string code = "x:=1+3.2/2;";
            var tokens = new Lexer(code).Lex();
            var expectedTokens = new List<Token>
            {
                new Token("x", TokenType.Id),
                new Token(":=", TokenType.Reserved, TokenSpecificType.Operator),
                new Token(1, TokenType.Number, TokenSpecificType.Int),
                new Token("+", TokenType.Reserved, TokenSpecificType.Operator, 2),
                new Token(3.2, TokenType.Number, TokenSpecificType.Float),
                new Token("/", TokenType.Reserved, TokenSpecificType.Operator, 3),
                new Token(2, TokenType.Number, TokenSpecificType.Int),
                new Token(";", TokenType.Reserved, TokenSpecificType.Symbol),
                new Token("EOF", TokenType.Reserved, TokenSpecificType.Symbol)
            };
            Assert.Equal(expectedTokens, tokens);
        }
    }
}
