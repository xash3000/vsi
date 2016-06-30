import sys
from .lexer import Lexer
from .parser import Parser
from .interpreter import Interpreter

# general types
RESERVED = "RESERVED"
NUMBER = "NUMBER"
ID = "ID"

# specific types
OPERATOR = "OPERATOR"
STATEMENT = "STATEMENT"
INT = "INT"
SYMBOL = "SYMBOL"

tokens_exprs = [
    # reject whitespaces and comments
    (r"[ \n\t]+", None, None, 0),
    (r"#[^\n]*", None, None, 0),
    # (regex_pattern, general_type, specific_type, precednece)
    (r"(;)", RESERVED, SYMBOL, 0),
    (r"(:=)", RESERVED, OPERATOR, 0),
    (r'(\()', RESERVED, OPERATOR, 9),
    (r'(\))', RESERVED, OPERATOR, 0),
    (r'(\+)', RESERVED, OPERATOR, 2),
    (r'(-)', RESERVED, OPERATOR, 2),
    (r'(\*)', RESERVED, OPERATOR, 3),
    (r'(/)', RESERVED, OPERATOR, 3),
    (r'(%)', RESERVED, OPERATOR, 4),
    (r'(<=)', RESERVED, OPERATOR, 2),
    (r'(<)', RESERVED, OPERATOR, 2),
    (r'(>=)', RESERVED, OPERATOR, 2),
    (r'(>)', RESERVED, OPERATOR, 2),
    (r'(==)', RESERVED, OPERATOR, 2),
    (r'(!=)', RESERVED, OPERATOR, 2),
    (r' (and) ', RESERVED, OPERATOR, 1),
    (r' (or) ', RESERVED, OPERATOR, 1),
    (r'(not) ', RESERVED, OPERATOR, 1),
    (r'(print) ', RESERVED, STATEMENT, 0),
    (r'(if) ', RESERVED, STATEMENT, 0),
    (r'(then)', RESERVED, STATEMENT, 0),
    (r'(else) ', RESERVED, STATEMENT, 0),
    (r'(while) ', RESERVED, STATEMENT, 0),
    (r'(do)[ \n\t]{1,}', RESERVED, STATEMENT, 0),
    (r'(done)[ \n\t]{1,}', RESERVED, STATEMENT, 0),
    (r'([0-9]+)', NUMBER, INT, 0),
    (r'([_A-Za-z][A-Za-z0-9_]*)', ID, None, None),
]


def lex(string, tokens_exprs=tokens_exprs):
    lexer = Lexer(string, tokens_exprs)
    tokens = lexer.lex()
    return tokens


def parse(string):
    tokens = lex(string)
    parser = Parser(tokens)
    ast = parser.parse()
    return ast


def interpret(string, env=None):
    ast = parse(string)
    interpreter = Interpreter(ast, env)
    interpreter.interpret()
