using System;

namespace Core.Processes
{
	public class ProcessRunnerException : Exception
	{
		public ProcessRunnerException(string message) : base(message) { }
	}
}