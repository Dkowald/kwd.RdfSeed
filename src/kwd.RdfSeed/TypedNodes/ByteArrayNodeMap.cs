using System;
using kwd.Rdf.Std;
using kwd.RdfSeed.Core.Nodes;

namespace kwd.RdfSeed.TypedNodes
{
    /// <summary>
    /// Create Binary data (byte[]) node types.
    /// </summary>
    public class ByteArrayNodeMap : NodeMap<byte[]>
    {
        /// <summary>
        /// Create new <see cref="ByteArrayNodeMap"/>.
        /// </summary>
        public ByteArrayNodeMap()
            :base(XMLSchema.Binary){}

        /// <inheritdoc />
        public override Node Create(ReadOnlySpan<char> valueString)
        {
            var value = new string(valueString);
            
            return new Node<byte[]>(this, Convert.FromBase64String(value), value);
        }

        /// <inheritdoc />
        public override Node<byte[]> Create(byte[] value)
            => new Node<byte[]>(this, value, Convert.ToBase64String(value));
    }
}