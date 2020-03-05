using System;
using kwd.Rdf.Std;
using kwd.RdfSeed.Core.Nodes;

namespace kwd.RdfSeed.TypedNodes
{
    /// <summary>
    /// Create DateTime type nodes.
    /// </summary>
    public class DateTimeNodeMap : NodeMap<DateTime>
    {
        /// <summary>
        /// Create new <see cref="DateTimeNodeMap"/>.
        /// </summary>
        public DateTimeNodeMap() 
            : base(XMLSchema.DateTime){}

        /// <inheritdoc />
        public override Node Create(ReadOnlySpan<char> valueString)
            => Create(DateTime.Parse(valueString));

        /// <inheritdoc />
        public override Node<DateTime> Create(DateTime value)
            => new Node<DateTime>(this, value.ToUniversalTime(), value.ToString("O"));
    }
}