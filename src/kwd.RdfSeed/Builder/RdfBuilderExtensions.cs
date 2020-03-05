using System;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes.Builtin;

namespace kwd.RdfSeed.Builder
{
	/// <summary>
	/// Shorthand helpers for <see cref="RdfBuilder"/>.
	/// </summary>
	public static class RdfBuilderExtensions
	{
		/// <summary>Select URI <see cref="Graph"/>.</summary>
		public static GraphBuilder From(this RdfBuilder self,
			string graphUri, out UriNode node)
		{
			node = self.Rdf().Uri(graphUri);
			return self.From(node);
		}

		/// <summary>Select URI <see cref="Graph"/>.</summary>
		public static GraphBuilder From(this RdfBuilder self, Uri graphUri)
			=> self.From(self.Rdf().Uri(graphUri.ToString()));

		/// <summary>New <see cref="Graph"/> with auto-blank node</summary>
		public static GraphBuilder FromBlank(this RdfBuilder self, out BlankNode node)
		{
			node = self.Rdf().BlankGraph();

			return self.From(node);
		}

		/// <summary>New <see cref="Graph"/> with labeled-blank node</summary>
		public static GraphBuilder FromBlank(this RdfBuilder self, string label, out BlankNode node)
		{
			node = self.Rdf().BlankGraph(label);

			return self.From(node);
		}

		/// <summary>New <see cref="Graph"/> with labeled-blank node</summary>
		public static GraphBuilder FromBlank(this RdfBuilder self, ReadOnlySpan<char> label, out BlankNode node)
		{
			node = self.Rdf().BlankGraph(label);

			return self.From(node);
		}

		private static IRdfData Rdf(this RdfBuilder self) =>
			((IQuadBuilderContext) self).Rdf;
	}
}