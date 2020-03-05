using System;

namespace kwd.RdfSeed.Errors
{
	/// <summary>
	/// Raised if attempt to create a object-based node with a Uri
	/// </summary>
	public class UriObjectNodeNotSupported : Exception
	{
		/// <summary>
		/// Create a <see cref="UriObjectNodeNotSupported"/>
		/// </summary>
		public UriObjectNodeNotSupported()
		:base("Create a URI node explicitly, not via the generic node factory")
		{}
	}
}
