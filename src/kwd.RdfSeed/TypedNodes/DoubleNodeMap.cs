using System;
using kwd.Rdf.Std;
using kwd.RdfSeed.Core.Nodes;

namespace kwd.RdfSeed.TypedNodes
{
    /// <summary>
    /// Create <see cref="double"/> type node.
    /// </summary>
    public class DoubleNodeMap : NodeMap<double>
    {
        private readonly int _precision;

        /// <summary>Create new <see cref="DoubleNodeMap"/>.</summary>
        public DoubleNodeMap(int precision = 4)
            :base(XMLSchema.Double)
        {
            _precision = precision;
        }

        /// <inheritdoc />
        public override Node Create(ReadOnlySpan<char> valueString)
            => Create(double.Parse(valueString));

        /// <inheritdoc />
        public override Node<double> Create(double value)
            => new Node<double>(this, Math.Round(value, _precision), 
                value.ToString($"F{_precision}")); 
    }
}