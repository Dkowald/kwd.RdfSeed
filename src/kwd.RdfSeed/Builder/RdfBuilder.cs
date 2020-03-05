using System.Linq;

using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;

namespace kwd.RdfSeed.Builder
{
	/// <summary>
	/// A fluent interface for updating RDF data.
	/// </summary>
	public class RdfBuilder : IQuadBuilderContext
	{
		private readonly IRdfData _owner;

		/// <summary>
		/// Create new RDF builder for fluent updates.
		/// </summary>
		public RdfBuilder(IRdfData data)
		{
			_owner = data;
		}

		/// <summary>Select <see cref="Graph"/>.</summary>
		public GraphBuilder From(Node<UriOrBlank> graphId)
			=> new GraphBuilder(this, _owner.GetGraph(graphId, Enumerable.Empty<Node<UriOrBlank>>()));

		#region IQuadBuilderContext

		IRdfData IQuadBuilderContext.Rdf => _owner;
		
		Node<UriOrBlank>? IQuadBuilderContext.GraphNode => null;
		Node<UriOrBlank>? IQuadBuilderContext.Subject => null;
		Node<UriOrBlank>? IQuadBuilderContext.Predicate => null;
		#endregion IQuadBuilder
	}
}