using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanShave.Core.Datamining.Phantom
{
	public class PhantomManuscriptArgumentArray : IPhantomManuscriptArgument
	{
		private readonly string _name;
		private readonly IEnumerable<object> _value;

		public PhantomManuscriptArgumentArray(string name, IEnumerable<object> value)
		{
			_name = name;
			_value = value;
		}

		public string ToFormattedString()
		{
			return String.Format("--{0}={1}", _name, String.Join(",", _value.Select(v => v.ToString())));
		}
	}
}