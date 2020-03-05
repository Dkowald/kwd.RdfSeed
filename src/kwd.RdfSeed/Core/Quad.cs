using System.Diagnostics;

using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;

namespace kwd.RdfSeed.Core
{
    /// <summary>
    /// A Quad; the basic relationship made up of nodes.
    /// </summary>
    [DebuggerDisplay("{kwd.RdfSeed.RdfDebug.Print(this)}")]
    public class Quad
    {
        /// <summary>Create new <see cref="Quad"/>.</summary>
        public Quad(Node<UriOrBlank> graph, Node<UriOrBlank> subject, 
            UriNode predicate, Node @object)
        {
            Graph = graph;
            Subject = subject;
            Predicate = predicate;
            Object = @object;
        }
        
        /// <summary>
        /// Name of the graph that owns the triple.
        /// </summary>
        public readonly Node<UriOrBlank> Graph;

        /// <summary>
        /// The subject / target of the triple
        /// </summary>
        public readonly Node<UriOrBlank> Subject;
            
        /// <summary>
        /// The predicate / relationship for the triple
        /// </summary>
        public readonly UriNode Predicate;

        /// <summary>
        /// The object / value for the triple.
        /// </summary>
        public readonly Node Object;
    }
}