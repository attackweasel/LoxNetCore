var lox = new Lox();

switch (args.Length)
{
	case > 1:
		Console.WriteLine("Usage: nlox [script]");
		Environment.Exit(64);
		break;
	case 1:
		lox.RunFile(args[0]);
		break;
	default:
		lox.RunPrompt();
		break;
}






