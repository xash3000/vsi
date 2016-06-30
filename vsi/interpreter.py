class Interpreter:

    def __init__(self, AST, env=None):
        self.env = {} if env is None else env
        self.ast = AST

    def interpret(self):
        self.ast.eval(self.env)
