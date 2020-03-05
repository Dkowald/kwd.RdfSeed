using System;
using System.Collections.Generic;
using System.Linq;

using kwd.RdfSeed.Builder;
using kwd.RdfSeed.Compare;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;

namespace kwd.RdfSeed.Core
{
    /// <summary>
    /// Store for quad collection.
    /// <see cref="INodeFactory"/>
    /// operations are directly forwarded to provided factory.
    /// </summary>
    /// <remarks>
    /// Maintain a set of quads.
    /// Helper classes used to get sub-groups such as
    /// a graph (set of quads with all same graph node).
    /// union graph - all quads, but updates limited to specified graph.
    /// In general expect to get a graph subset of the
    /// data to work on.
    /// </remarks>
    public class RdfData : IRdfData
    {
        private const string DefaultSysGraphUri = "rdf:_sys";
        private const string DefaultGraphUri = "rdf:_default";

        private readonly INodeFactory _factory;
        private readonly List<Quad> _data;
        
        /// <summary>
        /// Create an Rdf data store, with config.
        /// </summary>
        public RdfData(INodeFactory factory)
        {
	        _factory = factory ?? new NodeFactory();
            
            _data = new List<Quad>();
        }
        
        #region IRdfData
        /// <inheritdoc />
        public RdfBuilder Update => new RdfBuilder(this);

        /// <inheritdoc />
        public IReadOnlyCollection<Quad> Query =>
	        _data.ToList();

        /// <inheritdoc />
        public UriNode System => _factory.Uri(DefaultSysGraphUri);

        /// <inheritdoc/>
        public UriNode Default => _factory.Uri(DefaultGraphUri);

        /// <inheritdoc/>
        public Node<UriOrBlank>[] GraphIds =>
	        new[] {System, Default}
		        .Union(_data.Select(x => x.Graph).ToArray())
		        .Distinct().ToArray();

        /// <inheritdoc/>
        public IRdfData Assert(Node<UriOrBlank> graph, Node<UriOrBlank> sub, UriNode predicate, Node val)
        {
	        var item = new Quad(graph, sub, predicate, val); 

	        _data.Add(item);
            
	        return this;
	        //return Add(graph, sub, predicate, val);
        }

        /// <inheritdoc />
        public Graph GetGraph(Node<UriOrBlank> id, params Node<UriOrBlank>[] withOther)
        {
	        return new Graph(this, id, 
                _data.Where(x => x.Graph == id),
		        withOther);
        }

        /// <inheritdoc />
        public int Retract(params Quad[] items)
        {
	        var compare = new SameQuad();
	        var qryRefEqual = _data
		        .Where(x => items.Contains(x, compare))
		        .ToArray();

	        foreach (var item in qryRefEqual)
	        {
		        _data.Remove(item);
	        }

	        return qryRefEqual.Length;
        }

        /// <inheritdoc />
        public int Replace(IEnumerable<Quad> newQuads, IEnumerable<Quad> oldQuads)
        {
	        var compare = new SameQuad();
	        var found = _data.Where(x => oldQuads.Contains(x, compare))
		        .ToArray();
	        foreach (var item in found)
	        {
		        _data.Remove(item);
	        }
	        _data.AddRange(newQuads);

	        return found.Length;
        }

        #endregion

        #region INodeFactory
        
        /// <inheritdoc />
        public Node<T> New<T>(T value) where T : notnull =>
	        _factory.New(value);

        /// <inheritdoc />
        public Node New(ReadOnlySpan<char> valueString, ReadOnlySpan<char> typeString)
	        => _factory.New(valueString, typeString);

        /// <inheritdoc />
        public UriNode Uri(ReadOnlySpan<char> uriValue)
	        => _factory.Uri(uriValue);

        /// <inheritdoc />
        public BlankNode Blank(Node<UriOrBlank> scope)
	        => _factory.Blank(scope);

        /// <inheritdoc />
        public BlankNode Blank(Node<UriOrBlank> scope, ReadOnlySpan<char> label)
	        => _factory.Blank(scope, label);

        /// <inheritdoc />
        public BlankNode BlankGraph(ReadOnlySpan<char> label)
	        => _factory.BlankGraph(label);

        /// <inheritdoc />
        public BlankNode BlankGraph()
	        => _factory.BlankGraph();
        
        /// <inheritdoc />
        public IReadOnlyCollection<NodeMap> Mappings => _factory.Mappings;

        /// <inheritdoc />
        public NodeFactoryStats Stats() => _factory.Stats();
        #endregion
    }
}