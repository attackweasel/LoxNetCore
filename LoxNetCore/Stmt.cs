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

		public class Var : Stmt
		{
			public Token Name { get; }
			public Expr? Initializer { get; }

			public Var(Token name, Expr? initializer)
			{
				Name = name;
				Initializer = initializer;
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

		public class Block : Stmt
		{
			public List<Stmt?> Statements { get; }

			public Block(List<Stmt?> statements)
			{
				Statements = statements;
			}
		}

	}
}
