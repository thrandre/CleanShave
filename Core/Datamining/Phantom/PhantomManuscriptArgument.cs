namespace CleanShave.Core.Datamining.Phantom
{
	public class PhantomManuscriptArgument : IPhantomManuscriptArgument
	{
		private readonly string _name;
		private readonly object _value;

		public PhantomManuscriptArgument(string name, object value)
		{
			_name = name;
			_value = value;
		}

		public string ToFormattedString()
		{
			return string.Format("--{0}={1}", _name, _value);
		}
	}
}