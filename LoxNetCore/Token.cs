using LoxNetCore;

internal class Token
{
	internal TokenType Type { init; get; }
	internal string Lexeme { init; get; }
	internal object? Literal { init; get; }
	internal int Line { init; get; }

	internal Token(TokenType type, string lexeme, object? literal, int line)
	{
		Type = type;
		Lexeme = lexeme;
		Literal = literal;
		Line = line;
	}

	public override string ToString() =>
		$"{Type} {Lexeme} {Literal}";
}