using System;
using System.Linq;

using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Errors;
using kwd.RdfSeed.Query;

namespace kwd.RdfSeed.Builder
{
    /// <summary>
    /// Fluent interface where Graph; Subject and Predicate have been selected.
    /// </summary>
    public class PredicateBuilder : IQuadBuilderContext
    {
        private readonly SubjectBuilder _owner;
        private readonly UriNode _id;
        private readonly IQuadBuilderContext _ctx;

        /// <summary>
        /// Create new <see cref="PredicateBuilder"/>.
        /// </summary>
        public PredicateBuilder(SubjectBuilder forSubject, UriNode id)
        {
            _owner = forSubject;
            _id = id;
            _ctx = this;
        }

        /// <summary>
        /// Keep reference to current predicate in a variable.
        /// </summary>
        public PredicateBuilder Let(out UriNode id)
        {
            id = _id;
            return this;
        }

        /// <summary>
        /// Uses graph; subject and predicate to
        /// Assert quads with given values.
        /// </summary>
        public PredicateBuilder Add(params Node[] values)
        {
            foreach (var item in values)
                _ctx.Rdf.Assert(
                    _ctx.GraphNode ?? throw new Exception("Expected graph for quad"),
                    _ctx.Subject ?? throw new Exception("Expected subject for quad"),
                    _id, item);

            return this;
        }

        /// <summary>
        /// Retract any values that match the value type(s)
        /// </summary>
        public PredicateBuilder Remove(params Node[] values)
        {
            var found = _ctx.Rdf.Query.From(GetGraphId()).With(_id)
		        .IsType(values);

            _ctx.Rdf.Retract(found.ToArray());

            return this;
        }

        /// <summary>Retract any values that match the value type(s)</summary>
        /// <exception cref="TypeMustNotBeANode"></exception>
        public PredicateBuilder Remove<T>() where T : notnull
        {
	        var found = _ctx.Rdf.Query.From(GetGraphId()).With(_id)
		        .IsType<T>();

	        _ctx.Rdf.Retract(found.ToArray());

	        return this;
        }

        /// <summary>Back to current subject.</summary>
        public SubjectBuilder Then() => _owner;

        /// <summary>Back to other subject.</summary>
        public SubjectBuilder ThenFor(Node<UriOrBlank> subject) =>
            _owner.Then().For(subject);

        #region IQuadBuilderContext
        IRdfData IQuadBuilderContext.Rdf => ((IQuadBuilderContext)_owner).Rdf;
        
        Node<UriOrBlank>? IQuadBuilderContext.GraphNode => ((IQuadBuilderContext)_owner).GraphNode;
        Node<UriOrBlank>? IQuadBuilderContext.Subject => ((IQuadBuilderContext)_owner).Subject;
        Node<UriOrBlank>? IQuadBuilderContext.Predicate => _id;
        #endregion

        private Node<UriOrBlank> GetGraphId() =>
	        ((IQuadBuilderContext) _owner).GraphNode ??
	        throw new Exception("Expected context to have a graph id");
    }
}