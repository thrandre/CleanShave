using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Timers;
using System.Web;
using System.Web.Script.Serialization;
using CleanShave.Core.Torrent;
using CleanShave.Core.Torrent.Clients.MicroTorrent;
using SevenZip;
using Timer = System.Timers.Timer;

namespace CleanShave.Puppet
{
	internal class Program
	{
		private static PuppetConfig _config;

		private static ITorrentClient _client;

		private static string _logFile;
		private static Timer _pollingTimer;
		private static bool _currentlyProcessing;

		private static List<string> _processedList;
		private static string _processedListDataFile;

		private static void Main()
		{
			AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

			Configure(AppDomain.CurrentDomain.BaseDirectory);
			SetupPollingLoop();

			Console.ReadKey();
		}

		private static void Configure(string baseDir)
		{
			_logFile = Path.Combine(baseDir, "log.txt");
			_processedListDataFile = Path.Combine(baseDir, "processed.db");

			var configPath = String.Format(@"{0}\{1}", baseDir, "conf.json");
			var serializer = new JavaScriptSerializer();

			try
			{
				_config = serializer.Deserialize<PuppetConfig>(File.ReadAllText(configPath));
			}
			catch (Exception e)
			{
				Log(String.Format("Failed to read configuration file ({0})", e.Message));
			}

			var torrentConfig = _config.UTorrent;

			_client = new MicroTorrentAdapter(
				new TorrentClientConnectionParameters
				{
					Ip = torrentConfig.Ip,
					Port = torrentConfig.Port,
					Username = torrentConfig.Username,
					Password = torrentConfig.Password
				});
		}

		private static void SetupPollingLoop()
		{
			_pollingTimer = new Timer(10000);
			_pollingTimer.Elapsed += ProcessTorrents;
			_pollingTimer.Start();
		}

		private static void ProcessTorrents(object sender, ElapsedEventArgs eventArgs)
		{
			if (_currentlyProcessing)
			{
				return;
			}

			_currentlyProcessing = true;
			ReadProcessedList();

			IList<TorrentData> torrents;

			try
			{
				torrents = _client.GetAllTorrents();
			}
			catch (Exception e)
			{
				Log(e.Message);
				return;
			}

			foreach (var torrent in torrents)
			{
				try
				{
					ProcessTorrent(torrent);
				}
				catch (Exception e)
				{
					Log(e.Message);
				}
			}

			WriteProcessedList();
			_currentlyProcessing = false;
		}

		private static void ProcessTorrent(TorrentData torrent)
		{
			if (!_config.Labels.ContainsKey(torrent.Label))
			{
				return;
			}

			var labelConfig = _config.Labels[torrent.Label];

			if (torrent.Done && !_processedList.Contains(torrent.Hash))
			{
				var targetDirectory = labelConfig.TargetDirectory + @"\" + torrent.Name;
				var sourceDirectory = torrent.DownloadPath;
				var files = ExtractAllFiles(
					Wrap(torrent.Files).ToList(), labelConfig, sourceDirectory);

				MarkWantedFiles(files, labelConfig);
				ProcessFiles(files, sourceDirectory, targetDirectory);
				NotifySickBeard(targetDirectory);

				_processedList.Add(torrent.Hash);

				return;
			}

			if (torrent.Done && FullfillsSeedCriteria(torrent, labelConfig))
			{
				RemoveTorrent(torrent);
			}
		}

		private static void ReadProcessedList()
		{
			var processed = new List<string>();

			if (File.Exists(_processedListDataFile))
			{
				processed = File.ReadAllLines(_processedListDataFile).ToList();
			}

			_processedList = processed;
		}

		private static void WriteProcessedList()
		{
			File.WriteAllLines(_processedListDataFile, _processedList);
		}

		private static List<WrappedFile> ExtractAllFiles(List<WrappedFile> files, LabelConfig config, string sourceDirectory)
		{
			var archives = FindArchives(files, config);
			var extractedFiles = ExtractArchives(archives, sourceDirectory);

			return files.Union(extractedFiles).ToList();
		}

		private static void NotifySickBeard(string targetDirectory)
		{
			var wc = new WebClient();

			try
			{
				wc.DownloadString(
					String.Format("http://{0}:{1}/home/postprocess/processEpisode?quiet=1&dir={2}",
						_config.Sickbeard.Ip, _config.Sickbeard.Port, HttpUtility.UrlEncode(targetDirectory)));
			}
			catch (Exception)
			{
				Log("Unable to notify sickbeard.");
			}
		}

		private static void ProcessFiles(IEnumerable<WrappedFile> files, string sourceDirectory, string targetDirectory)
		{
			foreach (var file in files)
			{
				if (file.Wanted)
				{
					CopyFile(file, sourceDirectory, targetDirectory);
				}

				if (file.Extracted)
				{
					DeleteFile(file, sourceDirectory);
				}
			}
		}

		private static void RemoveTorrent(TorrentData torrent)
		{
			_client.RemoveTorrent(torrent.Hash);
		}

		private static bool FullfillsSeedCriteria(TorrentData torrent, LabelConfig config)
		{
			var seedTime = new TimeSpan(DateTime.Now.Ticks - torrent.DateAdded.Ticks).Hours;

			return torrent.Ratio >= config.TargetRatio || seedTime >= config.MinimumSeedHours;
		}

		private static void EnsurePathIsWriteable(string targetFile)
		{
			var targetDirectory = Path.GetDirectoryName(targetFile);

			if (!String.IsNullOrEmpty(targetDirectory) && !Directory.Exists(targetDirectory))
			{
				Directory.CreateDirectory(targetDirectory);
			}
		}

		private static void CopyFile(WrappedFile file, string sourceDirectory, string targetDirectory)
		{
			var sourceFile = Path.Combine(sourceDirectory, file.TorrentFile.RelativePath);
			var targetFile = Path.Combine(targetDirectory, file.TorrentFile.RelativePath);

			EnsurePathIsWriteable(targetFile);

			File.Copy(sourceFile, targetFile, true);
		}

		private static void DeleteFile(WrappedFile file, string sourceDirectory)
		{
			var sourceFile = Path.Combine(sourceDirectory, file.TorrentFile.RelativePath);
			File.Delete(sourceFile);
		}

		private static void MarkWantedFiles(IEnumerable<WrappedFile> files, LabelConfig config)
		{
			FilterDirectories(files, config.BlacklistedDirectories)
				.Where(f =>
						config.KeepExtensions.Any(e =>
							f.TorrentFile.RelativePath.EndsWith(e)))
								.ToList()
								.ForEach(f =>
									f.Wanted = true);
		}

		private static IEnumerable<WrappedFile> Wrap(IEnumerable<TorrentFile> files)
		{
			return files.Select(f =>
				new WrappedFile
				{
					TorrentFile = f,
					Extracted = false,
					Wanted = false
				});
		}

		private static IEnumerable<WrappedFile> ExtractArchives(IEnumerable<WrappedFile> archives, string sourceDirectory)
		{
			var newFiles = new List<WrappedFile>();

			foreach (var path in archives.Select(a => Path.Combine(sourceDirectory, a.TorrentFile.RelativePath)))
			{
				var extractor = new SevenZipExtractor(path);

				extractor.ExtractArchive(sourceDirectory);

				newFiles.AddRange(
						extractor.ArchiveFileNames
							.Select(
								file =>
									new WrappedFile
									{
										TorrentFile = new TorrentFile
										{
											RelativePath = file
										},
										Extracted = true
									}));
			}

			return newFiles;
		}

		private static IEnumerable<WrappedFile> FindArchives(IEnumerable<WrappedFile> files, LabelConfig labelConfig)
		{
			return FilterDirectories(files, labelConfig.BlacklistedDirectories)
				.Where(f =>
					_config.ArchiveExtensions.Any(s =>
							f.TorrentFile.RelativePath.EndsWith(s, StringComparison.InvariantCultureIgnoreCase)));
		}

		private static IEnumerable<WrappedFile> FilterDirectories(IEnumerable<WrappedFile> files, IEnumerable<string> blacklistedDirectories)
		{
			return files
				.Where(f =>
					!blacklistedDirectories.Any(d =>
						f.TorrentFile.RelativePath
							.StartsWith(
								String.Format(@"{0}\", d),
								StringComparison.InvariantCultureIgnoreCase)));
		}

		static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
		{
			Log(e.ExceptionObject.ToString());
		}

		private static void Log(string line)
		{
			Console.WriteLine(line);
			File.AppendAllText(_logFile, line + "\n");
		}
	}
}