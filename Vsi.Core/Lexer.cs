using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Vsi.Core
{
    public class TokenExpr
    {
        public string Pattern { get; set; }
        public TokenType Type { get; set; }
        public TokenSpecificType SpecificType { get; set; }
        public int Precedence { get; set; }

        public TokenExpr(string pattern, TokenType type, TokenSpecificType specificType, int precedence)
        {
            Pattern = pattern;
            Type = type;
            SpecificType = specificType;
            Precedence = precedence;
        }
    }

    public class Lexer
    {
        public string Chars { get; }
        public static readonly List<TokenExpr> TokensExprs = new List<TokenExpr>
        {
            new TokenExpr(@"[ \n\t]+", TokenType.None, TokenSpecificType.None, 0),
            new TokenExpr(@"#[^\n]*", TokenType.None, TokenSpecificType.None, 0),
            new TokenExpr(@"(;)", TokenType.Reserved, TokenSpecificType.Symbol, 0),
            new TokenExpr(@"(:=)", TokenType.Reserved, TokenSpecificType.Operator, 0),
            new TokenExpr(@"(\()", TokenType.Reserved, TokenSpecificType.Operator, 9),
            new TokenExpr(@"(\))", TokenType.Reserved, TokenSpecificType.Operator, 0),
            new TokenExpr(@"(\+)", TokenType.Reserved, TokenSpecificType.Operator, 2),
            new TokenExpr(@"(-)", TokenType.Reserved, TokenSpecificType.Operator, 2),
            new TokenExpr(@"(\*)", TokenType.Reserved, TokenSpecificType.Operator, 3),
            new TokenExpr(@"(/)", TokenType.Reserved, TokenSpecificType.Operator, 3),
            new TokenExpr(@"(%)", TokenType.Reserved, TokenSpecificType.Operator, 4),
            new TokenExpr(@"(<=)", TokenType.Reserved, TokenSpecificType.Operator, 2),
            new TokenExpr(@"(<)", TokenType.Reserved, TokenSpecificType.Operator, 2),
            new TokenExpr(@"(>=)", TokenType.Reserved, TokenSpecificType.Operator, 2),
            new TokenExpr(@"(>)", TokenType.Reserved, TokenSpecificType.Operator, 2),
            new TokenExpr(@"(==)", TokenType.Reserved, TokenSpecificType.Operator, 2),
            new TokenExpr(@"(!=)", TokenType.Reserved, TokenSpecificType.Operator, 2),
            new TokenExpr(@" (and) ", TokenType.Reserved, TokenSpecificType.Operator, 1),
            new TokenExpr(@" (or) ", TokenType.Reserved, TokenSpecificType.Operator, 1),
            new TokenExpr(@"(not) ", TokenType.Reserved, TokenSpecificType.Operator, 1),
            new TokenExpr(@"(print) ", TokenType.Reserved, TokenSpecificType.Statement, 0),
            new TokenExpr(@"(if) ", TokenType.Reserved, TokenSpecificType.Statement, 0),
            new TokenExpr(@"(then)", TokenType.Reserved, TokenSpecificType.Statement, 0),
            new TokenExpr(@"(else) ", TokenType.Reserved, TokenSpecificType.Statement, 0),
            new TokenExpr(@"(while) ", TokenType.Reserved, TokenSpecificType.Statement, 0),
            new TokenExpr(@"(do)[ \n\t]{1,}", TokenType.Reserved, TokenSpecificType.Statement, 0),
            new TokenExpr(@"(done)[ \n\t]{1,}", TokenType.Reserved, TokenSpecificType.Statement, 0),
            new TokenExpr(@"([0-9]+\.[0-9]+)", TokenType.Number, TokenSpecificType.Float, 0),
            new TokenExpr(@"([0-9]+)", TokenType.Number, TokenSpecificType.Int, 0),
            new TokenExpr(@"([_A-Za-z][A-Za-z0-9_]*)", TokenType.Id, TokenSpecificType.None, 0)
        };

        public List<Token> Tokens { get; private set; }
        public int Pos { get; private set; }

        public Lexer(string chars)
        {
            Chars = chars;
            Tokens = new List<Token>();
            Pos = 0;
        }

        public override string ToString()
        {
            return $"Lexer({Chars}, {Tokens}, {Pos})";
        }

        public List<Token> Lex()
        {
            while (Pos < Chars.Length)
            {
                bool match = false;
                foreach (var tokenExpr in TokensExprs)
                {
                    var regex = new Regex("^" + tokenExpr.Pattern);
                    var m = regex.Match(Chars.Substring(Pos));
                    if (m.Success)
                    {
                        string? text = null;
                        if (m.Groups.Count > 1)
                        {
                            text = m.Groups[1].Value;
                        }

                        if (tokenExpr.Type != TokenType.None)
                        {
                            var token = new Token(text, tokenExpr.Type, tokenExpr.SpecificType, tokenExpr.Precedence);
                            Tokens.Add(token);
                        }
                        
                        Pos += m.Length;
                        match = true;
                        break;
                    }
                }

                if (!match)
                {
                    char currentChar = Chars[Pos];
                    Console.Error.WriteLine($"Invalid character: {currentChar}");
                    Environment.Exit(1);
                }
            }
            PrepareTokens();
            return Tokens;
        }

        public void PrepareTokens()
        {
            var eof = new Token("EOF", TokenType.Reserved, TokenSpecificType.Symbol, 0);
            Tokens.Add(eof);
            foreach (var token in Tokens)
            {
                if (token.SpecificType == TokenSpecificType.Int && token.Value != null)
                {
                    token.Value = int.Parse((string)token.Value);
                }
                else if (token.SpecificType == TokenSpecificType.Float && token.Value != null)
                {
                    token.Value = double.Parse((string)token.Value, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
        }
    }
}
