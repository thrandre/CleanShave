namespace Core.Datamining.Chimera
{
	public class ChimeraManuscriptArgument : IChimeraManuscriptArgument
	{
		private readonly string _name;
		private readonly object _value;

		public ChimeraManuscriptArgument(string name, object value)
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