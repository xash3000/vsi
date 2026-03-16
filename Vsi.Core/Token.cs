using System;

namespace Vsi.Core
{
    public enum TokenType
    {
        None,
        Reserved,
        Number,
        Id
    }

    public enum TokenSpecificType
    {
        None,
        Symbol,
        Operator,
        Statement,
        Int,
        Float
    }

    public class Token : IEquatable<Token>
    {
        public object? Value { get; set; }
        public TokenType Type { get; set; }
        public TokenSpecificType SpecificType { get; set; }
        public int Precedence { get; set; }

        public Token(object? value, TokenType type, TokenSpecificType specificType = TokenSpecificType.None, int precedence = 0)
        {
            Value = value;
            Type = type;
            SpecificType = specificType;
            Precedence = precedence;
        }

        public override string ToString()
        {
            return $"Token({Value}, {Type}, {SpecificType}, {Precedence})";
        }

        public bool Equals(Token? other)
        {
            if (other is null) return false;
            return Equals(Value, other.Value) &&
                   Type == other.Type &&
                   SpecificType == other.SpecificType &&
                   Precedence == other.Precedence;
        }

        public override bool Equals(object? obj) => Equals(obj as Token);

        public override int GetHashCode() => HashCode.Combine(Value, Type, SpecificType, Precedence);
    }
}