namespace LoxNetCore
{
	public abstract class Stmt
	{
		public class Expression : Stmt
		{
			public Expr Expr { get; }

			public Expression(Expr expression)
			{
				Expr = expression;
			}
		}

		public class Print : Stmt
		{
			public Expr Expr { get; }

			public Print(Expr expression)
			{
				Expr = expression;
			}
		}

	}
}
