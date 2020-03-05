using System.IO;

using kwd.CoreUtil.FileSystem;
using kwd.RdfSeed.Serialize.NTriple;
using kwd.RdfSeed.Tests.TestHelpers;
using kwd.RdfSeed.TypedNodes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Samples
{
	[TestClass]
	public class StoringData
	{
		private FileInfo SampleFile()
		{
			var file = 
				Files.TestData.Sample1
				.CopyTo(Files.AppDataDir.Get(nameof(StoringData)), true);

			return file;
		}

		[TestMethod]
		public void LoadAFile()
		{
			var rdf = RdfDataFactory.CreateNoLock();

			var file = SampleFile();

			var g = rdf.GetBlankGraph();

			//Read file data into the graph g
			new NTripleFile(file).Read(g).Wait();

			var tripleCount = g.Query.Count;
			Assert.IsTrue(tripleCount > 0, "Loaded some data");
		}

		[TestMethod]
		public void WriteAFile()
		{
			var file = Files.AppDataDir.GetFile(
				nameof(StoringData), nameof(WriteAFile) + ".nt");

			file.EnsureDelete();

			var rdf = RdfDataFactory.CreateNoLock();
			var g = rdf.GetBlankGraph();

			g.Assert(g.Uri("test:sub"), g.Uri("test:pred"), g.Literal("a name"));

			new NTripleFile(file).Write(g).Wait();

			Assert.IsTrue(file.Exists() && file.Length > 0, 
				"File created with some data");
		}
	}
}
