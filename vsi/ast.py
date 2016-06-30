import sys


class AST:

    def __init__(self, nodes=None):
        self.nodes = [] if nodes is None else nodes

    def __repr__(self):
        return "Statements({})".format(self.nodes)

    def eval(self, env):
        for node in self.nodes:
            node.eval(env)

    def insert_node(self, node):
        self.nodes.append(node)


class printStatement:

    def __init__(self, expr):
        self.expr = expr

    def __repr__(self):
        return "printStatement({})".format(self.expr)

    def eval(self, env):
        value = self.expr.eval(env)
        sys.stdout.write(str(value))
        sys.stdout.write("\n")


class Integer:

    def __init__(self, i):
        self.i = i

    def __repr__(self):
        return "Integer({})".format(self.i)

    def eval(self, env):
        return self.i


class Varexpr:

    def __init__(self, name):
        self.name = name

    def __repr__(self):
        return "Varexpr({})".format(self.name)

    def eval(self, env):
        if self.name in env:
            return env[self.name]
        else:
            err = "RuntimeError: Variable {} is not defined".format(self.name)
            sys.stderr.write(err + "\n")
            sys.exit(1)


class BinOpexpr:

    def __init__(self, op, left, right):
        self.op = op
        self.right = right
        self.left = left

    def __repr__(self):
        return "BinOpexpr({}, {}, {})".format(self.op, self.right, self.left)

    def eval(self, env):
        left_value = self.left.eval(env)
        right_value = self.right.eval(env)
        if self.op == '+':
            value = left_value + right_value
        elif self.op == '-':
            value = left_value - right_value
        elif self.op == '*':
            value = left_value * right_value
        elif self.op == '/':
            value = left_value / right_value
        elif self.op == "%":
            value = left_value % right_value
        else:
            err = "RuntimeError: unknown operator {}".format(self.op)
            sys.stderr.write(err + "\n")
            sys.exit(1)
        return value


class RelOpexpr:

    def __init__(self, op, left, right):
        self.op = op
        self.left = left
        self.right = right

    def __repr__(self):
        return "RelOpexpr({}, {}, {})".format(self.op, self.left, self.right)

    def eval(self, env):
        left_value = self.left.eval(env)
        right_value = self.right.eval(env)
        if self.op == '<':
            value = left_value < right_value
        elif self.op == '<=':
            value = left_value <= right_value
        elif self.op == '>':
            value = left_value > right_value
        elif self.op == '>=':
            value = left_value >= right_value
        elif self.op == '==':
            value = left_value == right_value
        elif self.op == '!=':
            value = left_value != right_value
        else:
            err = "RuntimeError: unknown operator {}".format(self.op)
            sys.stderr.write(err + "\n")
            sys.exit(1)
        return value


class Andexpr:

    def __init__(self, left, right):
        self.left = left
        self.right = right

    def __repr__(self):
        return "Andexpr({}, {})".format(self.left, self.right)

    def eval(self, env):
        left_value = self.left.eval(env)
        right_value = self.right.eval(env)
        return left_value and right_value


class Orexpr:

    def __init__(self, left, right):
        self.left = left
        self.right = right

    def __repr__(self):
        return "Orexpr({}, {})".format(self.left, self.right)

    def eval(self, env):
        left_value = self.left.eval(env)
        right_value = self.right.eval(env)
        return left_value or right_value


class Notexpr:

    def __init__(self, expr):
        self.expr = expr

    def __repr__(self):
        return "Notexpr({})".format(self.expr)

    def eval(self, env):
        value = self.expr.eval(env)
        return not value


class AssignStatement:

    def __init__(self, name, expr):
        self.name = name
        self.expr = expr

    def __repr__(self):
        return "AssignStatement({}, {})".format(self.name, self.expr)

    def eval(self, env):
        value = self.expr.eval(env)
        env[self.name] = value


class IfStatement:

    def __init__(self, condition, true_stmt, false_stmt):
        self.condition = condition
        self.true_stmt = true_stmt
        self.false_stmt = false_stmt

    def __repr__(self):
        return "IfStatement({}, {}, {})".format(self.condition,
                                                self.true_stmt,
                                                self.false_stmt
                                                )

    def eval(self, env):
        condition_value = self.condition.eval(env)
        if condition_value:
            self.true_stmt.eval(env)
        else:
            if self.false_stmt:
                self.false_stmt.eval(env)


class WhileStatement:

    def __init__(self, condition, body):
        self.condition = condition
        self.body = body

    def __repr__(self):
        return "WhileStatement({}, {})".format(self.condition, self.body)

    def eval(self, env):
        condition_value = self.condition.eval(env)
        while condition_value:
            self.body.eval(env)
            condition_value = self.condition.eval(env)
