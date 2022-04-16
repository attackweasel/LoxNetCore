using LoxNetCore;
using static LoxNetCore.TokenType;

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
        Scanner scanner = new Scanner(source, _errorHandler);
        List<Token> tokens = scanner.ScanTokens();

        CheckForAndPrepareReplExpression(tokens);

        Parser parser = new Parser(tokens, _errorHandler);
        List<Stmt?> statements = parser.Parse();

        // Stop if there was a syntax error
        if (_errorHandler.HadError) return;

        _interpreter.Interpret(statements);
    }

    private static void CheckForAndPrepareReplExpression(List<Token> tokens)
    {
        if (tokens.Count > 0
            && !tokens.Exists(token => token.Type == SEMICOLON)
            && !tokens.Exists(token => token.Line > 0))
        {
            tokens.Insert(0, new Token(PRINT, "print", null, 1));
            tokens.Insert(tokens.Count - 1, new Token(SEMICOLON, ";", null, 1));
        }
    }
}
