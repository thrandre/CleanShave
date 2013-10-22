namespace CleanShave.Core.Serialization
{
	public interface ISerializer
	{
		T Deserialize<T>(string serializedData);
	}
}