using LoxNetCore;

var lox = new Lox();

/*
var expr = new Expr.Binary(
	new Expr.Unary(
		new Token(TokenType.MINUS, "-", null, 1),
		new Expr.Literal(123)),
	new Token(TokenType.STAR, "*", null, 1),
	new Expr.Grouping(new Expr.Literal(45.67)));

Console.WriteLine(AstPrinter.Print(expr));
*/


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






