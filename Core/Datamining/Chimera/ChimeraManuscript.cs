using System;
using System.Collections.Generic;
using Core.Serialization;

namespace Core.Datamining.Chimera
{
	public abstract class ChimeraManuscript<T>
	{
	    private readonly ISerializer _serializer;

	    protected ChimeraManuscript(ISerializer serializer)
	    {
	        _serializer = serializer;
	    }

	    public abstract IChimeraManuscriptArgument GetFileArgument();
		public abstract IEnumerable<IChimeraManuscriptArgument> GetManuscriptArguments();
		public virtual T DeserializeEntity(string serializedEntity)
		{
			T entity;
			
			try
			{
				entity = _serializer.Deserialize<T>(serializedEntity);
			}
			catch (Exception e)
			{
				throw new ChimeraException(
					String.Format("Failed to deserialize string: {0} to {1}", serializedEntity, typeof(T)));
			}

			return entity;
		}
	}
}