using System;

namespace kwd.RdfSeed.Errors
{
	/// <summary>
	/// Raised where node value string is null.
	/// </summary>
	public class ValueStringIsNull : Exception
	{
		/// <summary>Create new <see cref="ValueStringIsNull"/>.</summary>
		public ValueStringIsNull()
			:base("Node has no value string"){}
	}
}