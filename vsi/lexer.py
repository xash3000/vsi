import re
import sys


class Token:

    def __init__(self, value, _type, specific_type=None, precedence=0):
        self.value = value
        self.type = _type
        self.specific_type = specific_type
        self.precedence = precedence

    def __repr__(self):
        attrs = [self.value, self.type, self.specific_type, self.precedence]
        return "Token({}, {}, {}, {})".format(*attrs)


class Lexer:

    def __init__(self, chars, tokens_exprs):
        self.chars = chars
        self.tokens_exprs = tokens_exprs
        self.tokens = []
        self.pos = 0

    def lex(self):
        while self.pos < len(self.chars):
            match = False
            for token_expr in self.tokens_exprs:
                pattern, _type, specific_type, precedence = token_expr
                regex = re.compile(pattern)
                match = regex.match(self.chars, self.pos)
                if match:
                    try:
                        text = match.groups()[0]
                    except IndexError:
                        # comments and whitespaces
                        pass
                    if _type:
                        token = Token(text, _type, specific_type, precedence)
                        self.tokens.append(token)
                    break
            if not match:
                currnet_pos = self.chars[self.pos]
                sys.stderr.write('Invalid character: {}\n'.format(currnet_pos))
                sys.exit(1)
            else:
                self.pos = match.end(0)
        self.prepare_tokens()
        return self.tokens

    def prepare_tokens(self):
        eof = Token("EOF", "RESERVED", "SYMBOL", 0)
        self.tokens.append(eof)
        for token in self.tokens:
            if token.specific_type == "INT":
                token.value = int(token.value)
