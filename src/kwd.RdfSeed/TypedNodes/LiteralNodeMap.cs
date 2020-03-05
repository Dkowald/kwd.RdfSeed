using System;
using kwd.Rdf.Std;
using kwd.RdfSeed.Core.Nodes;

namespace kwd.RdfSeed.TypedNodes
{
    /// <summary>
    /// Create new <see cref="string"/> node type
    /// to represent plain literal text node.
    /// </summary>
    public class LiteralNodeMap : NodeMap<string>
    {
        /// <inheritdoc />
        public LiteralNodeMap() 
            : base(XMLSchema.String){}

        /// <inheritdoc />
        public override Node Create(ReadOnlySpan<char> stringValue)
            => Create(new string(stringValue));

        /// <inheritdoc />
        public override Node<string> Create(string stringValue)
            => new Node<string>(this, stringValue, stringValue);
    }
}