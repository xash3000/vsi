using System;
using System.Collections.Generic;

namespace Vsi.Core
{
    public abstract class AstNode : IEquatable<AstNode>
    {
        public abstract object? Eval(Dictionary<string, object?> env);
        public abstract bool Equals(AstNode? other);
        public override bool Equals(object? obj) => Equals(obj as AstNode);
        public abstract override int GetHashCode();
    }

    public class Ast : AstNode
    {
        public List<AstNode> Nodes { get; set; }

        public Ast(List<AstNode>? nodes = null)
        {
            Nodes = nodes ?? new List<AstNode>();
        }

        public override string ToString() => $"Statements([{string.Join(", ", Nodes)}])";

        public override object? Eval(Dictionary<string, object?> env)
        {
            foreach (var node in Nodes)
            {
                node.Eval(env);
            }
            return null;
        }

        public void InsertNode(AstNode node)
        {
            Nodes.Add(node);
        }

        public override bool Equals(AstNode? other)
        {
            if (other is not Ast ast) return false;
            if (Nodes.Count != ast.Nodes.Count) return false;
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (!Equals(Nodes[i], ast.Nodes[i])) return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var node in Nodes)
            {
                hash = hash * 31 + (node?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }

    public class PrintStatement : AstNode
    {
        public AstNode Expr { get; set; }

        public PrintStatement(AstNode expr)
        {
            Expr = expr;
        }

        public override string ToString() => $"PrintStatement({Expr})";

        public override object? Eval(Dictionary<string, object?> env)
        {
            var value = Expr.Eval(env);
            Console.WriteLine(value is bool b ? (b ? "True" : "False") : value);
            return null;
        }

        public override bool Equals(AstNode? other)
        {
            if (other is not PrintStatement p) return false;
            return Equals(Expr, p.Expr);
        }
        public override int GetHashCode() => Expr?.GetHashCode() ?? 0;
    }

    public class Integer : AstNode
    {
        public int I { get; set; }
        public Integer(int i) { I = i; }
        public override string ToString() => $"Integer({I})";
        public override object? Eval(Dictionary<string, object?> env) => I;
        public override bool Equals(AstNode? other) => other is Integer i && I == i.I;
        public override int GetHashCode() => I.GetHashCode();
    }

    public class Float : AstNode
    {
        public double F { get; set; }
        public Float(double f) { F = f; }
        public override string ToString() => $"Float({F.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
        public override object? Eval(Dictionary<string, object?> env) => F;
        public override bool Equals(AstNode? other) => other is Float f && F == f.F;
        public override int GetHashCode() => F.GetHashCode();
    }

    public class VarExpr : AstNode
    {
        public string Name { get; set; }
        public VarExpr(string name) { Name = name; }
        public override string ToString() => $"Varexpr({Name})";
        public override object? Eval(Dictionary<string, object?> env)
        {
            if (env.ContainsKey(Name))
                return env[Name];
            Console.Error.WriteLine($"RuntimeError: Variable {Name} is not defined");
            Environment.Exit(1);
            return null;
        }
        public override bool Equals(AstNode? other) => other is VarExpr v && Name == v.Name;
        public override int GetHashCode() => Name?.GetHashCode() ?? 0;
    }

    public class BinOpExpr : AstNode
    {
        public string Op { get; set; }
        public AstNode Left { get; set; }
        public AstNode Right { get; set; }

        public BinOpExpr(string op, AstNode left, AstNode right)
        {
            Op = op; Left = left; Right = right;
        }

        public override string ToString() => $"BinOpexpr({Op}, {Left}, {Right})";

        public override object? Eval(Dictionary<string, object?> env)
        {
            var leftValue = Left.Eval(env);
            var rightValue = Right.Eval(env);

            double l = Convert.ToDouble(leftValue);
            double r = Convert.ToDouble(rightValue);

            double value = 0;
            if (Op == "+") value = l + r;
            else if (Op == "-") value = l - r;
            else if (Op == "*") value = l * r;
            else if (Op == "/") value = l / r;
            else if (Op == "%") value = l % r;
            else
            {
                Console.Error.WriteLine($"RuntimeError: unknown operator {Op}");
                Environment.Exit(1);
            }

            if (leftValue is int && rightValue is int && (Op != "/" || (l % r == 0)))
                return (int)value;
            
            return value;
        }

        public override bool Equals(AstNode? other)
        {
            if (other is not BinOpExpr b) return false;
            return Op == b.Op && Equals(Left, b.Left) && Equals(Right, b.Right);
        }
        public override int GetHashCode() => HashCode.Combine(Op, Left, Right);
    }

    public class RelOpExpr : AstNode
    {
        public string Op { get; set; }
        public AstNode Left { get; set; }
        public AstNode Right { get; set; }

        public RelOpExpr(string op, AstNode left, AstNode right)
        {
            Op = op; Left = left; Right = right;
        }

        public override string ToString() => $"RelOpexpr({Op}, {Left}, {Right})";

        public override object? Eval(Dictionary<string, object?> env)
        {
            var leftValue = Left.Eval(env);
            var rightValue = Right.Eval(env);

            double l = Convert.ToDouble(leftValue);
            double r = Convert.ToDouble(rightValue);

            bool value = false;
            if (Op == "<") value = l < r;
            else if (Op == "<=") value = l <= r;
            else if (Op == ">") value = l > r;
            else if (Op == ">=") value = l >= r;
            else if (Op == "==") value = l == r;
            else if (Op == "!=") value = l != r;
            else
            {
                Console.Error.WriteLine($"RuntimeError: unknown operator {Op}");
                Environment.Exit(1);
            }
            return value;
        }

        public override bool Equals(AstNode? other)
        {
            if (other is not RelOpExpr b) return false;
            return Op == b.Op && Equals(Left, b.Left) && Equals(Right, b.Right);
        }
        public override int GetHashCode() => HashCode.Combine(Op, Left, Right);
    }

    public class AndExpr : AstNode
    {
        public AstNode Left { get; set; }
        public AstNode Right { get; set; }

        public AndExpr(AstNode left, AstNode right)
        {
            Left = left; Right = right;
        }

        public override string ToString() => $"Andexpr({Left}, {Right})";

        public override object? Eval(Dictionary<string, object?> env)
        {
            var leftValue = Left.Eval(env);
            var rightValue = Right.Eval(env);
            return Convert.ToBoolean(leftValue) && Convert.ToBoolean(rightValue);
        }

        public override bool Equals(AstNode? other)
        {
            if (other is not AndExpr a) return false;
            return Equals(Left, a.Left) && Equals(Right, a.Right);
        }
        public override int GetHashCode() => HashCode.Combine(Left, Right);
    }

    public class OrExpr : AstNode
    {
        public AstNode Left { get; set; }
        public AstNode Right { get; set; }

        public OrExpr(AstNode left, AstNode right)
        {
            Left = left; Right = right;
        }

        public override string ToString() => $"Orexpr({Left}, {Right})";

        public override object? Eval(Dictionary<string, object?> env)
        {
            var leftValue = Left.Eval(env);
            var rightValue = Right.Eval(env);
            return Convert.ToBoolean(leftValue) || Convert.ToBoolean(rightValue);
        }

        public override bool Equals(AstNode? other)
        {
            if (other is not OrExpr a) return false;
            return Equals(Left, a.Left) && Equals(Right, a.Right);
        }
        public override int GetHashCode() => HashCode.Combine(Left, Right);
    }

    public class NotExpr : AstNode
    {
        public AstNode Expr { get; set; }

        public NotExpr(AstNode expr)
        {
            Expr = expr;
        }

        public override string ToString() => $"Notexpr({Expr})";

        public override object? Eval(Dictionary<string, object?> env)
        {
            var value = Expr.Eval(env);
            return !Convert.ToBoolean(value);
        }

        public override bool Equals(AstNode? other)
        {
            if (other is not NotExpr n) return false;
            return Equals(Expr, n.Expr);
        }
        public override int GetHashCode() => Expr?.GetHashCode() ?? 0;
    }

    public class AssignStatement : AstNode
    {
        public string Name { get; set; }
        public AstNode Expr { get; set; }

        public AssignStatement(string name, AstNode expr)
        {
            Name = name; Expr = expr;
        }

        public override string ToString() => $"AssignStatement({Name}, {Expr})";

        public override object? Eval(Dictionary<string, object?> env)
        {
            var value = Expr.Eval(env);
            env[Name] = value;
            return null;
        }

        public override bool Equals(AstNode? other)
        {
            if (other is not AssignStatement a) return false;
            return Name == a.Name && Equals(Expr, a.Expr);
        }
        public override int GetHashCode() => HashCode.Combine(Name, Expr);
    }

    public class IfStatement : AstNode
    {
        public AstNode Condition { get; set; }
        public AstNode TrueStmt { get; set; }
        public AstNode? FalseStmt { get; set; }

        public IfStatement(AstNode condition, AstNode trueStmt, AstNode? falseStmt = null)
        {
            Condition = condition; TrueStmt = trueStmt; FalseStmt = falseStmt;
        }

        public override string ToString() => $"IfStatement({Condition}, {TrueStmt}, {(FalseStmt == null ? "None" : FalseStmt.ToString())})";

        public override object? Eval(Dictionary<string, object?> env)
        {
            var conditionValue = Condition.Eval(env);
            if (Convert.ToBoolean(conditionValue))
            {
                TrueStmt.Eval(env);
            }
            else if (FalseStmt != null)
            {
                FalseStmt.Eval(env);
            }
            return null;
        }

        public override bool Equals(AstNode? other)
        {
            if (other is not IfStatement i) return false;
            return Equals(Condition, i.Condition) && Equals(TrueStmt, i.TrueStmt) && Equals(FalseStmt, i.FalseStmt);
        }
        public override int GetHashCode() => HashCode.Combine(Condition, TrueStmt, FalseStmt);
    }

    public class WhileStatement : AstNode
    {
        public AstNode Condition { get; set; }
        public AstNode Body { get; set; }

        public WhileStatement(AstNode condition, AstNode body)
        {
            Condition = condition; Body = body;
        }

        public override string ToString() => $"WhileStatement({Condition}, {Body})";

        public override object? Eval(Dictionary<string, object?> env)
        {
            var conditionValue = Condition.Eval(env);
            while (Convert.ToBoolean(conditionValue))
            {
                Body.Eval(env);
                conditionValue = Condition.Eval(env);
            }
            return null;
        }

        public override bool Equals(AstNode? other)
        {
            if (other is not WhileStatement w) return false;
            return Equals(Condition, w.Condition) && Equals(Body, w.Body);
        }
        public override int GetHashCode() => HashCode.Combine(Condition, Body);
    }
}