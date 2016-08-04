from unittest import TestCase
from vsi.lexer import Lexer, Token
from vsi.api import tokens_exprs


class TestLexer(TestCase):

    def test___repr___method(self):
        lexer = Lexer("10", tokens_exprs)
        lexer_repr = repr(lexer)
        expected_repr = "Lexer(10, {}, [], 0)".format(tokens_exprs)
        self.assertEqual(lexer_repr, expected_repr)

    def test_prepare_tokens_method(self):
        lexer = Lexer("", tokens_exprs)
        lexer.tokens = [Token("10", "NUMBER", "INT"),
                        Token("1.2", "NUMBER", "FLOAT")
                        ]
        lexer.prepare_tokens()
        self.assertIn(Token("EOF", "RESERVED", "SYMBOL"), lexer.tokens)
        self.assertEqual(lexer.tokens[0].value, 10)
        self.assertEqual(lexer.tokens[1].value, 1.2)

    def test_lex_method_with_whitespaces_and_comments(self):
        eof = Token("EOF", "RESERVED", "SYMBOL", 0)

        tokens_1 = Lexer("#comment", tokens_exprs).lex()
        self.assertEqual(tokens_1, [eof])

        # \n => newline
        tokens_2 = Lexer("\n      \n         \n", tokens_exprs).lex()
        self.assertEqual(tokens_2, [eof])

        tokens_3 = Lexer("# comment \n        122", tokens_exprs).lex()
        self.assertEqual(tokens_3, [Token(122, "NUMBER", "INT"), eof])

    def test_lex_method_simple(self):
        eof = Token("EOF", "RESERVED", "SYMBOL")

        tokens_1 = Lexer("x", tokens_exprs).lex()
        self.assertEqual(tokens_1, [Token("x", "ID"), eof])

        tokens_2 = Lexer("11", tokens_exprs).lex()
        self.assertEqual(tokens_2, [Token(11, "NUMBER", "INT"), eof])

        tokens_3 = Lexer(";", tokens_exprs).lex()
        expected = [Token(";", "RESERVED", "SYMBOL"), eof]
        self.assertEqual(tokens_3, expected)

    def test_lex_method_without_whitespaces(self):
        code = "x:=1+3.2/2;"
        tokens = Lexer(code, tokens_exprs).lex()
        expected_tokens = [Token("x", "ID"),
                           Token(":=", "RESERVED", "OPERATOR"),
                           Token(1, "NUMBER", "INT"),
                           Token("+", "RESERVED", "OPERATOR", 2),
                           Token(3.2, "NUMBER", "FLOAT"),
                           Token("/", "RESERVED", "OPERATOR", 3),
                           Token(2, "NUMBER", "INT"),
                           Token(";", "RESERVED", "SYMBOL"),
                           Token("EOF", "RESERVED", "SYMBOL")
                           ]
        self.assertEqual(tokens, expected_tokens)
