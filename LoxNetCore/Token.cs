using LoxNetCore;

public class Token
{
	public TokenType Type { init; get; }
	public string Lexeme { init; get; }
	public object? Literal { init; get; }
	public int Line { init; get; }

	public Token(TokenType type, string lexeme, object? literal, int line)
	{
		Type = type;
		Lexeme = lexeme;
		Literal = literal;
		Line = line;
	}

	public override string ToString() =>
		$"{Type} {Lexeme} {Literal}";
}