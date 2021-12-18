namespace LoxNetCore
{
	public class ErrorHandler
	{
		public bool HadError { get; set; } = false;

		public void Error(int line, string message) => Report(line, "", message);

		public void Report(int line, string where, string message)
		{
			Console.Error.WriteLine($"[line {line}] Error {where}: {message}");

			HadError = true;
		}
	}
}
