using System.Collections.Generic;
using System.IO;
using Core.Processes;

namespace CleanShave.Core.Processes
{
	public interface IProcessRunner
	{
		FileInfo ProcessPath { get; set; }
		ProcessResult Run(IEnumerable<IProcessArgument> arguments);
	}
}