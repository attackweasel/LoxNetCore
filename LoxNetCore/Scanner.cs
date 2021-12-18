using LoxNetCore;
using static LoxNetCore.TokenType;

public class Scanner
{
	private readonly string _source;
	private readonly ErrorHandler _errorHandler;
	private readonly List<Token> _tokens = new List<Token>();
	private int _start = 0;
	private int _current = 0;
	private int _line = 0;

	private readonly Dictionary<string, TokenType> _keywords = new ()
	{
		{ "and", AND },
		{ "class", CLASS },
		{ "else", ELSE },
		{ "false", FALSE },
		{ "for", FOR },
		{ "fun", FUN },
		{ "if", IF },
		{ "nil", NIL },
		{ "or", OR },
		{ "print", PRINT },
		{ "return", RETURN },
		{ "super", SUPER },
		{ "this", THIS },
		{ "true", TRUE },
		{ "var", VAR },
		{ "while", WHILE }
	};
	
	public Scanner(string source, ErrorHandler errorHandler)
	{
		_source = source;
		_errorHandler = errorHandler;
	}

	public List<Token> ScanTokens()
	{
		while (!IsAtEnd())
		{
			// We are at the beginning of the next lexeme
			_start = _current;
			ScanToken();
		}

		_tokens.Add(new Token(EOF, "", null, _line));
		return _tokens;
	}

	private void ScanToken()
	{
		char c = Advance();

		switch(c)
		{
			case '(':
				AddToken(LEFT_PAREN);
				break;
			case ')':
				AddToken(RIGHT_PAREN);
				break;
			case '{': 
				AddToken(LEFT_BRACE); 
				break;
			case '}': 
				AddToken(RIGHT_BRACE); 
				break;
			case ',': 
				AddToken(COMMA); 
				break;
			case '.': 
				AddToken(DOT); 
				break;
			case '-': 
				AddToken(MINUS); 
				break;
			case '+': 
				AddToken(PLUS); 
				break;
			case ';': 
				AddToken(SEMICOLON); 
				break;
			case '*': 
				AddToken(STAR); 
				break;
			case '!':
				AddToken(MatchNextChar('=') ? BANG_EQUAL : BANG);
				break;
			case '=':
				AddToken(MatchNextChar('=') ? EQUAL_EQUAL : EQUAL);
				break;
			case '<':
				AddToken(MatchNextChar('<') ? LESS_EQUAL : LESS);
				break;
			case '>':
				AddToken(MatchNextChar('>') ? GREATER_EQUAL : GREATER);
				break;
			case '/':
				if (MatchNextChar('/'))
				{
					ScanComment();
				}
				else
					AddToken(SLASH);
				break;
			
			case ' ':
			case '\r':
			case '\t':
				// Ignore whitespace
				break;

			case '\n':
				_line++;
				break;

			case '"':
				ScanString();
				break;

			case >= '0' and <= '9':
				ScanNumber();
				break;

			case >= 'a' and <= 'z'
				 or >= 'A' and <= 'Z'
				 or '_':
				ScanIdentifier();
				break;

			default:
				_errorHandler.Error(_line, "Unexpected character."); break;
		}
	}

	private bool MatchNextChar(char expected)
	{
		if (IsAtEnd())
			return false;

		if (_source[_current] != expected)
			return false;

		_current++;
		return true;
	}

	private char PeekNextChar() => IsAtEnd() ? '\0' : _source[_current];

	private char PeekCharAfterNext() =>
		(_current + 1 >= _source.Length) ? '\0' : _source[_current + 1];

	private bool IsValidIdentifierCharacter(char c) =>
		char.IsLetterOrDigit(c) || c == '_';

	private bool IsAtEnd() => _current >= _source.Length;

	private char Advance() => _source[_current++];

	private void AddToken(TokenType type) => AddToken(type, null);

	private void AddToken(TokenType type, Object? literal)
	{
		var text = _source[_start.._current];
		_tokens.Add(new Token(type, text, literal, _line));
	}

	private void ScanComment()
	{
		// A comment goes until the end of the line
		while (PeekNextChar() != '\n' && !IsAtEnd())
			Advance();
	}

	private void ScanString()
	{
		while (PeekNextChar() != '"' && !IsAtEnd())
		{
			if (PeekNextChar() != '\n')
				_line++;
			Advance();
		}

		if (IsAtEnd())
		{
			_errorHandler.Error(_line, "Unterminated string.");
			return;
		}

		Advance(); // The closing "

		// Trim the surrounding quotes
		var value = _source[(_start + 1)..(_current - 1)];
		AddToken(STRING, value);
	}

	private void ScanNumber()
	{
		while (char.IsDigit(PeekNextChar()))
			Advance();

		// Look for a fractional part
		if (PeekNextChar() == '.' && char.IsDigit(PeekCharAfterNext()))
		{
			Advance();

			while (char.IsDigit(PeekNextChar()))
				Advance();
		}

		AddToken(NUMBER, double.Parse(_source[_start.._current]));
	}

	private void ScanIdentifier()
	{
		while (IsValidIdentifierCharacter(PeekNextChar()))
			Advance();

		var text = _source[_start.._current];

		if (_keywords.ContainsKey(text))
			AddToken(_keywords[text]);
		else
			AddToken(IDENTIFIER);
	}
}