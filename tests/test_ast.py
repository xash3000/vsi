from unittest import TestCase
from vsi.ast import (
    AST,
    PrintStatement,
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


class TestAST(TestCase):

    def test___repr___method(self):
        ast = AST()
        ast.insert_node(Integer(10))
        ast_repr = repr(ast)
        expected_repr = "Statements([Integer(10)])"
        self.assertEqual(ast_repr, expected_repr)

    def test_eval_method(self):
        ast = AST()
        ast.insert_node(AssignStatement("x", Integer(10)))
        env = {}
        ast.eval(env)
        self.assertIn("x", env)
        self.assertEqual(env["x"], 10)

    def test_insert_node_method(self):
        ast = AST()
        ast.insert_node(Integer(10))
        ast.insert_node(Varexpr("x"))
        expected_nodes = [Integer(10), Varexpr("x")]
        self.assertEqual(ast.nodes, expected_nodes)


class TestPrintStatment(TestCase):

    def test___repr___method(self):
        print_statment = PrintStatement(Integer(10))
        print_statment_repr = repr(print_statment)
        expected_repr = "PrintStatement(Integer(10))"
        self.assertEqual(print_statment_repr, expected_repr)


class TestInteger(TestCase):

    def test___repr___method(self):
        integer = Integer(10)
        integer_repr = repr(integer)
        expected_repr = "Integer(10)"
        self.assertEqual(integer_repr, expected_repr)

    def test_eval_method(self):
        integer = Integer(10)
        env = {}
        result = integer.eval(env)
        self.assertEqual(result, 10)


class TestVarexpr(TestCase):

    def test___repr___method(self):
        varexpr = Varexpr("x")
        varexpr_repr = repr(varexpr)
        expected_repr = "Varexpr(x)"
        self.assertEqual(varexpr_repr, expected_repr)

    def test_eval_method_with_defined_variable(self):
        env = {"x": 10}
        varexpr = Varexpr("x")
        result = varexpr.eval(env)
        self.assertEqual(result, 10)


class TestBinOpexpr(TestCase):

    def test___repr___method(self):
        binopexpr = BinOpexpr("+", Integer(1), Integer(1))
        binopexpr_repr = repr(binopexpr)
        expected_repr = "BinOpexpr(+, Integer(1), Integer(1))"
        self.assertEqual(binopexpr_repr, expected_repr)

    def test_eval_method_with_plus_operator(self):
        binopexpr = BinOpexpr("+", Integer(12), Integer(5))
        env = {}
        result = binopexpr.eval(env)
        self.assertEqual(result, 17)

    def test_eval_method_with_minus_operator(self):
        binopexpr = BinOpexpr("-", Integer(12), Integer(5))
        env = {}
        result = binopexpr.eval(env)
        self.assertEqual(result, 7)

    def test_eval_method_with_times_operator(self):
        binopexpr = BinOpexpr("*", Integer(12), Integer(5))
        env = {}
        result = binopexpr.eval(env)
        self.assertEqual(result, 60)

    def test_eval_method_with_obelus_operator(self):
        binopexpr = BinOpexpr("/", Integer(10), Integer(5))
        env = {}
        result = binopexpr.eval(env)
        self.assertEqual(result, 2)

    def test_eval_method_with_modulo_operator(self):
        binopexpr = BinOpexpr("%", Integer(12), Integer(5))
        env = {}
        result = binopexpr.eval(env)
        self.assertEqual(result, 2)


class TestRelOpexpr(TestCase):

    def test___repr___method(self):
        relopexpr = RelOpexpr("==", Integer(10), Integer(12))
        relopexpr_repr = repr(relopexpr)
        expected_repr = "RelOpexpr(==, Integer(10), Integer(12))"
        self.assertEqual(relopexpr_repr, expected_repr)

    def test_eval_method_with_equal_operator(self):
        relopexpr = RelOpexpr("==", Integer(10), Integer(12))
        env = {}
        result = relopexpr.eval(env)
        self.assertFalse(result)

    def test_eval_method_with_greater_than_operator(self):
        relopexpr = RelOpexpr(">", Integer(10), Integer(12))
        env = {}
        result = relopexpr.eval(env)
        self.assertFalse(result)

    def test_eval_method_with_less_than_operator(self):
        relopexpr = RelOpexpr("<", Integer(10), Integer(12))
        env = {}
        result = relopexpr.eval(env)
        self.assertTrue(result)

    def test_eval_method_with_greater_than_or_equal_operator(self):
        relopexpr = RelOpexpr(">=", Integer(10), Integer(12))
        env = {}
        result = relopexpr.eval(env)
        self.assertFalse(result)

    def test_eval_method_with_less_than_or_equal_operator(self):
        relopexpr = RelOpexpr("<=", Integer(10), Integer(12))
        env = {}
        result = relopexpr.eval(env)
        self.assertTrue(result)

    def test_eval_method_with_not_equal_operator(self):
        relopexpr = RelOpexpr("!=", Integer(10), Integer(12))
        env = {}
        result = relopexpr.eval(env)
        self.assertTrue(result)


class TestAndexpr(TestCase):

    def test___repr___method(self):
        andexpr = Andexpr(Integer(1), Integer(2))
        andexpr_repr = repr(andexpr)
        expected_repr = "Andexpr(Integer(1), Integer(2))"
        self.assertEqual(andexpr_repr, expected_repr)

    def test_eval_method_with_true_expression(self):
        andexpr = Andexpr(Integer(1), Integer(1))
        env = {}
        result = andexpr.eval(env)
        self.assertTrue(result)

    def test_eval_method_with_false_expression(self):
        andexpr = Andexpr(Integer(1), Integer(0))
        env = {}
        result = andexpr.eval(env)
        self.assertFalse(result)


class TestOrexpr(TestCase):

    def test___repr___method(self):
        orexpr = Orexpr(Integer(1), Integer(0))
        orexpr_repr = repr(orexpr)
        expected_repr = "Orexpr(Integer(1), Integer(0))"
        self.assertEqual(orexpr_repr, expected_repr)

    def test_eval_method_with_true_expression(self):
        orexpr = Orexpr(Integer(1), Integer(0))
        env = {}
        result = orexpr.eval(env)
        self.assertTrue(result)

    def test_eval_method_with_false_expression(self):
        orexpr = Orexpr(Integer(0), Integer(0))
        env = {}
        result = orexpr.eval(env)
        self.assertFalse(result)


class TestNotexpr(TestCase):

    def test___repr___method(self):
        notexpr = Notexpr(Integer(1))
        notexpr_repr = repr(notexpr)
        expected_repr = "Notexpr(Integer(1))"
        self.assertEqual(notexpr_repr, expected_repr)

    def test_eval_method_with_true_expression(self):
        notexpr = Notexpr(Integer(0))
        env = {}
        result = notexpr.eval(env)
        self.assertTrue(result)

    def test_eval_method_with_false_expression(self):
        notexpr = Notexpr(Integer(1))
        env = {}
        result = notexpr.eval(env)
        self.assertFalse(result)


class TestAssignStatement(TestCase):

    def test___repr___method(self):
        assign_stmt = AssignStatement("x", Integer(10))
        assign_stmt_repr = repr(assign_stmt)
        expected_repr = "AssignStatement(x, Integer(10))"
        self.assertEqual(assign_stmt_repr, expected_repr)

    def test_eval_method(self):
        assign_stmt = AssignStatement("x", Integer(10))
        env = {}
        assign_stmt.eval(env)
        self.assertIn("x", env)
        self.assertEqual(env["x"], 10)


class TestIfStatement(TestCase):

    def test___repr___method(self):
        if_stmt = IfStatement(Integer(1),
                              BinOpexpr("+", Integer(1), Integer(2)),
                              # no else stmt
                              None
                              )
        if_stmt_repr = repr(if_stmt)
        expected_repr = "IfStatement(Integer(1), " \
                        "BinOpexpr(+, Integer(1), Integer(2)), None)"
        self.assertEqual(if_stmt_repr, expected_repr)

    def test_eval_method_with_true_condition_and_no_else_stmt(self):
        if_stmt = IfStatement(Integer(1),
                              AssignStatement("x", Integer(10)),
                              # no else stmt
                              None
                              )
        env = {}
        if_stmt.eval(env)
        self.assertIn("x", env)
        self.assertEqual(env["x"], 10)

    def test_eval_method_with_true_condition_and_else_stmt(self):
        if_stmt = IfStatement(Integer(1),
                              AssignStatement("x", Integer(10)),
                              # else stmt
                              AssignStatement("y", Integer(12)),
                              )
        env = {}
        if_stmt.eval(env)
        self.assertIn("x", env)
        self.assertEqual(env["x"], 10)
        self.assertNotIn("y", env)

    def test_eval_method_with_false_condition_and_else_stmt(self):
        if_stmt = IfStatement(Integer(0),
                              AssignStatement("x", Integer(10)),
                              # else stmt
                              AssignStatement("y", Integer(12)),
                              )
        env = {}
        if_stmt.eval(env)
        self.assertNotIn("x", env)
        self.assertIn("y", env)
        self.assertEqual(env["y"], 12)

    def test_eval_method_with_false_condition_and_no_else_stmt(self):
        if_stmt = IfStatement(Integer(0),
                              AssignStatement("x", Integer(10)),
                              # eno lse stmt
                              None
                              )
        env = {}
        if_stmt.eval(env)
        self.assertNotIn("x", env)


class TestWhileStatement(TestCase):

    def test___repr___method(self):
        while_stmt = WhileStatement(Integer(1),
                                    BinOpexpr("+", Integer(1), Integer(2)),
                                    )
        while_stmt_repr = repr(while_stmt)
        expected_repr = "WhileStatement(Integer(1), " \
                        "BinOpexpr(+, Integer(1), Integer(2)))"
        self.assertEqual(while_stmt_repr, expected_repr)

    def test_eval_method_with_true_expression(self):
        env = {"x": 0}
        while_stmt = WhileStatement(RelOpexpr("<", Varexpr("x"), Integer(10)),
                                    AssignStatement("x",
                                                    BinOpexpr("+",
                                                              Varexpr("x"),
                                                              Integer(1)
                                                              )
                                                    )
                                    )
        while_stmt.eval(env)
        self.assertEqual(env["x"], 10)

    def test_eval_method_with_false_expression(self):
        env = {"x": 0}
        while_stmt = WhileStatement(RelOpexpr(">", Varexpr("x"), Integer(10)),
                                    AssignStatement("x",
                                                    BinOpexpr("+",
                                                              Varexpr("x"),
                                                              Integer(1)
                                                              )
                                                    )
                                    )
        while_stmt.eval(env)
        self.assertEqual(env["x"], 0)
