from unittest import TestCase
from vsi.api import lex, parse, interpret
from vsi.lexer import Token
from vsi.ast import AST, AssignStatement, Integer


class TestApi(TestCase):

    def test_lex_function(self):
        string = "x := 10;"
        tokens = lex(string)
        expected_tokens = [Token("x", "ID", None, 0),
                           Token(":=", "RESERVED", "OPERATOR", 0),
                           Token(10, "NUMBER", "INT", 0),
                           Token(";", "RESERVED", "SYMBOL", 0),
                           Token("EOF", "RESERVED", "SYMBOL", 0)
                           ]

        self.assertEqual(tokens, expected_tokens)

    def test_parse_function(self):
        string = "x := 10;"
        ast = parse(string)
        expected_ast = AST([AssignStatement("x", Integer(10))])
        self.assertEqual(ast, expected_ast)

    def test_interpret_function(self):
        string = "x := 10;"
        env = {}
        interpret(string, env)
        self.assertIn("x", env)
        self.assertEqual(env["x"], 10)
