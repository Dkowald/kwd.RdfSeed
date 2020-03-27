using System.Collections.Generic;
using System.Linq;

using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;

namespace kwd.RdfSeed
{
    /// <summary>
    /// Extensions for <see cref="IRdfData"/>.
    /// </summary>
    public static class RdfDataExtensions
    {
        #region Graph
        /// <summary>
        /// A set of quads ordered by graph.
        /// Includes data from the specified additional graph(s)
        /// </summary>
        public static Graph GetGraph(this IRdfData store, 
	        Node<UriOrBlank> graphId, IEnumerable<Node<UriOrBlank>> other)
	        => store.GetGraph(graphId, other.ToArray());

        /// <summary>Get full graph: graph contains all current quads.</summary>
        public static Graph GetFullGraph(this IRdfData rdf, string graphUri)
         => rdf.GetGraph(rdf.Uri(graphUri), rdf.GraphIds);

        /// <summary>Get full graph: graph contains all current quads.</summary>
        public static Graph GetFullGraph(this IRdfData store, Node<UriOrBlank> graphUri)
	        => store.GetGraph(graphUri, store.GraphIds);

        /// <summary>Get self graph: graph contains only its own quads</summary>
        public static Graph GetSelfGraph(this IRdfData store, Node<UriOrBlank> graphId)
	        => store.GetGraph(graphId, Enumerable.Empty<Node<UriOrBlank>>());

        /// <summary>Get self graph: graph contains only its own quads</summary>
        public static Graph GetSelfGraph(this IRdfData rdf, string graphUri)
			=> rdf.GetGraph(rdf.Uri(graphUri), Enumerable.Empty<Node<UriOrBlank>>());

        /// <summary>Get new blank graph; has all the current data</summary>
        public static Graph GetBlankGraph(this IRdfData store, string? id = null)
            => store.GetGraph(store.BlankGraph(id), store.GraphIds);

        /// <summary>Get default graph with all data</summary>
        public static Graph GetDefault(this IRdfData store)
	        => store.GetGraph(store.Default, store.GraphIds);

        /// <summary>Get system graph with all data</summary>
        public static Graph GetSystem(this IRdfData store)
	        => store.GetGraph(store.System, store.GraphIds);

        /// <summary>Retract a set of quads</summary>
        /// <returns>The number of quads actually removed</returns>
        public static int Retract(this IRdfData self, IEnumerable<Quad> quads)
	        => self.Retract(quads.ToArray());

        #endregion
    }
}