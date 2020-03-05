using System;

namespace kwd.RdfSeed.Errors
{
	/// <summary>
	/// Raised when attempting to get node with particular value.
	/// </summary>
	public class ValueNotFoundError : Exception
	{
		/// <summary>Create new <see cref="ValueNotFoundError"/>.</summary>
		public ValueNotFoundError()
			: base("expected value not found"){}
	}
}