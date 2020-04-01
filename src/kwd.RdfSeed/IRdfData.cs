using System.Collections.Generic;
using kwd.RdfSeed.Builder;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;

namespace kwd.RdfSeed
{
	/// <summary>A set of <see cref="Quad"/> objects</summary>
	public interface IRdfData : INodeFactory
	{
		/// <summary>Get a copy of all the current quads.</summary>
		IReadOnlyList<Quad> Query { get; }

		/// <summary>
		/// Create a new <see cref="RdfBuilder"/> to
		/// update rdf data with a more fluent interface.
		/// </summary>
		public RdfBuilder Update { get; }

		/// <summary>
		/// Id for System graph used to track self generated quads.
		/// </summary>
		UriNode System { get; }

		/// <summary>Id for Default graph</summary>
		UriNode Default { get; }

		/// <summary>
		/// The current set of all graph id's.
		/// This ALWAYS starts with the <see cref="System"/>, then
		/// <see cref="Default"/> graphs.
		/// </summary>
		Node<UriOrBlank>[] GraphIds { get; }

		/// <summary>
		/// A set of quads ordered by graph.
		/// Includes data from the specified additional graph(s)
		/// </summary>
		Graph GetGraph(Node<UriOrBlank> graphId, params Node<UriOrBlank>[] other);

		/// <summary>
		/// Get quads for the given graph id(s).
		/// Quads are returned in same order as graph Id's
		/// </summary>
		Quad[] GraphData(params Node<UriOrBlank>[] graphIds);

		/// <summary>Assert / add a quad.</summary>
		IRdfData Assert(Node<UriOrBlank> graph, 
			Node<UriOrBlank> sub, 
			UriNode predicate, Node val,
			out Quad quad);

		/// <summary>Retract / remove a set of quads</summary>
		/// <returns>The number of quads actually removed</returns>
		int Retract(params Quad[] quads);

		/// <summary>
		/// Retract <paramref name="oldQuads"/> and
		/// Assert <paramref name="newQuads"/> in one operation.
		/// </summary>
		/// <returns>Number of quads actually removed.</returns>
		int Replace(IEnumerable<Quad> newQuads, IEnumerable<Quad> oldQuads);
	}
}