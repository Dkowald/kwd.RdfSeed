using System;

namespace kwd.RdfSeed.Serialize.Errors
{
	/// <summary>
	/// Raised to report a blank node scoped to unexpected graph.
	/// </summary>
	public class BlankNodeScopedToInvalidGraph : Exception
	{
		/// <summary>
		/// Create new <see cref="BlankNodeScopedToInvalidGraph"/>
		/// </summary>
		public BlankNodeScopedToInvalidGraph()
			:base("Blank node doesn't belong to this graph."){}
	}
}