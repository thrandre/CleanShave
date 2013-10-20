using System.IO;

namespace Core.Datamining.Chimera
{
	public class ChimeraManuscriptFileArgument : IChimeraManuscriptArgument
	{
		private readonly FileInfo _manuscriptPath;

		public ChimeraManuscriptFileArgument(FileInfo manuscriptPath)
		{
			_manuscriptPath = manuscriptPath;
		}

		public string ToFormattedString()
		{
			return _manuscriptPath.FullName;
		}
	}
}