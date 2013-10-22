using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanShave.Core.Processes;

namespace CleanShave.Core.Datamining.Phantom
{
	public class PhantomRunner : IDisposable
	{
		public IProcessRunner ProcessRunner
		{
			get
			{
				return _processRunner ?? (_processRunner = new ProcessRunner());
			}

			set
			{
				_processRunner = value;
			}
		}

		private FileInfo _runScript;
		private IProcessRunner _processRunner;

		private string GenerateRunScript()
		{
			return new StringBuilder("@ECHO OFF").Append("\n")
				.AppendFormat(@"set PHANTOM_PATH={0}", Settings.PhantomPath).Append("\n")
				.AppendFormat(@"set CASPER_PATH={0}", Settings.CasperPath).Append("\n")
				.AppendFormat(@"set CASPER_BIN=%CASPER_PATH%\bin").Append("\n")
				.AppendFormat(@"set ARGV=%*").Append("\n")
				.AppendFormat(@"call ""%PHANTOM_PATH%\phantomjs"" ""%CASPER_BIN%\bootstrap.js"" --casper-path=""%CASPER_PATH%"" --cli %ARGV%").Append("\n")
				.ToString();
		}

		private FileInfo SaveRunScript()
		{
			var stamp = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);

			var path = Path.Combine(
				Settings.WorkingDirectory.FullName,
				string.Format("chimera-{0}.cmd", stamp.Substring(stamp.Count() - 5)));

			var script = GenerateRunScript();

			File.WriteAllText(path, script);

			return new FileInfo(path);
		}

		public Task<T> Run<T>(PhantomManuscript<T> manuscript)
		{
			if (_runScript == null)
			{
				_runScript = SaveRunScript();
				ProcessRunner.ProcessPath = _runScript;
			}

			return Task<T>.Factory.StartNew(() =>
			{
				var allArguments =
					new List<IProcessArgument> { manuscript.GetFileArgument() }
						.Union(manuscript.GetManuscriptArguments());

				T entity;

				try
				{
					var result = ProcessRunner.Run(allArguments);
					if (String.IsNullOrEmpty(result.Errors))
					{
						entity = manuscript.DeserializeEntity(result.Output);
					}
					else
					{
						throw new PhantomException(
							String.Format("Returned with error: {0}", result.Errors));
					}
				}
				catch (ProcessRunnerException pre)
				{
					throw new PhantomException(
						String.Format("Failed to start process: {0}", pre.Message));
				}

				return entity;
			});
		}

		~PhantomRunner()
		{
			Dispose();
		}

		public void Dispose()
		{
			File.Delete(_runScript.FullName);
		}
	}
}
