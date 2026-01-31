namespace Amiasea.Loom.Test.BringUp
{
    public readonly struct Token
    {
        public TokenKind Kind { get; }
        public string Value { get; }

        public Token(TokenKind kind, string value)
        {
            Kind = kind;
            Value = value;
        }

        public override string ToString() => $"{Kind}: {Value}";
    }
}