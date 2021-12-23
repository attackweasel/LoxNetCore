namespace LoxNetCore
{
	public class ErrorHandler
	{
		public bool HadError { get; set; } = false;

		public bool HadRuntimeError { get; set; } = false;

		public void Error(int line, string message) => Report(line, "", message);

		public void Report(int line, string where, string message)
		{
			Console.Error.WriteLine($"[line {line}] Error {where}: {message}");

			HadError = true;
		}

		public void Error(Token token, string message)
		{
			if (token.Type == TokenType.EOF)
				Report(token.Line, "at end", message);
			else
				Report(token.Line, $"at '{token.Lexeme}'", message);
		}

		public void RuntimeError(RuntimeException exception)
		{
			Console.Error.WriteLine(exception.Message);
			Console.Error.WriteLine($"[line {exception.Token.Line}]");
			
			HadRuntimeError = true;
		}
	}
}
