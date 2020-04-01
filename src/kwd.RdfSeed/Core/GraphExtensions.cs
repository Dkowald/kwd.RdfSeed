using System.Collections.Generic;
using System.Linq;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;

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

	    /// <summary>Assert / add a new triple for this graph</summary>
	    public static Graph Assert(this Graph self, Node<UriOrBlank> subject, UriNode predicate, Node @object)
		    => self.Assert(subject, predicate, @object, out _);
    }
}