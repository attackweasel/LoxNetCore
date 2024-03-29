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

        public List<Stmt?> Parse()
        {
            List<Stmt?> statements = new List<Stmt?>();

            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;
        }

        private Stmt? Declaration()
        {
            try
            {
                if (Match(VAR))
                    return VarDeclaration();
                return Statement();
            }
            catch (ParseException)
            {
                Synchronize();
                return null;
            }
        }

        private Stmt VarDeclaration()
        {
            Token name = Consume(IDENTIFIER, "Expect variable name.");

            Expr? initializer = Match(EQUAL) ? Expression() : null;

            Consume(SEMICOLON, "Expect ';' after variable declaration.");

            return new Stmt.Var(name, initializer);
        }

        private Stmt Statement()
        {
            if (Match(FOR)) return ForStatement();
            if (Match(IF)) return IfStatement();
            if (Match(PRINT)) return PrintStatement();
            if (Match(WHILE)) return WhileStatement();
            if (Match(LEFT_BRACE)) return new Stmt.Block(Block());
            return ExpressionStatement();
        }

        private Stmt ForStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'for'.");

            Stmt? initializer;
            if (Match(SEMICOLON))
                initializer = null;
            else if (Match(VAR))
                initializer = VarDeclaration();
            else initializer = ExpressionStatement();

            Expr? condition = null;
            if (!Check(SEMICOLON))
                condition = Expression();
            Consume(SEMICOLON, "Expect ';' after loop condition.");

            Expr? increment = null;
            if (!Check(RIGHT_PAREN))
                increment = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after for clauses.");

            Stmt body = Statement();

            if (increment is not null)
            {
                body = new Stmt.Block(
                    new List<Stmt?>
                    {
                        body,
                        new Stmt.Expression(increment)
                    });
            }

            if (condition is null) condition = new Expr.Literal(true);
            body = new Stmt.While(condition, body);

            if (initializer is not null)
                body = new Stmt.Block(new List<Stmt?> { initializer, body });

            return body;
        }

        private Stmt IfStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' afer 'if'.");
            Expr condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after if condition.");

            Stmt thenBranch = Statement();
            Stmt? elseBranch = null;
            if (Match(ELSE))
                elseBranch = Statement();

            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        private Stmt WhileStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'while'.");
            Expr condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after while condition.");

            Stmt body = Statement();

            return new Stmt.While(condition, body);
        }

        private Stmt PrintStatement()
        {
            Expr value = Expression();
            Consume(SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(value);
        }

        private List<Stmt?> Block()
        {
            List<Stmt?> statements = new List<Stmt?>();

            while (!Check(RIGHT_BRACE) && !IsAtEnd())
                statements.Add(Declaration());

            Consume(RIGHT_BRACE, "Expect '}' after block.");

            return statements;
        }

        private Stmt ExpressionStatement()
        {
            Expr value = Expression();
            Consume(SEMICOLON, "Expect ';' after value.");
            return new Stmt.Expression(value);
        }

        private Expr Expression() => Assignment();

        private Expr Assignment()
        {
            Expr expr = Or();

            if (Match(EQUAL))
            {
                Token equals = Previous();
                Expr value = Assignment();

                if (expr is Expr.Variable variable)
                {
                    Token name = variable.Name;
                    return new Expr.Assign(name, value);
                }

                _errorHandler.Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Expr Or()
        {
            Expr expr = And();

            while (Match(OR))
            {
                Token op = Previous();
                Expr right = And();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        private Expr And()
        {
            Expr expr = Ternary();

            while (Match(AND))
            {
                Token op = Previous();
                Expr right = Ternary();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

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

            return Call();
        }

        private Expr Call()
        {
            Expr expr = Primary();

            while(true)
            {
                if(Match(LEFT_PAREN))
                {
                    expr = FinishCall(expr);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        private Expr FinishCall(Expr callee)
        {
            List<Expr> arguments = new();
            
            if (!Check(RIGHT_PAREN))
            {
                do
                {
                    if (arguments.Count >= 255)
                    {
                        Error(Peek(), "Can't have more than 255 arguments");
                    }

                    arguments.Add(Expression());
                } while (Match(COMMA));
            }

            Token paren = Consume(RIGHT_PAREN, "Expect ')' after arguments.");

            return new Expr.Call(callee, paren, arguments);

        }

        private Expr Primary()
        {
            if (Match(FALSE)) return new Expr.Literal(false);
            if (Match(TRUE)) return new Expr.Literal(true);
            if (Match(NIL)) return new Expr.Literal(null);

            if (Match(NUMBER, STRING)) return new Expr.Literal(Previous().Literal);

            if (Match(IDENTIFIER)) return new Expr.Variable(Previous());

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
            foreach(TokenType type in types)
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
                if (Previous().Type is SEMICOLON) return;

                if (Peek().Type is CLASS or FOR or FUN or IF or PRINT
                                or RETURN or VAR or WHILE)
                    return;

                Advance();
            }
        }
    }
}
