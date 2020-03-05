using System;
using kwd.Rdf.Std;
using kwd.RdfSeed.Core.Nodes;

namespace kwd.RdfSeed.TypedNodes
{
    /// <summary>
    /// Create new <see cref="int"/> nodes.
    /// </summary>
    public class IntegerNodeMap : NodeMap<int>
    {
        /// <summary>Create new <see cref="IntegerNodeMap"/></summary>
        public IntegerNodeMap()
            :base(XMLSchema.Integer){}

        /// <inheritdoc />
        public override Node Create(ReadOnlySpan<char> valueString)
            => Create(int.Parse(valueString));

        /// <inheritdoc />
        public override Node<int> Create(int value)
            => new Node<int>(this, value, value.ToString());
    }
}