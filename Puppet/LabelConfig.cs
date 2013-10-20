namespace CleanShave.Puppet
{
    public class LabelConfig
    {
        public int TargetRatio { get; set; }
        public int MinimumSeedHours { get; set; }
        public string[] BlacklistedDirectories { get; set; }
        public string[] KeepExtensions { get; set; }
        public string TargetDirectory { get; set; }
    }
}