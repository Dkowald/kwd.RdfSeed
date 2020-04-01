using System.IO;

using kwd.CoreUtil.FileSystem;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Serialize.NTriple;
using kwd.RdfSeed.Tests.TestHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Samples
{
	[TestClass]
	public class UsingNTripleFile
	{
		private FileInfo SampleFile()
		{
			var file = 
				Files.TestData.Sample1
				.CopyTo(Files.AppDataDir.Get(nameof(UsingNTripleFile)), true);

			return file;
		}

		[TestMethod]
		public void LoadAFile()
		{
			//Load a sample N-triple file
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
			//Write a sample N-triple file
			var file = Files.AppDataDir.GetFile(
				nameof(UsingNTripleFile), nameof(WriteAFile) + ".nt");

			file.EnsureDelete();

			var rdf = RdfDataFactory.CreateNoLock();
			var g = rdf.GetBlankGraph();

			g.Assert(g.Uri("test:sub"), g.Uri("test:pred"), g.New("a name"));

			new NTripleFile(file).Write(g).Wait();

			Assert.IsTrue(file.Exists() && file.Length > 0, 
				"File created with some data");
		}
	}
}
