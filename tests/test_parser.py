from unittest import TestCase
from vsi.parser import Parser
from vsi.lexer import Token
from vsi.api import lex
from vsi.ast import (
    PrintStatement,
    AST,
    Integer,
    Varexpr,
    BinOpexpr,
    RelOpexpr,
    Andexpr,
    Orexpr,
    Notexpr,
    AssignStatement,
    IfStatement,
    WhileStatement,
    Float
)


class TestParser(TestCase):

    def test___repr___method(self):
        tokens = lex("1 + 1;")
        ast = AST()
        parser = Parser(tokens)
        parser_repr = repr(parser)
        expected_repr = "Parser({}, 0, {})".format(tokens, ast)
        self.assertEqual(parser_repr, expected_repr)

    def test_parse_method(self):
        tokens = lex("print 1 + 1;")
        parser = Parser(tokens)
        ast = parser.parse()
        expected_ast = AST([PrintStatement(
                            BinOpexpr("+",
                                      Integer(1),
                                      Integer(1)
                                      )
                            )
                            ])
        self.assertEqual(ast, expected_ast)

    def test_parse_statements_method(self):
        tokens = lex("print 1;")
        parser = Parser(tokens)
        parser.parse()
        expected_ast = AST([PrintStatement(Integer(1))])
        self.assertEqual(parser.ast, expected_ast)

    def test_found_method(self):
        tokens = lex("x := 1 + 1;")
        parser = Parser(tokens)
        self.assertTrue(parser.found("ID", by="type"))
        parser.advance(by=2)
        self.assertTrue(parser.found(1, by="value"))

    def test_found_one_of_method(self):
        tokens = lex("x := 1 + 1;")
        parser = Parser(tokens)
        self.assertTrue(parser.found_one_of(["ID", "RESERVED"], by="type"))
        parser.advance(by=2)
        self.assertTrue(parser.found_one_of(["NUMBER", "OPERATOR"], by="type"))

    def test_expect_method(self):
        tokens = lex("print 1;")
        parser = Parser(tokens)
        self.assertTrue(parser.expect("RESERVED", by="type"))

    def test_expect_one_of_method(self):
        tokens = lex("print 1;")
        parser = Parser(tokens)
        self.assertTrue(parser.expect_one_of(["RESERVED", "ID"], by="type"))

    def test_optional_method(self):
        tokens = lex("if x > 10 then\nx:=1\nelse x:=10")
        parser = Parser(tokens)
        self.assertIsNone(parser.optional("else"))
        parser.advance(by=8)
        self.assertTrue(parser.optional("else"))

    def test_advance_method(self):
        tokens = lex("x := 1")
        parser = Parser(tokens)
        parser.advance()
        self.assertEqual(parser.pos, 1)

    def test_token_ahead_method(self):
        tokens = lex("x := 1")
        parser = Parser(tokens)
        parser.advance()
        self.assertEqual(parser.token_ahead().value, 1)

    def test_parse_if_statement_method(self):
        tokens = lex("if x > 22 then\nprint x;\ndone\n")
        parser = Parser(tokens)
        parser.advance()
        ast = parser.parse_if_statement()
        expected_ast = IfStatement(
            RelOpexpr(">", Varexpr("x"), Integer(22)),
            AST([PrintStatement(Varexpr("x"))]),
            false_stmt=None
        )
        self.assertEqual(ast, expected_ast)

    def parse_if_statement_method_with_else_statement(self):
        tokens = lex("if x > 22 then\nprint x;\nelse\nprint 0;\ndone\n")
        parser = Parser(tokens)
        parser.advance()
        ast = parser.parse_if_statement()
        expected_ast = IfStatement(
            RelOpexpr(">", Varexpr("x"), Integer(22)),
            AST([PrintStatement(Varexpr("x"))]),
            AST([PrintStatement(Integer(0))])
        )
        self.assertEqual(ast, expected_ast)

    def test_parse_while_statement_method(self):
        tokens = lex("while x > 22 do\nprint x;\nx := x - 1;\ndone\n")
        parser = Parser(tokens)
        parser.advance()
        ast = parser.parse_while_statement()
        expected_ast = WhileStatement(
            RelOpexpr(">", Varexpr("x"), Integer(22)),
            AST([PrintStatement(Varexpr("x")),
                 AssignStatement("x", BinOpexpr("-", Varexpr("x"), Integer(1)))
                 ]),
        )
        self.assertEqual(ast, expected_ast)

    def test_parse_var_assignment_method(self):
        tokens = lex("x := 10;")
        parser = Parser(tokens)
        ast = parser.parse_var_assignment()
        expected_ast = AssignStatement("x", Integer(10))
        self.assertEqual(ast, expected_ast)

    def test_parse_print_statement_method(self):
        tokens = lex("print 10;")
        parser = Parser(tokens)
        parser.advance()
        ast = parser.parse_print_statement()
        expected_ast = PrintStatement(Integer(10))
        self.assertEqual(ast, expected_ast)

    def test_parse_expr_method(self):
        tokens = lex("1+1;")
        parser = Parser(tokens)
        ast = parser.parse_expr()
        expected_ast = BinOpexpr("+", Integer(1), Integer(1))
        self.assertEqual(ast, expected_ast)

    def test_create_ast_from_postfix_method(self):
        postfix = [Token(1, "NUMBER", "INT"),
                   Token(1, "NUMBER", "INT"),
                   Token("+", "RESERVED", "OPERATOR")
                   ]
        parser = Parser([])
        ast = parser.create_ast_from_postfix(postfix)
        expected_ast = BinOpexpr("+", Integer(1), Integer(1))
        self.assertEqual(ast, expected_ast)

    def test_create_ast_from_expr_method(self):
        parser = Parser([])
        ast = parser.create_ast_from_expr("+", Integer(1), Integer(1))
        expected_ast = BinOpexpr("+", Integer(1), Integer(1))
        self.assertEqual(ast, expected_ast)
