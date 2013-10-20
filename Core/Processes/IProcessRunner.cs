using System.Collections.Generic;
using System.IO;

namespace Core.Processes
{
	public interface IProcessRunner
	{
		FileInfo ProcessPath { get; set; }
		ProcessResult Run(IEnumerable<IProcessArgument> arguments);
	}
}