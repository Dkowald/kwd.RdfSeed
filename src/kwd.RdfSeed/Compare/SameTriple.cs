using System;
using System.Collections.Generic;

using kwd.RdfSeed.Core;

namespace kwd.RdfSeed.Compare
{
    /// <summary>
    /// Compare Quads for triple equality.
    /// </summary>
    public class SameTriple: IEqualityComparer<Quad>
    {
        /// <inheritdoc />
        public bool Equals(Quad x, Quad y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            return x.Subject == y.Subject &&
                   x.Predicate == y.Predicate &&
                   x.Object == y.Object;
        }

        /// <inheritdoc />
        public int GetHashCode(Quad obj) =>
            HashCode.Combine(obj.Subject, obj.Predicate, obj.Object);
    }
}