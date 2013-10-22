using System;
using System.Collections.Generic;
using CleanShave.Core.Serialization;

namespace CleanShave.Core.Datamining.Phantom
{
	public abstract class PhantomManuscript<T>
	{
		private readonly ISerializer _serializer;

		protected PhantomManuscript(ISerializer serializer)
		{
			_serializer = serializer;
		}

		public abstract IPhantomManuscriptArgument GetFileArgument();
		public abstract IEnumerable<IPhantomManuscriptArgument> GetManuscriptArguments();
		public virtual T DeserializeEntity(string serializedEntity)
		{
			T entity;

			try
			{
				entity = _serializer.Deserialize<T>(serializedEntity);
			}
			catch (Exception e)
			{
				throw new PhantomException(
					String.Format("Failed to deserialize string: {0} to {1}", serializedEntity, typeof(T)));
			}

			return entity;
		}
	}
}