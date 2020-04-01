using System;
using System.Linq;

using kwd.Rdf.Std;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;

namespace kwd.RdfSeed.Builder
{
    /// <summary>
    /// Fluent interface where Graph has been selected.
    /// </summary>
    public class SubjectBuilder : IQuadBuilderContext
    {
        private readonly GraphBuilder _owner;
        private readonly IQuadBuilderContext _ctx;

        private readonly Node<UriOrBlank> _id;

        /// <summary>
        /// Create new <see cref="SubjectBuilder"/>.
        /// </summary>
        public SubjectBuilder(GraphBuilder forG, Node<UriOrBlank> id)
        {
            _owner = forG;
            _id = id;
            _ctx = forG;
        }

        /// <summary>Select predicate.</summary>
        public PredicateBuilder With(UriNode predicate) 
            => new PredicateBuilder(this, predicate);

        /// <summary>
        /// Back to the graph
        /// </summary>
        public GraphBuilder Then() => _owner;

        /// <summary>
        /// Back to a different graph
        /// </summary>
        public GraphBuilder Then(Node<UriOrBlank> graph)
            => _owner.Then(graph);

        /// <summary>
        /// Keep reference to current subject in a variable.
        /// </summary>
        public SubjectBuilder Let(out Node<UriOrBlank> id)
        {id = _id; return this; }

        /// <summary>Adds items as a RDFS list set (return last item)</summary>
        /// <returns><see cref="SubjectBuilder"/> for last list item</returns>
        public SubjectBuilder List(params Node[] items)
        {
            if (!items.Any()) return this;

            var rdf = _ctx.Rdf;

            var g = rdf.GetGraph(_ctx.GraphNode
            ?? throw new Exception("Expected graph defined in context"));

            var value = rdf.Uri(RDFS.First);
            var rest = rdf.Uri(RDFS.Rest);
            var nil = rdf.Uri(RDFS.Nil);

            var current = _id;
            foreach (var item in items)
            {
                g.Assert(current, value, item);
                
                var next = g.Blank();
                g.Assert(current, rest, next);

                current = next;
            }

            g.Assert(current, rest, nil);

            return new SubjectBuilder(_owner, current);
        }

        #region IQuadBuilderContext
        IRdfData IQuadBuilderContext.Rdf => ((IQuadBuilderContext)_owner).Rdf;
        
        Node<UriOrBlank>? IQuadBuilderContext.GraphNode => ((IQuadBuilderContext)_owner).GraphNode;
        Node<UriOrBlank>? IQuadBuilderContext.Subject => _id;
        Node<UriOrBlank>? IQuadBuilderContext.Predicate => null;
        #endregion
    }
}