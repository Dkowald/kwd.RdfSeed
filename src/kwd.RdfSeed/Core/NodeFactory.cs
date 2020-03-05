using System;
using System.Collections.Generic;
using System.Linq;

using kwd.Rdf.Std;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Errors;
using kwd.RdfSeed.TypedNodes;

namespace kwd.RdfSeed.Core
{
    /// <summary>
    /// Creates nodes for use in NodeSets.
    /// </summary>
    /// <remarks>
    /// Nodes with same valueType, and same valueString
    /// are considered as the same.
    /// The trend is away from string intern toward a local
    /// intern model. This avoids non-release mem.
    /// With this it would be better to use a node pool
    /// approach, since each node is immutable.
    ///
    /// Stuff on blanks
    /// http://www.jsoftware.us/vol7/jsw0709-09.pdf
    /// </remarks>
    public class NodeFactory : INodeFactory
    {
        /// <summary>
        /// Prefix used to identify auto-generated blank label for node.
        /// </summary>
        public const string BlankNodePrefix = "auto-";

        /// <summary>
        /// Prefix used to identify auto-generated label for graph node.
        /// </summary>
        public const string BlankGraphPrefix = "graph-";

        private class ScopeCounter
        {
            private int _counter;
            public ScopeCounter(Node<UriOrBlank>? scope)
            { Scope = scope;}

            public readonly Node<UriOrBlank>? Scope;
            
            public int Next() => ++_counter;

            public ScopeCounter SkipTo(int? value)
            {
                if(value.HasValue)
                    _counter = value.Value;

                return this;
            }
        }

        private readonly List<ScopeCounter> 
            _scopedCounters =new List<ScopeCounter>();

        /// todo: revisit to look at a week-ref list;
        /// nodes may be transient
        /// look at re-use GC'd node spot, just before
        /// alloc more space.
        private readonly List<Node> _nodes = new List<Node>();

        private readonly List<NodeMap> _nodeMaps = new List<NodeMap>();

        private readonly UriOrBlankMap _linkNodeBuilder;
        
        /// <summary>
        /// Create node factory, using set of node builders.
        /// </summary>
        /// <exception cref="AlreadyRegisteredNativeType"></exception>
        /// <exception cref="AlreadyRegisteredTypeString"></exception>
        public NodeFactory(IEnumerable<NodeMap> builders)
        {
            _linkNodeBuilder = new UriOrBlankMap();
            AddNodeType(_linkNodeBuilder);

            builders ??= Enumerable.Empty<NodeMap>();
            
            foreach (var builder in builders)
            { AddNodeType(builder); }
        }

        /// <summary>
        /// Create node factory, using set of node builders.
        /// </summary>
		/// <exception cref="AlreadyRegisteredNativeType"></exception>
        /// <exception cref="AlreadyRegisteredTypeString"></exception>
        public NodeFactory(params NodeMap[] builders)
            :this(builders.AsEnumerable()){}

        /// <summary>
        /// Create default factory; using all typed nodes.
        /// </summary>
        public NodeFactory()
            :this(TypedNodesHelpers.AllNodeMappings()){}

        /// <summary>Report the current number of nodes.</summary>
        public int NodeCount => _nodes.Count;

        #region INodeFactory

        /// <inheritdoc />
        public Node<T> New<T>(T value) where T:notnull
        {
	        TypeMustNotBeANode.Verify<T>();

            //Uri not supported.
            if (value is Uri)
	            throw new UriObjectNodeNotSupported();

            //Try native object equality.
            var found = _nodes
                .FirstOrDefault(x => 
                    x is Node<T> v &&
                    v.Value.Equals(value));

            if (found != null) return (Node<T>)found;

            //Find a Node map to use.
            var nodeMap = _nodeMaps
	            .OfType<NodeMap<T>>()
	            .FirstOrDefault(x => !(x is TypedLiteralMap)) ??
	            AddNodeType(new ObjectNodeMap<T>());
            
            //Create temporary.
            var tempNode = nodeMap.Create(value);

            //again try match
            if (tempNode.ValueString != null)
            {
                found = _nodes
		                .FirstOrDefault(x => 
                            x.ValueType == tempNode.ValueType &&
			                x.ValueStringHash == tempNode.ValueStringHash &&
                            x.ValueString == tempNode.ValueString);
                    
                if (found != null) return (Node<T>)found;
            }
            
            //New 
            _nodes.Add(tempNode);
            
            return tempNode;
        }

        /// <inheritdoc />
        public Node New(ReadOnlySpan<char> valueString, ReadOnlySpan<char> typeString)
        {
	        if (typeString.SequenceEqual(XMLSchema.AnyUri))
		        return Uri(valueString);
            
            //Find matching node map
            var typeStringHash = string.GetHashCode(typeString);
            NodeMap? nodeMap = null;
            var qry = _nodeMaps
	            .Where(x => x.DataTypeHash == typeStringHash);
            foreach (var item in qry)
            {
                if (typeString.SequenceEqual(item.DataType))
                {nodeMap = item; break;}
            }

            nodeMap ??=  AddNodeType(new TypedLiteralMap(typeString));
            
            //Try match on node map and value string.
            // (assumes in general the value string is already normalized)
            Node? found = null;
            var valueStringHash = string.GetHashCode(valueString);
            var qryVal = _nodes
                .Where(x => x.ValueStringHash == valueStringHash && x.ValueType == nodeMap);
            foreach (var node in qryVal)
            {
                if (valueString.SequenceEqual(node.ValueString))
                { found = node; break;}
            }

            if (found != null) return found;

            //Create temporary, try match with it.
            var tempNode = nodeMap.Create(valueString);
            
            //try temp match.
            if (tempNode.ValueString != null)
            {
	            found = _nodes
		            .FirstOrDefault(x => 
                        x.ValueType == tempNode.ValueType &&
			            x.ValueStringHash == tempNode.ValueStringHash && 
			            x.ValueString == tempNode.ValueString);

	            if (found != null)
		            return found;
            }

            //new
	        _nodes.Add(tempNode);
            return tempNode;
        }

        /// <inheritdoc />
        public UriNode Uri(ReadOnlySpan<char> uriValue)
        {
            UriNode? found = null;
            var uriHash = string.GetHashCode(uriValue);
            var qry = _nodes
                .Where(x => x.ValueStringHash == uriHash)
                .OfType<UriNode>();
            foreach (var item in qry)
            {
                if (uriValue.SequenceEqual(item.ValueString))
                { found = item; break;}
            }

            if(found is null)
            {
                found = _linkNodeBuilder.New(uriValue);
                _nodes.Add(found);
            }

            return found;
        }

        /// <inheritdoc />
        public BlankNode Blank(Node<UriOrBlank> scope)
        {
            var label = BlankNodePrefix + Counter(scope).Next();
            
            var blank = _linkNodeBuilder.NewBlank(scope, label);
                
            _nodes.Add(blank);

            return blank;
        }

        /// <inheritdoc />
        public BlankNode Blank(Node<UriOrBlank> scope, ReadOnlySpan<char> label)
        {
            if (label.IsEmpty) return Blank(scope);

            var valueString = BlankNode.ValueStringFromLabel(label);
            var valueStringHash = string.GetHashCode(valueString);

            var qry = _nodes
                .Where(x => x.ValueStringHash == valueStringHash)
                .OfType<BlankNode>()
                .Where(x => !x.IsSelfScoped() && x.Value.Scope == scope);
                
            BlankNode? found = null;
            foreach (var item in qry)
            {
                if (valueString.SequenceEqual(item.ValueString))
                { found = item;break;}
            }

            if (found is null)
            {
                Counter(scope).SkipTo(TryReadLabelIndex(BlankNodePrefix, label));
                found = _linkNodeBuilder
                    .NewBlank(scope, new string(label));

                _nodes.Add(found);
            }

            return found;
        }
        
        /// <inheritdoc />
        public BlankNode BlankGraph(ReadOnlySpan<char> label)
        {
            if (label.IsEmpty) return BlankGraph();
            
            var valueString = BlankNode.ValueStringFromLabel(label);
            
            var valueStringHash = string.GetHashCode(valueString);

            var qry = _nodes
                .Where(x => x.ValueStringHash == valueStringHash)
                .OfType<BlankNode>()
                .Where(x => x.IsSelfScoped());

            BlankNode? found = null;
            
            foreach (var item in qry)
            {
                if (valueString.SequenceEqual(item.ValueString))
                {
                    found = item; break;
                }
            }

            if (found is null)
            {
                Counter(null).SkipTo(TryReadLabelIndex(BlankGraphPrefix, label));
                found = _linkNodeBuilder.NewSelfScoped(new string(valueString));
                _nodes.Add(found);
            }

            return found;
        }

        /// <inheritdoc />
        public BlankNode BlankGraph()
        {
            var valueString = BlankGraphPrefix + Counter(null).Next();

            var node = _linkNodeBuilder.NewSelfScoped(valueString);
    
            _nodes.Add(node);

            return node;
        }
        
        /// <inheritdoc />
        public NodeFactoryStats Stats()
        {
	        var stats = new NodeFactoryStats(
		        _nodes.Count,
		        _nodes.Count(x => x is BlankNode),
		        _nodes.Count(x =>
			        x is BlankNode b &&
			        (b.Label.StartsWith(BlankNodePrefix) ||
			         b.Label.StartsWith(BlankGraphPrefix)))
	        );

	        return stats;
        }

        #endregion

        #region INodeMapper

        /// <inheritdoc />
        public IReadOnlyCollection<NodeMap> Mappings => _nodeMaps;

        #endregion

        private T AddNodeType<T>(T nodeMap) where T: NodeMap
        {
            var found = _nodeMaps.SingleOrDefault(x => ReferenceEquals(x, nodeMap));
            if (found != null) return (T)found;

            if(_nodeMaps.Any(x => x.DataType == nodeMap.DataType))
	            throw new AlreadyRegisteredTypeString(nodeMap.DataType);

            //Allow multiple typed literal builders.
            if(_nodeMaps.Any(x => !(nodeMap is TypedLiteralMap) &&
                               x.Native == nodeMap.Native))
	            throw new AlreadyRegisteredNativeType(nodeMap.Native);

            _nodeMaps.Add(nodeMap);

            return nodeMap;
        }

        private ScopeCounter Counter(Node<UriOrBlank>? scope)
        {
            var found = _scopedCounters.FirstOrDefault(x => x.Scope == scope);
            if (found is null)
            {
                found = new ScopeCounter(scope);
                _scopedCounters.Add(found);
            }
            return found;
        }

        private static int? TryReadLabelIndex(string prefix, ReadOnlySpan<char> valueString)
        {
            if (valueString.StartsWith(prefix)
                && int.TryParse(valueString.Slice(prefix.Length), out var val))
                return val;

            return null;
        }
    }
}