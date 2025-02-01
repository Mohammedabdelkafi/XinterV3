namespace XinterV3
{
    class Token
    {
        public string Type { get; }
        public string Value { get; }

        public Token(string type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        public override string ToString()
        {
            return $"{Type}({Value})";
        }
    }
}
