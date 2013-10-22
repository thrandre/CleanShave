using System.Web.Script.Serialization;

namespace CleanShave.Core.Serialization
{
	public class DefaultJavascriptSerializer : ISerializer
	{
		private readonly JavaScriptSerializer _serializer;

		public DefaultJavascriptSerializer()
		{
			_serializer = new JavaScriptSerializer();
		}

		public T Deserialize<T>(string serializedData)
		{
			return _serializer.Deserialize<T>(serializedData);
		}
	}
}