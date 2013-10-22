using System.IO;

namespace CleanShave.Core.Datamining.Phantom
{
	public class PhantomManuscriptFileArgument : IPhantomManuscriptArgument
	{
		private readonly FileInfo _manuscriptPath;

		public PhantomManuscriptFileArgument(FileInfo manuscriptPath)
		{
			_manuscriptPath = manuscriptPath;
		}

		public string ToFormattedString()
		{
			return _manuscriptPath.FullName;
		}
	}
}