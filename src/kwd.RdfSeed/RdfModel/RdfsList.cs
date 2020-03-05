using System.Collections.Generic;
using System.Linq;

using kwd.Rdf.Std;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Query;
using kwd.RdfSeed.Util;

namespace kwd.RdfSeed.RdfModel
{
    /// <summary>
    /// Wraps a graph providing operations
    /// using RDFS schema.
    /// </summary>
    /// <remarks>
    /// Null list items are recorded
    /// as RDFS Nil: <see cref="RDFS.Nil"/>.
    /// </remarks>
    public class RdfsList
    {
        private readonly INodeFactory _f;
        private readonly Graph _owner;

        private readonly UriNode _first;
        private readonly UriNode _next;
        private readonly UriNode _nil;

        /// <summary>Create an new <see cref="RdfsList"/>.</summary>
        public RdfsList(INodeFactory f, Graph owner)
        {
            _f = f;
            _owner = owner;

            _first = f.Uri(RDFS.First);
            _next = f.Uri(RDFS.Rest);
            _nil = f.Uri(RDFS.Nil);
        }

        /// <summary>
        /// Assert a series of quads.
        /// Returns the starting point for the set.
        /// </summary>
        public Node<UriOrBlank> AddList(Node<UriOrBlank> root, params Node?[] items)
        {
            if (!items.Any()) return root;

            var cur = root;

            for (var i = 0; i < items.Length; i++)
            {
                var next = i == items.Length - 1 ? 
	                (Node<UriOrBlank>)_nil : 
	                _f.Blank(_owner.Id);
                
                _owner.Assert(cur, _first, items[i] ?? _nil)
                    .Assert(cur, _next, next);

                cur = next;
            }

            return root;
        }

        /// <summary>
        /// Given the root of a list;
        /// get all object nodes for the set..
        /// </summary>
        public IEnumerable<Node> GetList(Node<UriOrBlank> root)
        {
            var cur = root;
            while (cur != _nil)
            {
                var val = _owner.Query.For(cur).With(_first)
                    .SingleOrNull();
                if (!(val is null))
                    yield return val.Object;

                cur = _owner.Query.For(cur).With(_next)
                          .SingleOrNull()?.Object.As<UriOrBlank>()
                      ?? _nil;
            }
        }
    }
}