using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;

namespace kwd.RdfSeed.Builder
{
	/// <summary>
    /// A fluent interface to interact with a graph.
    /// </summary>
    public class GraphBuilder : IQuadBuilderContext
    {
	    private readonly RdfBuilder _owner;
	    private readonly Graph _data;
        
        /// <summary>
        /// Create new <see cref="GraphBuilder"/> for the given
        /// <see cref="Graph"/> <paramref name="g"/>.
        /// </summary>
        public GraphBuilder(RdfBuilder owner, Graph g)
        {
	        _owner = owner;
	        _data = g;
        }

        /// <summary>
        /// Keep reference to current graph in a variable.
        /// </summary>
        public GraphBuilder Let(out Node<UriOrBlank> id)
        {
            id = _data.Id;
            return this;
        }

        /// <summary>Select subject.</summary>
        public SubjectBuilder For(Node<UriOrBlank> subject)
	        => new SubjectBuilder(this, subject);
        
        /// <summary>
        /// A blank node scoped to this Graph
        /// </summary>
        public SubjectBuilder ForBlank(string? label, out BlankNode node)
        {
            node = label is null ? 
                _data.Blank() : 
                _data.Blank(label);
            return new SubjectBuilder(this, node);
        }
        
        /// <summary>
        /// Go back to the starting rdf data.
        /// </summary>
        public RdfBuilder Then() => _owner;

        /// <summary>
        /// Back to a different graph
        /// </summary>
        public GraphBuilder Then(Node<UriOrBlank> graph) => 
            _owner.From(graph);

        #region IQuadBuilderContext
        IRdfData IQuadBuilderContext.Rdf => ((IQuadBuilderContext)_owner).Rdf;
        
        Node<UriOrBlank> IQuadBuilderContext.GraphNode => _data.Id;
        Node<UriOrBlank>? IQuadBuilderContext.Subject => null;
        Node<UriOrBlank>? IQuadBuilderContext.Predicate => null;
        #endregion IQuadBuilder
    }
}