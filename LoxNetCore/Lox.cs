﻿using LoxNetCore;

public class Lox
{
	private static Interpreter _interpreter = new Interpreter();
	private ErrorHandler _errorHandler = new ErrorHandler();

	public void RunFile(string path)
	{
		Run(File.ReadAllText(path));

		if (_errorHandler.HadError) Environment.Exit(65);
		if (_errorHandler.HadRuntimeError) Environment.Exit(70);
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
		var tokens = scanner.ScanTokens();

		var parser = new Parser(tokens, _errorHandler);
		var statements = parser.Parse();

		// Stop if there was a syntax error
		if (_errorHandler.HadError) return;

		_interpreter.Interpret(statements);
	}
}
