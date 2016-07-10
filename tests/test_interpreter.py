from unittest import TestCase
from vsi.ast import AST, AssignStatement, Integer
from vsi.interpreter import Interpreter


class TestInterpreter(TestCase):

    def test___repr___method(self):
        env = {}
        interpreter = Interpreter(AST([AssignStatement("x", 10)]), env)
        interpreter_repr = repr(interpreter)
        expected_repr = "Interpreter(Statements([AssignStatement(x, 10)]), {})"
        self.assertEqual(interpreter_repr, expected_repr)

    def test_interpret_method(self):
        env = {}
        ast = AST([AssignStatement("x", Integer(10))])
        interpreter = Interpreter(ast, env)
        interpreter.interpret()
        self.assertIn("x", env)
        self.assertEqual(env["x"], 10)
