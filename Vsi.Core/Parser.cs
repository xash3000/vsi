using System;
using System.Collections.Generic;

namespace Vsi.Core
{
    public class Parser
    {
        public List<Token> Tokens { get; }
        public int Pos { get; private set; }
        public Ast Ast { get; }

        public Parser(List<Token> tokens, int pos = 0)
        {
            Tokens = tokens;
            Pos = pos;
            Ast = new Ast();
        }

        public override string ToString()
        {
            return $"Parser([{string.Join(", ", Tokens)}], {Pos}, {Ast})";
        }

        public Ast Parse()
        {
            ParseStatements(Ast);
            return Ast;
        }

        public void ParseStatements(Ast ast)
        {
            while (Pos <= Tokens.Count)
            {
                if (FoundOneOf(new[] { "EOF", "else" }))
                {
                    break;
                }
                else if (Found("if"))
                {
                    Advance();
                    var astNode = ParseIfStatement();
                    if (astNode != null) ast.InsertNode(astNode);
                }
                else if (Found("while"))
                {
                    Advance();
                    var astNode = ParseWhileStatement();
                    if (astNode != null) ast.InsertNode(astNode);
                }
                else if (Found(TokenType.Id))
                {
                    AstNode? astNode;
                    if (TokenAhead()?.Value?.ToString() == ":=")
                    {
                        astNode = ParseVarAssignment();
                    }
                    else
                    {
                        astNode = ParseExpr();
                    }
                    if (astNode != null) ast.InsertNode(astNode);
                }
                else if (Found("print"))
                {
                    Advance();
                    var astNode = ParsePrintStatement();
                    if (astNode != null) ast.InsertNode(astNode);
                }
                else
                {
                    break;
                }
            }
        }

        public bool Found(string token)
        {
            if (Pos >= Tokens.Count) return false;
            return Tokens[Pos].Value?.ToString() == token;
        }

        public bool Found(TokenType type)
        {
            if (Pos >= Tokens.Count) return false;
            return Tokens[Pos].Type == type;
        }

        public bool Found(TokenSpecificType specificType)
        {
            if (Pos >= Tokens.Count) return false;
            return Tokens[Pos].SpecificType == specificType;
        }

        public bool FoundOneOf(string[] tokens)
        {
            foreach (var token in tokens)
            {
                if (Found(token))
                    return true;
            }
            return false;
        }

        public bool FoundOneOf(TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Found(type))
                    return true;
            }
            return false;
        }

        public bool FoundOneOf(TokenSpecificType[] specificTypes)
        {
            foreach (var specificType in specificTypes)
            {
                if (Found(specificType))
                    return true;
            }
            return false;
        }

        public bool Expect(string token)
        {
            if (Found(token)) return true;
            var currentToken = Tokens[Pos].Value;
            Console.Error.WriteLine($"SyntaxError: expecting {token} found {currentToken}");
            Environment.Exit(1);
            return false;
        }

        public bool Expect(TokenType type)
        {
            if (Found(type)) return true;
            var currentToken = Tokens[Pos].Value;
            Console.Error.WriteLine($"SyntaxError: expecting {type} found {currentToken}");
            Environment.Exit(1);
            return false;
        }

        public bool Expect(TokenSpecificType specificType)
        {
            if (Found(specificType)) return true;
            var currentToken = Tokens[Pos].Value;
            Console.Error.WriteLine($"SyntaxError: expecting {specificType} found {currentToken}");
            Environment.Exit(1);
            return false;
        }

        public bool ExpectOneOf(string[] tokens)
        {
            if (FoundOneOf(tokens)) return true;
            var currentToken = Tokens[Pos].Value;
            Console.Error.WriteLine($"SyntaxError: expecting one of [{string.Join(", ", tokens)}] found {currentToken}");
            Environment.Exit(1);
            return false;
        }

        public bool ExpectOneOf(TokenType[] types)
        {
            if (FoundOneOf(types)) return true;
            var currentToken = Tokens[Pos].Value;
            Console.Error.WriteLine($"SyntaxError: expecting one of [{string.Join(", ", types)}] found {currentToken}");
            Environment.Exit(1);
            return false;
        }

        public bool ExpectOneOf(TokenSpecificType[] specificTypes)
        {
            if (FoundOneOf(specificTypes)) return true;
            var currentToken = Tokens[Pos].Value;
            Console.Error.WriteLine($"SyntaxError: expecting one of [{string.Join(", ", specificTypes)}] found {currentToken}");
            Environment.Exit(1);
            return false;
        }

        public bool Optional(string token)
        {
            if (Found(token)) return true;
            return false;
        }

        public void Advance(int by = 1)
        {
            Pos += by;
        }

        public Token? TokenAhead(int by = 1)
        {
            if (Pos + by < Tokens.Count)
                return Tokens[Pos + by];
            return null;
        }

        public AstNode? ParseIfStatement()
        {
            var condition = ParseExpr();
            Expect("then");
            Advance();
            var trueStmt = new Ast();
            ParseStatements(trueStmt);
            AstNode? falseStmt = null;
            if (Optional("else"))
            {
                Advance();
                falseStmt = new Ast();
                ParseStatements((Ast)falseStmt);
            }
            Expect("done");
            Advance();
            if (condition == null) return null;
            return new IfStatement(condition, trueStmt, falseStmt);
        }

        public AstNode? ParseWhileStatement()
        {
            var condition = ParseExpr();
            Expect("do");
            Advance();
            var body = new Ast();
            ParseStatements(body);
            Expect("done");
            Advance();
            if (condition == null) return null;
            return new WhileStatement(condition, body);
        }

        public AstNode? ParseVarAssignment()
        {
            var varName = Tokens[Pos].Value?.ToString();
            Advance();
            Expect(":=");
            Advance();
            var varValue = ParseExpr();
            Expect(";");
            Advance();
            if (varName == null || varValue == null) return null;
            return new AssignStatement(varName, varValue);
        }

        public AstNode? ParsePrintStatement()
        {
            var expr = ParseExpr();
            Expect(";");
            Advance();
            if (expr == null) return null;
            return new PrintStatement(expr);
        }

        public AstNode? ParseExpr()
        {
            var output = new List<Token>();
            var opStack = new Stack<Token>();

            while (Pos < Tokens.Count)
            {
                var currentToken = Tokens[Pos];
                if (currentToken.SpecificType == TokenSpecificType.Operator)
                {
                    while (opStack.Count > 0 && currentToken.Precedence <= opStack.Peek().Precedence)
                    {
                        output.Add(opStack.Pop());
                    }
                    opStack.Push(currentToken);
                }
                else if (currentToken.Type == TokenType.Number || currentToken.Type == TokenType.Id)
                {
                    output.Add(currentToken);
                }
                else
                {
                    break;
                }
                Advance();
            }

            while (opStack.Count > 0)
            {
                output.Add(opStack.Pop());
            }

            return CreateAstFromPostfix(output);
        }

        public AstNode? CreateAstFromPostfix(List<Token> postfix)
        {
            var stack = new Stack<AstNode>();
            foreach (var token in postfix)
            {
                if (token.SpecificType == TokenSpecificType.Operator)
                {
                    var op = token.Value?.ToString();
                    if (op == null) continue;

                    var operand2 = stack.Pop();
                    AstNode? operand1 = null;
                    if (op != "not")
                    {
                        operand1 = stack.Pop();
                    }
                    var astNode = CreateAstFromExpr(op, operand2, operand1);
                    if (astNode != null)
                        stack.Push(astNode);
                }
                else if (token.SpecificType == TokenSpecificType.Int && token.Value != null)
                {
                    stack.Push(new Integer((int)token.Value));
                }
                else if (token.SpecificType == TokenSpecificType.Float && token.Value != null)
                {
                    stack.Push(new Float((double)token.Value));
                }
                else if (token.Type == TokenType.Id && token.Value != null)
                {
                    stack.Push(new VarExpr(token.Value.ToString()!));
                }
            }

            if (stack.Count == 0) return null;

            var final = stack.Peek();
            return final;
        }

        public AstNode? CreateAstFromExpr(string op, AstNode operand2, AstNode? operand1)
        {
            if (operand1 != null && (op == "+" || op == "-" || op == "*" || op == "/" || op == "%"))
                return new BinOpExpr(op, operand1, operand2);
            else if (operand1 != null && (op == "<" || op == ">" || op == "<=" || op == ">=" || op == "!=" || op == "=="))
                return new RelOpExpr(op, operand1, operand2);
            else if (operand1 != null && op == "and")
                return new AndExpr(operand1, operand2);
            else if (operand1 != null && op == "or")
                return new OrExpr(operand1, operand2);
            else if (op == "not")
                return new NotExpr(operand2);
            else
            {
                Console.Error.WriteLine($"SyntaxError: Invalid operator {op}");
                Environment.Exit(1);
                return null;
            }
        }
    }
}
