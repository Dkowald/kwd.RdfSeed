using System;
using System.Collections.Generic;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Errors;

namespace kwd.RdfSeed.Core
{
	/// <summary>
	/// Factory for creating nodes.
	/// </summary>
	public interface IBasicNodeFactory
	{
		/// <summary>
		/// Create (or reuse) node;
		/// uses <see cref="ObjectNodeMap{T}"/> if no
		/// registered builder.
		/// </summary>
		/// <exception cref="TypeMustNotBeANode"></exception>
		Node<T> New<T>(T value)where T:notnull;

		/// <summary>
		/// Create or reuse node.
		/// Uses <see cref="TypedLiteralMap"/> if no registered builder.
		/// </summary>
		Node New(ReadOnlySpan<char> valueString, ReadOnlySpan<char> typeString);

		/// <summary>
		/// Create <see cref="UriNode"/> node that Must be a uri.
		/// </summary>
		UriNode Uri(ReadOnlySpan<char> uriValue);

		/// <summary>
		/// The (current) set of current node mappings.
		/// </summary>
		IReadOnlyCollection<NodeMap> Mappings { get; }
	}
}