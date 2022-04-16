using static LoxNetCore.TokenType;

namespace LoxNetCore
{
    public class Interpreter
    {
        private Env _environment = new Env();
        private ErrorHandler _errorHandler = new ErrorHandler();

        public void Interpret(List<Stmt?> statements)
        {
            try
            {
                foreach (Stmt? statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeException error)
            {
                _errorHandler.RuntimeError(error);
            }
        }

        private void Execute(Stmt? statement)
        {
            switch (statement)
            {
                case Stmt.Print printStmt:
                    HandlePrintStatement(printStmt);
                    break;
                case Stmt.Var varStmt:
                    HandleVarStatement(varStmt);
                    break;
                case Stmt.Expression expression:
                    HandleExpression(expression);
                    break;
                case Stmt.Block block:
                    HandleBlock(block.Statements, new Env(_environment));
                    break;
            }
        }

        private void HandlePrintStatement(Stmt.Print stmt) =>
            Console.WriteLine(Stringify(Evaluate(stmt.Expr)));

        private void HandleVarStatement(Stmt.Var stmt)
        {
            object? value = (stmt.Initializer is null) ? null : Evaluate(stmt.Initializer);

            _environment.Define(stmt.Name.Lexeme, value);
        }

        public void HandleExpression(Stmt.Expression expression) => Evaluate(expression.Expr);

        public void HandleBlock(List<Stmt?> statements, Env evironment)
        {
            Env previousEnvironment = _environment;

            try
            {
                _environment = evironment;

                foreach (var statement in statements)
                    Execute(statement);
            }

            finally 
            { 
                _environment = previousEnvironment; 
            }
        }

        private object? Evaluate(Expr expression)
        {
            return expression switch
            {
                Expr.Literal literal => literal.Value,
                Expr.Variable variable => RetrieveVariable(variable.Name),
                Expr.Grouping grouping => Evaluate(grouping.Expr),
                Expr.Unary unary => HandleUnary(unary),
                Expr.Binary binary => HandleBinary(binary),
                Expr.Ternary ternary => HandleTernary(ternary),
                Expr.Assign assignment => HandleAssignment(assignment),
                _ => throw new NotImplementedException()
            };
        }

        private object? HandleAssignment(Expr.Assign assignment)
        {
            Object? value = Evaluate(assignment.Value);
            _environment.Assign(assignment.Name, value);
            return value;
        }

        private object? RetrieveVariable(Token name) => _environment.Get(name);

        private object? HandleUnary(Expr.Unary unary)
        {
            object? right = Evaluate(unary.Right);

            return unary.Op.Type switch
            {
                MINUS when right is double r => -r,

                BANG => !IsTruthy(right),

                _ => throw new RuntimeException(unary.Op, $"Unary operator {unary.Op.Lexeme} can't be applied to operand {right} with type of {right?.GetType().Name}"),
            };
        }

        private object? HandleTernary(Expr.Ternary ternary)
        {
            object? boolExpr = Evaluate(ternary.BoolExpr);

            return IsTruthy(boolExpr) ? Evaluate(ternary.TrueExpr) : Evaluate(ternary.FalseExpr);
        }

        private object? HandleBinary(Expr.Binary binary)
        {
            object? left = Evaluate(binary.Left);
            object? right = Evaluate(binary.Right);

            return binary.Op.Type switch
            {
                MINUS when left is double l && right is double r => l - r,

                PLUS when left is double l && right is double r => l + r,
                PLUS when left is string l && right is string r => l + r,
                PLUS when left is double l && right is string r => l + r,
                PLUS when left is string l && right is double r => l + r,

                SLASH when left is double l && right is double r && r == 0 => throw new RuntimeException(binary.Op, "Can't divide by 0"),
                SLASH when left is double l && right is double r => l / r,
                
                STAR when left is double l && right is double r => l * r,
                STAR when left is string l && right is double r => string.Concat(Enumerable.Repeat(l, (int) r)),

                GREATER when left is double l && right is double r => l > r,
                GREATER_EQUAL when left is double l && right is double r => l >= r,

                GREATER when left is string l && right is string r => l.CompareTo(r) > 0,
                GREATER_EQUAL when left is string l && right is string r => l.CompareTo(r) >= 0,

                LESS when left is double l && right is double r => l < r,
                LESS_EQUAL when left is double l && right is double r => l <= r,

                LESS when left is string l && right is string r => l.CompareTo(r) < 0,
                LESS_EQUAL when left is string l && right is string r => l.CompareTo(r) <= 0,

                EQUAL_EQUAL => IsEqual(left, right),
                BANG_EQUAL => !IsEqual(left, right),

                _ => throw new RuntimeException(binary.Op, $"Binary operator {binary.Op.Lexeme} can't be applied to left operand {Stringify(left)} ({left?.GetType().Name}) and right operand {Stringify(right)} ({right?.GetType().Name})"),
            };
        }

        private bool IsTruthy(object? obj)
        {
            if (obj is null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }

        private bool IsEqual(object? a, object? b)
        {
            if (a is null && b is null) return true;
            if (a is null) return false;
            
            return a.Equals(b);
        }

        private string? Stringify(object? obj)
        {
            switch (obj)
            {
                case null:
                    return "nil";
                case bool b:
                    return b ? "true" : "false";
                case double d:
                    string text = d.ToString();

                    if (text.EndsWith(".0"))
                        text = text[..-2];

                    return text;
                default:
                    return obj.ToString();
            }
        }
    }
}
