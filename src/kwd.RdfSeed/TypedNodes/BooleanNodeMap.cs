using System;
using kwd.Rdf.Std;
using kwd.RdfSeed.Core.Nodes;

namespace kwd.RdfSeed.TypedNodes
{
    /// <summary>
    /// Creates Bool node types.
    /// </summary>
    public class BooleanNodeMap : NodeMap<bool>
    {
        /// <summary>Create new <see cref="BooleanNodeMap"/>.</summary>
        public BooleanNodeMap()
            :base(XMLSchema.Boolean){}

        /// <inheritdoc />
        public override Node Create(ReadOnlySpan<char> valueString)
            => Create(bool.Parse(valueString));

        /// <inheritdoc />
        public override Node<bool> Create(bool value)
            => new Node<bool>(this, value, value ? "true" : "false");
    }
}