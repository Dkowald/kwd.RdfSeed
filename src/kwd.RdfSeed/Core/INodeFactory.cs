using System;

using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;

namespace kwd.RdfSeed.Core
{
	/// <summary>
	/// Create nodes, including blank nodes.
	/// </summary>
	public interface INodeFactory : IBasicNodeFactory
	{
		/// <summary>
		/// Get a new unique blank node
		/// </summary>
		BlankNode Blank(Node<UriOrBlank> scope);

		/// <summary>
		/// Get a blank node with label.
		/// </summary>
		BlankNode Blank(Node<UriOrBlank> scope, ReadOnlySpan<char> label);

		/// <summary>
		/// A Blank that is scoped to itself.
		/// </summary>
		BlankNode BlankSelf(ReadOnlySpan<char> label);

		/// <summary>
		/// A blank scoped to self, with auto label
		/// </summary>
		BlankNode BlankSelf();
	}
}