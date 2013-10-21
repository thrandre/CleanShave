using System.Collections.Generic;

namespace CleanShave.Puppet
{
	public class PuppetConfig
	{
		public UTorrentConfig UTorrent { get; set; }
		public SickbeardConfig Sickbeard { get; set; }

		public int PollInterval { get; set; }
		public string[] ArchiveExtensions { get; set; }

		public Dictionary<string, LabelConfig> Labels { get; set; }
	}
}
