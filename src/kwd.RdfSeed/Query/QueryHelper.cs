using System.Collections.Generic;
using System.Linq;
using kwd.RdfSeed.Core;

namespace kwd.RdfSeed.Query
{
	internal static class QueryHelper
	{
		internal static IReadOnlyList<Quad> Ro(this IEnumerable<Quad> self)
			=> self as IReadOnlyList<Quad> ?? self.ToArray();
	}
}