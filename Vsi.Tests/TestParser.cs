using System.Collections.Generic;
using Xunit;
using Vsi.Core;

namespace Vsi.Tests
{
    public class TestParser
    {
        [Fact]
        public void TestParserReprMethod()
        {
            var tokens = Api.Lex("1 + 1;");
            var parser = new Parser(tokens);
            var repr = parser.ToString();
            Assert.Contains("Parser([", repr);
        }

        [Fact]
        public void TestParseMethod()
        {
            var tokens = Api.Lex("print 1 + 1;");
            var parser = new Parser(tokens);
            var ast = parser.Parse();
            var expectedAst = new Ast(new List<AstNode>
            {
                new PrintStatement(new BinOpExpr("+", new Integer(1), new Integer(1)))
            });
            Assert.Equal(expectedAst, ast);
        }

        [Fact]
        public void TestParseStatementsMethod()
        {
            var tokens = Api.Lex("print 1;");
            var parser = new Parser(tokens);
            parser.Parse();
            var expectedAst = new Ast(new List<AstNode> { new PrintStatement(new Integer(1)) });
            Assert.Equal(expectedAst, parser.Ast);
        }

        [Fact]
        public void TestFoundMethod()
        {
            var tokens = Api.Lex("x := 1 + 1;");
            var parser = new Parser(tokens);
            Assert.True(parser.Found(TokenType.Id));
            parser.Advance(2);
            Assert.True(parser.Found("1"));
        }

        [Fact]
        public void TestFoundOneOfMethod()
        {
            var tokens = Api.Lex("x := 1 + 1;");
            var parser = new Parser(tokens);
            Assert.True(parser.FoundOneOf(new[] { TokenType.Id, TokenType.Reserved }));
            parser.Advance(1); // at :=
            Assert.True(parser.FoundOneOf(new[] { TokenSpecificType.Operator, TokenSpecificType.Symbol }));
        }

        [Fact]
        public void TestExpectMethod()
        {
            var tokens = Api.Lex("print 1;");
            var parser = new Parser(tokens);
            Assert.True(parser.Expect(TokenType.Reserved));
        }

        [Fact]
        public void TestExpectOneOfMethod()
        {
            var tokens = Api.Lex("print 1;");
            var parser = new Parser(tokens);
            Assert.True(parser.ExpectOneOf(new[] { TokenType.Reserved, TokenType.Id }));
        }

        [Fact]
        public void TestOptionalMethod()
        {
            var tokens = Api.Lex("if x > 10 then\nx:=1\nelse x:=10");
            var parser = new Parser(tokens);
            Assert.False(parser.Optional("else"));
            parser.Advance(8);
            Assert.True(parser.Optional("else"));
        }

        [Fact]
        public void TestAdvanceMethod()
        {
            var tokens = Api.Lex("x := 1");
            var parser = new Parser(tokens);
            parser.Advance();
            Assert.Equal(1, parser.Pos);
        }

        [Fact]
        public void TestTokenAheadMethod()
        {
            var tokens = Api.Lex("x := 1");
            var parser = new Parser(tokens);
            parser.Advance();
            Assert.Equal(1, parser.TokenAhead()?.Value);
        }

        [Fact]
        public void TestParseIfStatementMethod()
        {
            var tokens = Api.Lex("if x > 22 then\nprint x;\ndone\n");
            var parser = new Parser(tokens);
            parser.Advance();
            var ast = parser.ParseIfStatement();
            var expectedAst = new IfStatement(
                new RelOpExpr(">", new VarExpr("x"), new Integer(22)),
                new Ast(new List<AstNode> { new PrintStatement(new VarExpr("x")) }),
                null
            );
            Assert.Equal(expectedAst, ast);
        }

        [Fact]
        public void ParseIfStatementMethodWithElseStatement()
        {
            var tokens = Api.Lex("if x > 22 then\nprint x;\nelse\nprint 0;\ndone\n");
            var parser = new Parser(tokens);
            parser.Advance();
            var ast = parser.ParseIfStatement();
            var expectedAst = new IfStatement(
                new RelOpExpr(">", new VarExpr("x"), new Integer(22)),
                new Ast(new List<AstNode> { new PrintStatement(new VarExpr("x")) }),
                new Ast(new List<AstNode> { new PrintStatement(new Integer(0)) })
            );
            Assert.Equal(expectedAst, ast);
        }

        [Fact]
        public void TestParseWhileStatementMethod()
        {
            var tokens = Api.Lex("while x > 22 do\nprint x;\nx := x - 1;\ndone\n");
            var parser = new Parser(tokens);
            parser.Advance();
            var ast = parser.ParseWhileStatement();
            var expectedAst = new WhileStatement(
                new RelOpExpr(">", new VarExpr("x"), new Integer(22)),
                new Ast(new List<AstNode>
                {
                    new PrintStatement(new VarExpr("x")),
                    new AssignStatement("x", new BinOpExpr("-", new VarExpr("x"), new Integer(1)))
                })
            );
            Assert.Equal(expectedAst, ast);
        }

        [Fact]
        public void TestParseVarAssignmentMethod()
        {
            var tokens = Api.Lex("x := 10;");
            var parser = new Parser(tokens);
            var ast = parser.ParseVarAssignment();
            var expectedAst = new AssignStatement("x", new Integer(10));
            Assert.Equal(expectedAst, ast);
        }

        [Fact]
        public void TestParsePrintStatementMethod()
        {
            var tokens = Api.Lex("print 10;");
            var parser = new Parser(tokens);
            parser.Advance();
            var ast = parser.ParsePrintStatement();
            var expectedAst = new PrintStatement(new Integer(10));
            Assert.Equal(expectedAst, ast);
        }

        [Fact]
        public void TestParseExprMethod()
        {
            var tokens = Api.Lex("1+1;");
            var parser = new Parser(tokens);
            var ast = parser.ParseExpr();
            var expectedAst = new BinOpExpr("+", new Integer(1), new Integer(1));
            Assert.Equal(expectedAst, ast);
        }

        [Fact]
        public void TestCreateAstFromPostfixMethod()
        {
            var postfix = new List<Token>
            {
                new Token(1, TokenType.Number, TokenSpecificType.Int),
                new Token(1, TokenType.Number, TokenSpecificType.Int),
                new Token("+", TokenType.Reserved, TokenSpecificType.Operator)
            };
            var parser = new Parser(new List<Token>());
            var ast = parser.CreateAstFromPostfix(postfix);
            var expectedAst = new BinOpExpr("+", new Integer(1), new Integer(1));
            Assert.Equal(expectedAst, ast);
        }

        [Fact]
        public void TestCreateAstFromExprMethod()
        {
            var parser = new Parser(new List<Token>());
            var ast = parser.CreateAstFromExpr("+", new Integer(1), new Integer(1));
            var expectedAst = new BinOpExpr("+", new Integer(1), new Integer(1));
            Assert.Equal(expectedAst, ast);
        }
    }
}
