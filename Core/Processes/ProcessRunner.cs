using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CleanShave.Core.Processes
{
	public class ProcessRunner : IProcessRunner
	{
		public FileInfo ProcessPath { get; set; }

		private string BuildArgumentString(IEnumerable<IProcessArgument> arguments)
		{
			return arguments == null ? String.Empty : String.Join(" ", arguments).Trim();
		}

		public ProcessResult Run(IEnumerable<IProcessArgument> arguments)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = ProcessPath.FullName,
					Arguments = BuildArgumentString(arguments),
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				}
			};

			string result;
			string errors;

			try
			{
				process.Start();

				result = process.StandardOutput.ReadToEnd();
				errors = process.StandardError.ReadToEnd();

				process.WaitForExit();
			}
			catch (Exception e)
			{
				throw new ProcessRunnerException(
					String.Format("The process exited unexpectedely with message: {0}", e.Message));
			}
			finally
			{
				process.Dispose();
			}

			return new ProcessResult
			{
				Output = result,
				Errors = errors
			};
		}
	}
}