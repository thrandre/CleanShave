using System;
using System.Collections.Generic;
using System.Linq;
using UTorrent.Api;

namespace Core.Torrent.Clients.MicroTorrent
{
	public class MicroTorrentAdapter : ITorrentClient
	{
		private readonly TorrentClientConnectionParameters _parameters;
		private readonly UTorrentClient _client;

		public MicroTorrentAdapter(TorrentClientConnectionParameters parameters)
		{
			_parameters = parameters;
			_client = new UTorrentClient(_parameters.Ip, _parameters.Port, _parameters.Username, _parameters.Password);
		}

		private Response Request(Func<UTorrentClient, Response> query)
		{
			try
			{
				return query(_client);
			}
			catch (ServerUnavailableException e)
			{
				throw new TorrentClientConnectionException(
						String.Format("Unable to connect to uTorrent on {0}:{1} with username {2} ({3})",
								_parameters.Ip, _parameters.Port, _parameters.Username, e.Message));
			}
		}

		public TorrentData GetTorrentByHash(string hash)
		{
			var response = Request(c => c.GetTorrent(hash));
			var torrent = response.Result.Torrents.FirstOrDefault(t => t.Hash == hash);

			if (torrent == null)
			{
				throw new TorrentNotFoundException(
					String.Format("Unable to get torrent with hash {0}", hash));
			}

			var files = response.Result.Files[hash].Select(
					f => new TorrentFile { RelativePath = f.Name }).ToList();

			return new TorrentData
			{
				Hash = torrent.Hash,
				Name = torrent.Name,
				Label = torrent.Label,
				Done = (torrent.Progress == 1000),
				Ratio = torrent.Ratio,
				DateAdded = torrent.AddedDate,
				Files = files,
				DownloadPath = torrent.Path
			};
		}

		public IList<TorrentData> GetAllTorrents()
		{
			return Request(c => c.GetList()).Result.Torrents
				.Select(t => GetTorrentByHash(t.Hash)).ToList();
		}

		public void RemoveTorrent(string hash)
		{
			Request(c => c.DeleteTorrent(hash));
		}
	}
}
