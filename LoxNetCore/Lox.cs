using LoxNetCore;

public class Lox
{
	private ErrorHandler _errorHandler = new ErrorHandler();

	public void RunFile(string path)
	{
		Run(File.ReadAllText(path));

		if (_errorHandler.HadError) Environment.Exit(65);
	}

	public void RunPrompt()
	{
		while (true)
		{
			Console.Write("> ");
			var line = Console.ReadLine();
			if (line is null) break;

			Run(line);

			_errorHandler.HadError = false;
		}
	}

	private void Run(string source)
	{
		var scanner = new Scanner(source, _errorHandler);
		List<Token> tokens = scanner.ScanTokens();

		Parser parser = new Parser(tokens, _errorHandler);
		Expr expression = parser.Parse();

		// Stop if there was a syntax error
		if (_errorHandler.HadError) return;

		Console.WriteLine(AstPrinter.Print(expression));
	}
}
