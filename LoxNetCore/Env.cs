namespace LoxNetCore
{
    public class Env
    {
        private readonly Dictionary<string, object?> _values = new();
        private readonly Env? _enclosing;

        public Env(Env? enclosing = null) => _enclosing = enclosing;

        public void Define(string name, object? value) =>
            _values[name] = value;

        public object? Get(Token name)
        {
            if (_values.ContainsKey(name.Lexeme))
                return _values[name.Lexeme];
            if (_enclosing is not null)
                return _enclosing.Get(name);
            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Assign(Token name, object? value)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                _values[name.Lexeme] = value;
                return;
            }

            if (_enclosing is not null)
            {
                _enclosing.Assign(name, value);
                return;
            }
                
            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}
