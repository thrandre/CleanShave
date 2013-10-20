using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.Expressions;

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
