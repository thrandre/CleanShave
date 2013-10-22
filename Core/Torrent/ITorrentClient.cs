using System.Collections.Generic;
using Core.Torrent;

namespace CleanShave.Core.Torrent
{
	public interface ITorrentClient
	{
		TorrentData GetTorrentByHash(string hash);
		void RemoveTorrent(string hash);
		IList<TorrentData> GetAllTorrents();
	}
}