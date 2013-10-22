using System;

namespace CleanShave.Core.Torrent
{
	public class TorrentNotFoundException : Exception
	{
		public TorrentNotFoundException(string message) : base(message) { }
	}
}