using Core.Torrent;

namespace CleanShave.Puppet
{
	public class WrappedFile
	{
		public TorrentFile TorrentFile { get; set; }
		public bool Extracted { get; set; }
		public bool Wanted { get; set; }
	}
}