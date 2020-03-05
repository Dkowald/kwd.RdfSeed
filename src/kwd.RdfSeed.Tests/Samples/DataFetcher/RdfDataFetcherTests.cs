using System.Linq;
using System.Threading.Tasks;

using kwd.CoreUtil.FileSystem;
using kwd.RdfSeed.Tests.Samples.Rdf;
using kwd.RdfSeed.Tests.TestHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Samples.DataFetcher
{
	[TestClass]
	public class RdfDataFetcherTests
	{
		private readonly IRdfData _rdf 
			= RdfDataFactory.CreateDefault(new FileInfoNodeMap());

		[TestMethod]
		public async Task Load_()
		{
			var loader = new RdfDataFetcher(_rdf);
			
			var brazil = await loader.Load(Files.TestData.Brazil);

			Assert.IsTrue(brazil.Query.Count > 0, "Loaded some data");
		}

		[TestMethod]
		public async Task Save_()
		{
			var file = Files.TestData.Sample1.CopyTo(
				Files.AppDataDir.GetFolder(nameof(RdfDataFetcherTests)), true);

			var loader = new RdfDataFetcher(_rdf);
			var g = await loader.Load(file);

			var preUpdate = g.Query.Count;

			g.Assert(g.Blank(), g.Uri("app:isTest"), g.New(true));
			await loader.Save(g);
			g.Clear();

			await loader.Load(file);
			var postUpdate = g.Query.Count;

			Assert.IsTrue(postUpdate > preUpdate);
		}

		[TestMethod]
		public async Task List_()
		{
			var loader = new RdfDataFetcher(_rdf);
			var g = await loader.Load(Files.TestData.Sample1, Files.TestData.Brazil);

			Assert.AreEqual(1, g.Other.Count, "Loaded the data files.");
		}

		[TestMethod]
		public async Task Close_()
		{
			var loader = new RdfDataFetcher(_rdf);
			var sample = await loader.Load(Files.TestData.Sample1);

			var preClose = loader.List().Count;
			loader.Close(sample);

			var postClose = loader.List().Count;
			Assert.IsTrue(postClose < preClose, "File closed");
			Assert.IsTrue(sample.Query.Any(), "But data kept");
		}

		[TestMethod]
		public async Task Purge_()
		{
			var loader = new RdfDataFetcher(_rdf);

			var g = await loader.Load(Files.TestData.Sample1);

			loader.Purge(Files.TestData.Sample1);

			Assert.IsFalse(loader.List().Any(), "backing file removed.");
			_rdf.GetGraph(g.Id);
		}
	}
}