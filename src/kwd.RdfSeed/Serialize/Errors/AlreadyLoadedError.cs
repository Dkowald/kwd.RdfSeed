using System;

namespace kwd.RdfSeed.Serialize.Errors
{
	/// <summary>
	/// Raised if attempt to load data from same source to different graph.
	/// </summary>
	public class AlreadyLoadedError : Exception
	{
		/// <summary>Create a new <see cref="AlreadyLoadedError"/>.</summary>
		public AlreadyLoadedError()
			:base("Graph is already loaded with different source"){}
	}
}