namespace LoxNetCore
{
    public class Env
    {
        private Dictionary<string, object?> _values = new();

        public void Define(string name, object? value) =>
            _values[name] = value;

        public object? Get(Token name) => _values.ContainsKey(name.Lexeme)
            ? _values[name.Lexeme]
            : throw new RuntimeException(name,
                $"Undefined variable '{name.Lexeme}'.");

        public void Assign(Token name, object? value)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                _values[name.Lexeme] = value;
                return;
            }

            throw new RuntimeException(
                name,
                $"Undefined variable '{name.Lexeme}'.");
        }
    }
}
