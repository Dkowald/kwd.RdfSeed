using System;
using kwd.RdfSeed.Core.Nodes.Builtin;

namespace kwd.RdfSeed.Core
{
	/// <summary>
	/// Create nodes for a particular <see cref="Graph"/>.
	/// </summary>
	public interface IGraphNodeFactory : IBasicNodeFactory
	{
		/// <summary>
		/// Create new auto blank node for this graph.
		/// </summary>
		BlankNode Blank();

		/// <summary>
		/// Create new labeled blank node for this graph.
		/// </summary>
		BlankNode Blank(ReadOnlySpan<char> label);
	}
}