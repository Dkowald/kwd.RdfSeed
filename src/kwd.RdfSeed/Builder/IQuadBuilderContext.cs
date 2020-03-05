using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;

namespace kwd.RdfSeed.Builder
{
    interface IQuadBuilderContext
    {
        IRdfData Rdf { get; }

        Node<UriOrBlank>? GraphNode { get; }
        Node<UriOrBlank>? Subject { get; }
        Node<UriOrBlank>? Predicate { get; }
    }
}