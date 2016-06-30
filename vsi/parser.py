from .ast import (
    printStatement,
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
    WhileStatement
)
import sys


class Parser:

    def __init__(self, tokens, pos=0):
        self.tokens = tokens
        self.pos = pos
        self.ast = AST()

    def parse(self):
        self.parse_statements(ast=self.ast)
        return self.ast

    def parse_statements(self, ast):
        while self.pos <= len(self.tokens):
            if self.found_one_of(["EOF", "else"]):
                break
            elif self.found("if"):
                self.advance()
                astnode = self.parse_if_statement()
                ast.insert_node(astnode)
            elif self.found("while"):
                self.advance()
                astnode = self.parse_while_statement()
                ast.insert_node(astnode)
            elif self.found("ID", by="type"):
                if self.token_ahead().value == ":=":
                    astnode = self.parse_var_assignment()
                else:
                    astnode = self.parse_expr()
                ast.insert_node(astnode)
            elif self.found("print"):
                self.advance()
                astnode = self.parse_print_statement()
                ast.insert_node(astnode)
            else:
                break

    def found(self, token, by="value"):
        if by == "value":
            return self.tokens[self.pos].value == token
        elif by == "type":
            return self.tokens[self.pos].type == token

    def found_one_of(self, tokens, by="value"):
        for token in tokens:
            if self.found(token, by=by):
                return True
        return False

    def expect(self, token, by="value"):
        if self.found(token, by=by):
            return True
        current_token = self.tokens[self.pos].value
        args = (token, current_token)
        err = "SyntaxError: expecting {} found {}".format(*args)
        sys.stderr.write(err + "\n")
        sys.exit(1)

    def expect_one_of(self, tokens, by="value"):
        if self.found_one_of(tokens, by=by):
            return True
        current_token = self.tokens[self.pos].value
        args = (tokens, current_token)
        err = "SyntaxError: expecting one of {} found {}".format(*args)
        sys.stderr.write(err + "\n")
        sys.exit(1)

    def optional(self, token, by="value"):
        if self.found(token, by=by):
            return True

    def advance(self, by=1):
        self.pos += by

    def token_ahead(self, by=1):
        if self.pos + by < len(self.tokens):
            return self.tokens[self.pos + by]

    def parse_if_statement(self):
        condition = self.parse_expr()
        self.expect("then")
        self.advance()
        true_stmt = AST()  # sub AST
        self.parse_statements(ast=true_stmt)
        false_stmt = None
        if self.optional("else"):
            self.advance()
            false_stmt = AST()  # sub AST
            self.parse_statements(ast=false_stmt)
        self.expect("done")
        self.advance()
        astnode = IfStatement(condition, true_stmt, false_stmt)
        return astnode

    def parse_while_statement(self):
        condition = self.parse_expr()
        self.expect("do")
        self.advance()
        body = AST()  # sub AST
        self.parse_statements(ast=body)
        self.expect("done")
        self.advance()
        astnode = WhileStatement(condition, body)
        return astnode

    def parse_var_assignment(self):
        var_name = self.tokens[self.pos].value
        self.advance()
        self.expect(":=")
        self.advance()
        var_value = self.parse_expr()
        self.expect(";")
        self.advance()
        astnode = AssignStatement(var_name, var_value)
        return astnode

    def parse_print_statement(self):
        expr = self.parse_expr()
        self.expect(";")
        self.advance()
        return printStatement(expr)

    def parse_expr(self):
        output = []
        op_stack = []
        while self.pos < len(self.tokens):
            current_token = self.tokens[self.pos]
            if current_token.specific_type == "OPERATOR":
                if (op_stack and
                        current_token.precedence <= op_stack[-1].precedence):
                    output.append(op_stack.pop())
                op_stack.append(current_token)
            elif current_token.type in ("NUMBER", "ID"):
                output.append(current_token)
            else:
                break
            self.advance()

        for op in reversed(op_stack):
            output.append(op_stack.pop())
        expr_ast = self.create_ast_from_postfix(output)
        return expr_ast

    def create_ast_from_postfix(self, postfix):
        stack = []
        for token in postfix:
            if token.specific_type == "OPERATOR":
                op = token.value
                operand_2 = stack.pop()
                operand_1 = None
                if op != "not":
                    # not operator has only one operand
                    operand_1 = stack.pop()
                astnode = self.create_ast_from_expr(op, operand_2, operand_1)
                stack.append(astnode)
            elif token.type == "NUMBER":
                token = Integer(token.value)
                stack.append(token)
            elif token.type == "ID":
                token = Varexpr(token.value)
                stack.append(token)
        final = stack[0]
        try:
            if final.type == "NUMBER":
                return Integer(final.value)
            elif final.type == "ID":
                return Varexpr(final.value)
        except AttributeError:
            return final

    def create_ast_from_expr(self, op, operand_2, operand_1):
        if op in ("+", "-", "*", "/", "%"):
            return BinOpexpr(op, operand_1, operand_2)
        elif op in ("<", ">", "<=", ">=", "!=", "=="):
            return RelOpexpr(op, operand_1, operand_2)
        elif op == "and":
            return Andexpr(operand_1, operand_2)
        elif op == "or":
            return Orexpr(operand_1, operand_2)
        elif op == "not":
            return Notexpr(operand_2)
        else:
            err = "SyntaxError: Invalid operator {}".format(op)
            sys.stderr.write(err + "\n")
            sys.exit(1)
