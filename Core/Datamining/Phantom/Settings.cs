using System.IO;

namespace CleanShave.Core.Datamining.Phantom
{
	public static class Settings
	{
		public static FileInfo PhantomPath { get; set; }
		public static FileInfo CasperPath { get; set; }
		public static DirectoryInfo WorkingDirectory { get; set; }
	}
}
