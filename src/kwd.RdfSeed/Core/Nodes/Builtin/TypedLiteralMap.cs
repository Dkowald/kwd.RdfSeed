using System;

namespace kwd.RdfSeed.Core.Nodes.Builtin
{
    /// <summary>
    /// Creates nodes where no mapping for dataType
    /// (uses a   Node&lt;string&gt;).
    /// </summary>
    public class TypedLiteralMap : NodeMap<string>
    {
        /// <summary>Create new <see cref="TypedLiteralMap"/>.</summary>
        public TypedLiteralMap(ReadOnlySpan<char> typeString)
        :base(new string(typeString)){}

        /// <inheritdoc />
        public override Node Create(ReadOnlySpan<char> valueString)
            => Create(new string(valueString));

        /// <inheritdoc />
        public override Node<string> Create(string value)
            => new Node<string>(this, value, value);
    }
}