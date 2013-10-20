using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Datamining.Chimera
{
	public class ChimeraManuscriptArgumentArray : IChimeraManuscriptArgument
	{
		private readonly string _name;
		private readonly IEnumerable<object> _value;

		public ChimeraManuscriptArgumentArray(string name, IEnumerable<object> value)
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