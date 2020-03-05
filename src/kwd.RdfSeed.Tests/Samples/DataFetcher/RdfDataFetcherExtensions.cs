using System.IO;
using System.Linq;
using System.Threading.Tasks;

using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;

namespace kwd.RdfSeed.Tests.Samples.DataFetcher
{
	public static class RdfDataFetcherExtensions
	{
		/// <summary>
		/// (re)loads file data; existing graph data is overwritten.
		/// </summary>
		/// <returns>
		/// A Graph with all data, loaded graphs first.
		/// </returns>
		public static async Task<Graph> Load
			(this RdfDataFetcher self, params FileInfo[] file)
			=> await self.Load(file.Select(x => Rdf(self).New(x)).ToArray());

		/// <summary>Try get loaded graph</summary>
		public static Graph? Get(this RdfDataFetcher self, FileInfo file)
			=> self.Get(Rdf(self).New(file));

		public static Node<FileInfo>? Get(this RdfDataFetcher self, Graph g)
			=> self.Get(g.Id);

		/// <summary>Save previously loaded graph </summary>
		/// <exception cref="ErrorNoBackingFile"></exception>
		public static async Task<RdfDataFetcher> Save(this RdfDataFetcher self, Graph g)
			=> await self.Save(g.Id);

		/// <summary>
		/// Disconnect file(s) from fetcher (existing data remains).
		/// </summary>
		public static RdfDataFetcher Close(this RdfDataFetcher self, params FileInfo[] files)
			=> self.Close(files.Select(x => Rdf(self).New(x)).ToArray());

		/// <summary>
		/// Disconnect graph from fetcher
		/// (existing data remains).
		/// </summary>
		public static RdfDataFetcher Close(this RdfDataFetcher self, Graph g)
		 => self.Close(self.Get(g.Id) ?? throw new ErrorNoBackingFile(g.Id));

		public static RdfDataFetcher Purge(this RdfDataFetcher self, Graph g)
			=> self.Purge(self.Get(g) ?? throw new ErrorNoBackingFile(g.Id));

		public static RdfDataFetcher Purge(this RdfDataFetcher self, FileInfo file)
			=> self.Purge(Rdf(self).New(file));

		private static IRdfData Rdf(RdfDataFetcher fetcher)
			=> ((IUseRdf) fetcher).Rdf;
	}
}