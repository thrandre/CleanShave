using System.Collections.Generic;

namespace Core.Torrent
{
    public interface ITorrentClient
    {
        TorrentData GetTorrentByHash(string hash);
        void RemoveTorrent(string hash);
	    IList<TorrentData> GetAllTorrents();
    }
}