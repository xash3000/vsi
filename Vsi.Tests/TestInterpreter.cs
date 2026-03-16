using System.Collections.Generic;
using Xunit;
using Vsi.Core;

namespace Vsi.Tests
{
    public class TestInterpreter
    {
        [Fact]
        public void TestInterpreterReprMethod()
        {
            var env = new Dictionary<string, object?>();
            var interpreter = new Interpreter(new Ast(new List<AstNode> { new AssignStatement("x", new Integer(10)) }), env);
            Assert.Equal("Interpreter(Statements([AssignStatement(x, Integer(10))]), {})", interpreter.ToString());
        }

        [Fact]
        public void TestInterpretMethod()
        {
            var env = new Dictionary<string, object?>();
            var ast = new Ast(new List<AstNode> { new AssignStatement("x", new Integer(10)) });
            var interpreter = new Interpreter(ast, env);
            interpreter.Interpret();
            Assert.True(env.ContainsKey("x"));
            Assert.Equal(10, env["x"]);
        }
    }
}
