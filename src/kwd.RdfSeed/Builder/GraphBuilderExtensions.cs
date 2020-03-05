using System;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;

namespace kwd.RdfSeed.Builder
{
	/// <summary>
	/// Sugar for <see cref="GraphBuilder"/>.
	/// </summary>
	public static class GraphBuilderExtensions
	{
		/// <summary>Select URI subject.</summary>
		public static SubjectBuilder For(this GraphBuilder self, string uriSubject, out UriNode node)
		{
			node = self.Rdf().Uri(uriSubject);
			return self.For(node);
		}

		/// <summary>Select URI subject.</summary>
		public static SubjectBuilder For(this GraphBuilder self, Uri uriSubject, out UriNode node)
		{
			node = self.Rdf().Uri(uriSubject.ToString());
			return self.For(node);
		}

		/// <summary>New blank auto node subject.</summary>
		public static SubjectBuilder ForBlank(this GraphBuilder self, out BlankNode node)
		{
			node = self.Rdf().Blank(self.GraphId());
			return self.For(node);
		}

		/// <summary>New blank labeled node subject.</summary>
		public static SubjectBuilder ForBlank(this GraphBuilder self, string label, out BlankNode node)
		{
			node = self.Rdf().Blank(self.GraphId(), label);
			return self.For(node);
		}

		/// <summary>New blank labeled node subject.</summary>
		public static SubjectBuilder ForBlank(this GraphBuilder self, ReadOnlySpan<char> label, out BlankNode node)
		{
			node = self.Rdf().Blank(self.GraphId(), label);
			return self.For(node);
		}

		private static IRdfData Rdf(this GraphBuilder self) =>
			((IQuadBuilderContext) self).Rdf;

		private static Node<UriOrBlank> GraphId(this GraphBuilder self) =>
			((IQuadBuilderContext) self).GraphNode ??
			throw new Exception($"Expected Graph id for {nameof(GraphBuilder)}.");
	}
}