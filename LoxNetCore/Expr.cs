namespace LoxNetCore
{
	public abstract class Expr
	{
		public class Assign : Expr
		{
			public Token Name { get; }
			public Expr Value { get; }

			public Assign(Token name, Expr value)
			{
				Name = name;
				Value = value;
			}
		}

		public class Ternary : Expr
		{
			public Expr Condition { get; }
			public Token Op { get; }
			public Expr TrueExpr { get; }
			public Expr FalseExpr { get; }

			public Ternary(Expr condition, Token op, Expr trueExpr, Expr falseExpr)
			{
				Condition = condition;
				Op = op;
				TrueExpr = trueExpr;
				FalseExpr = falseExpr;
			}
		}

		public class Binary : Expr
		{
			public Expr Left { get; }
			public Token Op { get; }
			public Expr Right { get; }

			public Binary(Expr left, Token op, Expr right)
			{
				Left = left;
				Op = op;
				Right = right;
			}
		}

		public class Call : Expr
		{
			public Expr Callee { get; }
			public Token Paren { get; }
			public List<Expr> Arguments { get; }

			public Call(Expr callee, Token paren, List<Expr> arguments)
			{
				Callee = callee;
				Paren = paren;
				Arguments = arguments;
			}
		}

		public class Grouping : Expr
		{
			public Expr Expr { get; }

			public Grouping(Expr expression)
			{
				Expr = expression;
			}
		}

		public class Literal : Expr
		{
			public object? Value { get; }

			public Literal(object? value)
			{
				Value = value;
			}
		}

		public class Logical : Expr
		{
			public Expr Left { get; }
			public Token Op { get; }
			public Expr Right { get; }

			public Logical(Expr left, Token op, Expr right)
			{
				Left = left;
				Op = op;
				Right = right;
			}
		}

		public class Variable : Expr
		{
			public Token Name { get; }

			public Variable(Token name)
			{
				Name = name;
			}
		}

		public class Unary : Expr
		{
			public Token Op { get; }
			public Expr Right { get; }

			public Unary(Token op, Expr right)
			{
				Op = op;
				Right = right;
			}
		}

	}
}
