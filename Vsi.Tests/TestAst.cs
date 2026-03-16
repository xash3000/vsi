using System.Collections.Generic;
using Xunit;
using Vsi.Core;

namespace Vsi.Tests
{
    public class TestAst
    {
        [Fact]
        public void TestAstReprMethod()
        {
            var ast = new Ast();
            ast.InsertNode(new Integer(10));
            Assert.Equal("Statements([Integer(10)])", ast.ToString());
        }

        [Fact]
        public void TestAstEvalMethod()
        {
            var ast = new Ast();
            ast.InsertNode(new AssignStatement("x", new Integer(10)));
            var env = new Dictionary<string, object?>();
            ast.Eval(env);
            Assert.True(env.ContainsKey("x"));
            Assert.Equal(10, env["x"]);
        }

        [Fact]
        public void TestAstInsertNodeMethod()
        {
            var ast = new Ast();
            ast.InsertNode(new Integer(10));
            ast.InsertNode(new VarExpr("x"));
            var expectedNodes = new List<AstNode> { new Integer(10), new VarExpr("x") };
            Assert.Equal(expectedNodes, ast.Nodes);
        }

        [Fact]
        public void TestPrintStatementReprMethod()
        {
            var printStatement = new PrintStatement(new Integer(10));
            Assert.Equal("PrintStatement(Integer(10))", printStatement.ToString());
        }

        [Fact]
        public void TestIntegerReprMethod()
        {
            var integer = new Integer(10);
            Assert.Equal("Integer(10)", integer.ToString());
        }

        [Fact]
        public void TestIntegerEvalMethod()
        {
            var integer = new Integer(10);
            var env = new Dictionary<string, object?>();
            Assert.Equal(10, integer.Eval(env));
        }

        [Fact]
        public void TestFloatReprMethod()
        {
            var f = new Float(1.2);
            Assert.Equal("Float(1.2)", f.ToString());
        }

        [Fact]
        public void TestFloatEvalMethod()
        {
            var f = new Float(1.2);
            var env = new Dictionary<string, object?>();
            Assert.Equal(1.2, f.Eval(env));
        }

        [Fact]
        public void TestVarExprReprMethod()
        {
            var varexpr = new VarExpr("x");
            Assert.Equal("Varexpr(x)", varexpr.ToString());
        }

        [Fact]
        public void TestVarExprEvalMethodWithDefinedVariable()
        {
            var env = new Dictionary<string, object?> { { "x", 10 } };
            var varexpr = new VarExpr("x");
            Assert.Equal(10, varexpr.Eval(env));
        }

        [Fact]
        public void TestBinOpExprReprMethod()
        {
            var binopexpr = new BinOpExpr("+", new Integer(1), new Integer(1));
            Assert.Equal("BinOpexpr(+, Integer(1), Integer(1))", binopexpr.ToString());
        }

        [Fact]
        public void TestBinOpExprEvalMethodWithPlusOperator()
        {
            var binopexpr = new BinOpExpr("+", new Integer(12), new Integer(5));
            var env = new Dictionary<string, object?>();
            Assert.Equal(17, binopexpr.Eval(env));
        }

        [Fact]
        public void TestBinOpExprEvalMethodWithMinusOperator()
        {
            var binopexpr = new BinOpExpr("-", new Integer(12), new Integer(5));
            var env = new Dictionary<string, object?>();
            Assert.Equal(7, binopexpr.Eval(env));
        }

        [Fact]
        public void TestBinOpExprEvalMethodWithTimesOperator()
        {
            var binopexpr = new BinOpExpr("*", new Integer(12), new Integer(5));
            var env = new Dictionary<string, object?>();
            Assert.Equal(60, binopexpr.Eval(env));
        }

        [Fact]
        public void TestBinOpExprEvalMethodWithObelusOperator()
        {
            var binopexpr = new BinOpExpr("/", new Integer(10), new Integer(5));
            var env = new Dictionary<string, object?>();
            Assert.Equal(2, binopexpr.Eval(env));
        }

        [Fact]
        public void TestBinOpExprEvalMethodWithModuloOperator()
        {
            var binopexpr = new BinOpExpr("%", new Integer(12), new Integer(5));
            var env = new Dictionary<string, object?>();
            Assert.Equal(2, binopexpr.Eval(env));
        }

        [Fact]
        public void TestRelOpExprReprMethod()
        {
            var relopexpr = new RelOpExpr("==", new Integer(10), new Integer(12));
            Assert.Equal("RelOpexpr(==, Integer(10), Integer(12))", relopexpr.ToString());
        }

        [Fact]
        public void TestRelOpExprEvalMethodWithEqualOperator()
        {
            var relopexpr = new RelOpExpr("==", new Integer(10), new Integer(12));
            var env = new Dictionary<string, object?>();
            Assert.Equal(false, relopexpr.Eval(env));
        }

        [Fact]
        public void TestRelOpExprEvalMethodWithGreaterThanOperator()
        {
            var relopexpr = new RelOpExpr(">", new Integer(10), new Integer(12));
            var env = new Dictionary<string, object?>();
            Assert.Equal(false, relopexpr.Eval(env));
        }

        [Fact]
        public void TestRelOpExprEvalMethodWithLessThanOperator()
        {
            var relopexpr = new RelOpExpr("<", new Integer(10), new Integer(12));
            var env = new Dictionary<string, object?>();
            Assert.Equal(true, relopexpr.Eval(env));
        }

        [Fact]
        public void TestRelOpExprEvalMethodWithGreaterThanOrEqualOperator()
        {
            var relopexpr = new RelOpExpr(">=", new Integer(10), new Integer(12));
            var env = new Dictionary<string, object?>();
            Assert.Equal(false, relopexpr.Eval(env));
        }

        [Fact]
        public void TestRelOpExprEvalMethodWithLessThanOrEqualOperator()
        {
            var relopexpr = new RelOpExpr("<=", new Integer(10), new Integer(12));
            var env = new Dictionary<string, object?>();
            Assert.Equal(true, relopexpr.Eval(env));
        }

        [Fact]
        public void TestRelOpExprEvalMethodWithNotEqualOperator()
        {
            var relopexpr = new RelOpExpr("!=", new Integer(10), new Integer(12));
            var env = new Dictionary<string, object?>();
            Assert.Equal(true, relopexpr.Eval(env));
        }

        [Fact]
        public void TestAndExprReprMethod()
        {
            var andexpr = new AndExpr(new Integer(1), new Integer(2));
            Assert.Equal("Andexpr(Integer(1), Integer(2))", andexpr.ToString());
        }

        [Fact]
        public void TestAndExprEvalMethodWithTrueExpression()
        {
            var andexpr = new AndExpr(new Integer(1), new Integer(1));
            var env = new Dictionary<string, object?>();
            Assert.Equal(true, andexpr.Eval(env));
        }

        [Fact]
        public void TestAndExprEvalMethodWithFalseExpression()
        {
            var andexpr = new AndExpr(new Integer(1), new Integer(0));
            var env = new Dictionary<string, object?>();
            Assert.Equal(false, andexpr.Eval(env));
        }

        [Fact]
        public void TestOrExprReprMethod()
        {
            var orexpr = new OrExpr(new Integer(1), new Integer(0));
            Assert.Equal("Orexpr(Integer(1), Integer(0))", orexpr.ToString());
        }

        [Fact]
        public void TestOrExprEvalMethodWithTrueExpression()
        {
            var orexpr = new OrExpr(new Integer(1), new Integer(0));
            var env = new Dictionary<string, object?>();
            Assert.Equal(true, orexpr.Eval(env));
        }

        [Fact]
        public void TestOrExprEvalMethodWithFalseExpression()
        {
            var orexpr = new OrExpr(new Integer(0), new Integer(0));
            var env = new Dictionary<string, object?>();
            Assert.Equal(false, orexpr.Eval(env));
        }

        [Fact]
        public void TestNotExprReprMethod()
        {
            var notexpr = new NotExpr(new Integer(1));
            Assert.Equal("Notexpr(Integer(1))", notexpr.ToString());
        }

        [Fact]
        public void TestNotExprEvalMethodWithTrueExpression()
        {
            var notexpr = new NotExpr(new Integer(0));
            var env = new Dictionary<string, object?>();
            Assert.Equal(true, notexpr.Eval(env));
        }

        [Fact]
        public void TestNotExprEvalMethodWithFalseExpression()
        {
            var notexpr = new NotExpr(new Integer(1));
            var env = new Dictionary<string, object?>();
            Assert.Equal(false, notexpr.Eval(env));
        }

        [Fact]
        public void TestAssignStatementReprMethod()
        {
            var assignStmt = new AssignStatement("x", new Integer(10));
            Assert.Equal("AssignStatement(x, Integer(10))", assignStmt.ToString());
        }

        [Fact]
        public void TestAssignStatementEvalMethod()
        {
            var assignStmt = new AssignStatement("x", new Integer(10));
            var env = new Dictionary<string, object?>();
            assignStmt.Eval(env);
            Assert.True(env.ContainsKey("x"));
            Assert.Equal(10, env["x"]);
        }

        [Fact]
        public void TestIfStatementReprMethod()
        {
            var ifStmt = new IfStatement(new Integer(1), new BinOpExpr("+", new Integer(1), new Integer(2)), null);
            Assert.Equal("IfStatement(Integer(1), BinOpexpr(+, Integer(1), Integer(2)), None)", ifStmt.ToString());
        }

        [Fact]
        public void TestIfStatementEvalMethodWithTrueConditionAndNoElseStmt()
        {
            var ifStmt = new IfStatement(new Integer(1), new AssignStatement("x", new Integer(10)), null);
            var env = new Dictionary<string, object?>();
            ifStmt.Eval(env);
            Assert.True(env.ContainsKey("x"));
            Assert.Equal(10, env["x"]);
        }

        [Fact]
        public void TestIfStatementEvalMethodWithTrueConditionAndElseStmt()
        {
            var ifStmt = new IfStatement(new Integer(1), new AssignStatement("x", new Integer(10)), new AssignStatement("y", new Integer(12)));
            var env = new Dictionary<string, object?>();
            ifStmt.Eval(env);
            Assert.True(env.ContainsKey("x"));
            Assert.Equal(10, env["x"]);
            Assert.False(env.ContainsKey("y"));
        }

        [Fact]
        public void TestIfStatementEvalMethodWithFalseConditionAndElseStmt()
        {
            var ifStmt = new IfStatement(new Integer(0), new AssignStatement("x", new Integer(10)), new AssignStatement("y", new Integer(12)));
            var env = new Dictionary<string, object?>();
            ifStmt.Eval(env);
            Assert.False(env.ContainsKey("x"));
            Assert.True(env.ContainsKey("y"));
            Assert.Equal(12, env["y"]);
        }

        [Fact]
        public void TestIfStatementEvalMethodWithFalseConditionAndNoElseStmt()
        {
            var ifStmt = new IfStatement(new Integer(0), new AssignStatement("x", new Integer(10)), null);
            var env = new Dictionary<string, object?>();
            ifStmt.Eval(env);
            Assert.False(env.ContainsKey("x"));
        }

        [Fact]
        public void TestWhileStatementReprMethod()
        {
            var whileStmt = new WhileStatement(new Integer(1), new BinOpExpr("+", new Integer(1), new Integer(2)));
            Assert.Equal("WhileStatement(Integer(1), BinOpexpr(+, Integer(1), Integer(2)))", whileStmt.ToString());
        }

        [Fact]
        public void TestWhileStatementEvalMethodWithTrueExpression()
        {
            var env = new Dictionary<string, object?> { { "x", 0 } };
            var whileStmt = new WhileStatement(
                new RelOpExpr("<", new VarExpr("x"), new Integer(10)),
                new AssignStatement("x", new BinOpExpr("+", new VarExpr("x"), new Integer(1)))
            );
            whileStmt.Eval(env);
            Assert.Equal(10, env["x"]);
        }

        [Fact]
        public void TestWhileStatementEvalMethodWithFalseExpression()
        {
            var env = new Dictionary<string, object?> { { "x", 0 } };
            var whileStmt = new WhileStatement(
                new RelOpExpr(">", new VarExpr("x"), new Integer(10)),
                new AssignStatement("x", new BinOpExpr("+", new VarExpr("x"), new Integer(1)))
            );
            whileStmt.Eval(env);
            Assert.Equal(0, env["x"]);
        }
    }
}
