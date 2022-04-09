using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxNetCore
{
    public class Env
    {
        private Dictionary<string, object?> values = new();

        public void Define(string name, object? value) =>
            values[name] = value;

        public object? Get(Token name) => values.ContainsKey(name.Lexeme)
            ? values[name.Lexeme]
            : throw new RuntimeException(name,
                $"Undefined variable '{name.Lexeme}'.");
    }
}
