﻿using static LoxNetCore.TokenType;

namespace LoxNetCore
{
	public class Parser
	{
		private readonly List<Token> _tokens;
		private int _current = 0;
		private ErrorHandler _errorHandler;

		public Parser(List<Token> tokens, ErrorHandler errorHandler)
		{
			_tokens = tokens;
			_errorHandler = errorHandler;
		}

		public Expr? Parse()
		{
			try
			{
				return Expression();
			}
			catch (ParseException ex)
			{
				return null;
			}
		}

		private Expr Expression() => Ternary();


		private Expr Ternary()
		{
			Expr expr = Equality();

			if (Match(QUESTION))
			{
				Token op = new Token(TERNARY, "?:", null, Previous().Line);

				Expr trueExpr = Equality();

				if (Match(COLON))
				{
					Expr falseExpr = Equality();
					expr = new Expr.Ternary(expr, op, trueExpr, falseExpr);
				}
				else
				{
					throw Error(new Token(TERNARY, "?:", null, Previous().Line), "Ternary expression requires ':'");
				}
			}

			return expr;
		}

		private Expr Equality()
		{
			Expr expr = Comparison();

			while (Match(BANG_EQUAL, EQUAL_EQUAL))
			{
				Token op = Previous();
				Expr right = Comparison();
				expr = new Expr.Binary(expr, op, right);
			}

			return expr;
		}

		private Expr Comparison()
		{
			Expr expr = Term();

			while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
			{
				Token op = Previous();
				Expr right = Term();
				expr = new Expr.Binary(expr, op, right);
			}

			return expr;
		}

		private Expr Term()
		{
			Expr expr = Factor();

			while (Match(MINUS, PLUS))
			{
				Token op = Previous();
				Expr right = Factor();
				expr = new Expr.Binary(expr, op, right);
			}

			return expr;
		}

		private Expr Factor()
		{
			Expr expr = Unary();

			while (Match(SLASH, STAR))
			{
				Token op = Previous();
				Expr right = Unary();
				expr = new Expr.Binary(expr, op, right);
			}

			return expr;
		}

		private Expr Unary()
		{
			if (Match(BANG, MINUS))
			{
				Token op = Previous();
				Expr right = Unary();
				return new Expr.Unary(op, right);
			}

			return Primary();
		}

		private Expr Primary()
		{
			if (Match(FALSE)) return new Expr.Literal(false);
			if (Match(TRUE)) return new Expr.Literal(true);
			if (Match(NIL)) return new Expr.Literal(null);

			if (Match(NUMBER, STRING)) return new Expr.Literal(Previous().Literal);

			if (Match(LEFT_PAREN))
			{
				Expr expr = Expression();
				Consume(RIGHT_PAREN, "Expect ')' after expression.");
				return new Expr.Grouping(expr);
			}

			throw Error(Peek(), "Expect expression.");
		}

		private bool Match(params TokenType[] types)
		{
			foreach(var type in types)
			{
				if (Check(type))
				{
					Advance();
					return true;
				}
			}
			return false;
		}

		private Token Consume(TokenType type, string message)
		{
			if (Check(type)) return Advance();

			throw Error(Peek(), message);
		}



		private bool Check(TokenType type)
		{
			if (IsAtEnd()) return false;

			return Peek().Type == type;
		}

		private Token Advance()
		{
			if (!IsAtEnd()) _current++;

			return Previous();
		}

		private bool IsAtEnd() => Peek().Type == EOF;
		
		private Token Peek() => _tokens[_current];

		private Token Previous() => _tokens[_current - 1];

		private ParseException Error(Token token, string message)
		{
			_errorHandler.Error(token, message);
			return new ParseException();
		}

		private void Synchronize()
		{
			Advance();

			while(!IsAtEnd())
			{
				if (Previous().Type == SEMICOLON) return;
			}

			switch (Peek().Type)
			{
				case CLASS or FOR or FUN or IF or PRINT or RETURN or VAR or WHILE:
					return;
			}

			Advance();
		}
	}
}
