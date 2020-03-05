using System;
using System.Diagnostics;

namespace kwd.RdfSeed.Core.Nodes
{
    /// <summary>
    /// Base Node for all RDF Nodes.
    /// </summary>
    /// <remarks>
    /// With generics its generally useful to have a
    /// non-generic base.
    /// </remarks>
    [DebuggerDisplay("{kwd.RdfSeed.RdfDebug.Print(this)}")]
    public abstract class Node
    {
        /// <summary>Create new <see cref="Node"/>.</summary>
        protected Node(ReadOnlySpan<char> valueString, NodeMap valueType)
        {
            ValueString = valueString.IsEmpty ? null : new string(valueString);
            ValueType = valueType;
            ValueStringHash = ValueString?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// Value represented as a string.
        /// </summary>
        public string? ValueString { get; }

        /// <summary>
        /// The value type.
        /// </summary>
        public readonly NodeMap ValueType;

        /// <summary>Hash for <see cref="ValueString"/></summary>
        public readonly int ValueStringHash;
    }

    /// <summary>
    /// A <see cref="Node"/> with strongly typed value.
    /// </summary>
    public class Node<T> : Node where T:notnull
    {
        /// <summary>Create new <see cref="Node{T}"/>.</summary>
        public Node(NodeMap<T> spec, T value, string? valueString = null)
        :base(valueString, spec)
        {
            Value = value;
        }

        /// <summary>The typed node value</summary>
        public T Value;
    }
}
