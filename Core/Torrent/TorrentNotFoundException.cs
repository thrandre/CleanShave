using System;

namespace Core.Torrent
{
	public class TorrentNotFoundException : Exception
	{
		public TorrentNotFoundException(string message) : base(message) { }
	}
}