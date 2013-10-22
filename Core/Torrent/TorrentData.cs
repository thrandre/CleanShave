using System;
using System.Collections.Generic;
using Core.Torrent;

namespace CleanShave.Core.Torrent
{
	public class TorrentData
	{
		public string Hash { get; set; }
		public string Name { get; set; }
		public string Label { get; set; }
		public bool Done { get; set; }
		public int Ratio { get; set; }
		public DateTime DateAdded { get; set; }

		public string DownloadPath { get; set; }
		public IList<TorrentFile> Files { get; set; }
	}
}