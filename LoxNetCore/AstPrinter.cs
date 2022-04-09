using System.Text;

namespace LoxNetCore
{
	public static class AstPrinter
	{
		public static string? Print(Expr expr) => expr switch
		{
			Expr.Binary binary => Parenthesize(binary.Op.Lexeme, binary.Left, binary.Right),
			Expr.Grouping grouping => Parenthesize("group", grouping.Expr),
			Expr.Literal literal => literal.Value is null ? "nil" : literal.Value.ToString(),
			Expr.Unary unary => Parenthesize(unary.Op.Lexeme, unary.Right),
			_ => throw new NotImplementedException()
		};

		private static string Parenthesize(string name, params Expr[] exprs)
		{
			var sb = new StringBuilder();

			sb.Append('(').Append(name);
			foreach (var expr in exprs)
			{
				sb.Append(' ');
				sb.Append(Print(expr));
			}
			sb.Append(')');

			return sb.ToString();
		}
	}
}
