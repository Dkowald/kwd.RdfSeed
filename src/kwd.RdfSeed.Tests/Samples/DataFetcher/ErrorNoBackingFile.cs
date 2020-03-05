using System;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;

namespace kwd.RdfSeed.Tests.Samples.DataFetcher
{
	public class ErrorNoBackingFile : Exception
	{
		public ErrorNoBackingFile(Node<UriOrBlank> graphId)
			:base($"The graph has no backing file: {graphId}")
		{}
	}
}