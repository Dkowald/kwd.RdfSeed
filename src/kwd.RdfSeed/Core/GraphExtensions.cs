using System.Collections.Generic;
using System.Linq;

namespace kwd.RdfSeed.Core
{
    /// <summary>
    /// Extensions for <see cref="Graph"/>.
    /// </summary>
    public static class GraphExtensions
    {
	    /// <summary>
	    /// Retract (remove) triples from this graph.
	    /// Only quads for this graph (<see cref="Graph.Id"/>) are considered.
	    /// </summary>
	    /// <returns>Number of <see cref="Quad"/> actually removed</returns>
	    public static int Retract(this Graph self, IEnumerable<Quad> quads)
		    => self.Retract(quads.ToArray());
    }
}