using System;
using System.Collections.Generic;
using System.Linq;

using kwd.RdfSeed.Builder;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Query;

namespace kwd.RdfSeed.Core
{
    /// <summary>
    /// Rdf data focused on a particular graph;
    /// </summary>
    /// <remarks>
    /// A graph contains its own quads, and
    /// optionally the set of quads from other graphs.
    ///
    /// The main graph is used for writes;
    /// while main and all other graphs are used for queries.
    ///
    /// Data from other graphs is snapshot of current state.
    /// </remarks>
    public class Graph : IGraphNodeFactory
    {
        private readonly IEnumerable<Quad> _qryGraphQuads;
        private readonly IReadOnlyCollection<Quad> _includedQuads;
        private readonly IRdfData _rdf;

        /// <summary>Create graph with Id and data from other graphs</summary>
        public Graph(IRdfData rdf, 
	        Node<UriOrBlank> id,
            IEnumerable<Quad> qryGraphQuads,
	        IEnumerable<Node<UriOrBlank>> other)
        {
	        _rdf = rdf;
	        
	        _qryGraphQuads = qryGraphQuads;

	        Id = id;
	        Other = other.ToList();

	        _includedQuads = rdf.Query
		        .Where(x => Other.Contains(x.Graph))
		        .ToList();
        }

        /// <summary>The graph id / subject</summary>
        public Node<UriOrBlank> Id { get; }

		/// <summary>The other graph(s) making up this</summary>
		public IReadOnlyCollection<Node<UriOrBlank>> Other { get; }

		/// <summary>Assert / add a new triple for this graph</summary>
		public Graph Assert(Node<UriOrBlank> subject, UriNode predicate, Node @object)
        {
	        _rdf.Assert(Id, subject, predicate, @object);
	        return this;
        }
        
        /// <summary>Remove all Quads in this graph.</summary>
        /// <returns>Number of triples removed</returns>
        public int Clear()
	        => _rdf.Retract(_rdf.Query.From(Id).ToArray());

        /// <summary>
        /// Retract (remove) triples from this graph.
        /// Only quads for this graph (<see cref="Id"/>) are considered.
        /// </summary>
        /// <returns>Number of <see cref="Quad"/> actually removed</returns>
        public int Retract(params Quad[] quads)
         => _rdf.Retract(quads.Where(x => x.Graph == Id).ToArray());
        
        /// <summary>
        /// Create a new <see cref="GraphBuilder"/> to
        /// update data to the graph with a more fluent interface.
        /// </summary>
        public GraphBuilder Update => _rdf.Update.From(Id);

        /// <summary>
        /// List all current quads in this graph.
        /// </summary>
        public IReadOnlyCollection<Quad> Query =>
            _qryGraphQuads.Union(_includedQuads).ToArray();

        /// <summary>Convert graph to a self-only graph</summary>
        public Graph SelfOnly() => _rdf.GetSelfGraph(Id);

		#region IGraphNodeFactory

		/// <inheritdoc />
		public Node<T> New<T>(T value) where T : notnull
			=> _rdf.New(value);

		/// <inheritdoc />
		public Node New(ReadOnlySpan<char> valueString, ReadOnlySpan<char> typeString)
			=> _rdf.New(valueString, typeString);

		/// <inheritdoc />
		public UriNode Uri(ReadOnlySpan<char> uriValue)
			=> _rdf.Uri(uriValue);

		/// <inheritdoc />
		public BlankNode Blank()
			=> _rdf.Blank(Id);

		/// <inheritdoc />
		public BlankNode Blank(ReadOnlySpan<char> label)
			=> _rdf.Blank(Id, label);

		/// <inheritdoc />
		public IReadOnlyCollection<NodeMap> Mappings => _rdf.Mappings;

		#endregion
    }
}