using System;

namespace Core.Torrent
{
    public class TorrentClientConnectionException : Exception
    {
        public TorrentClientConnectionException(string message) : base(message) {}
    }
}