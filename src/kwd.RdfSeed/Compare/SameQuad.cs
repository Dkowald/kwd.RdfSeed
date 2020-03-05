using System;
using System.Collections.Generic;

using kwd.RdfSeed.Core;

namespace kwd.RdfSeed.Compare
{
    /// <summary>
    /// Compare Quad for quad equality.
    /// </summary>
    public class SameQuad : IEqualityComparer<Quad>
    {
        /// <inheritdoc />
        public bool Equals(Quad x, Quad y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            return new SameTriple().Equals(x, y) &&
	            x.Graph == y.Graph;
        }

        /// <inheritdoc />
        public int GetHashCode(Quad obj) => 
            HashCode.Combine(obj.Graph,obj.Subject, obj.Predicate, obj.Object);
    }
}