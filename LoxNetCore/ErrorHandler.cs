namespace LoxNetCore
{
	internal class ErrorHandler
	{
		internal bool HadError { get; set; } = false;

		internal void Error(int line, string message) => Report(line, "", message);

		internal void Report(int line, string where, string message)
		{
			Console.Error.WriteLine($"[line {line}] Error {where}: {message}");

			HadError = true;
		}
	}
}
