using System.IO;

using kwd.CoreUtil.FileSystem;
using kwd.Rdf.Std;
using kwd.RdfSeed.Builder;
using kwd.RdfSeed.Query;
using kwd.RdfSeed.Serialize.NTriple;
using kwd.RdfSeed.Tests.TestHelpers;
using kwd.RdfSeed.Util;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Samples
{
	[TestClass]
	public class QuickStart
	{
		private readonly FileInfo _sampleData = Files.TestData.Brazil;

		private const string SubjectId = "http://www.heml.org/docs/samples/heml/2002-05-29/brazil.xmlFRAGbrazil";

		[TestMethod]
		public void Go()
		{
			//Create a store
			var rdf = RdfDataFactory.CreateDefault();

			//new graph node.
			var sample = rdf.GetBlankGraph("SampleDataGraph");

			//load some data.
			//  n-triple file read / write.
			new NTripleFile(_sampleData).Read(sample).Wait();

			//find some data.
			var brazil = rdf.Uri(SubjectId);
			var name = sample.Query
				.For(brazil)
				.With(rdf, RDFS.Label)
				.Get<string>()
				.FirstOrNull();

			Assert.IsNotNull(name);

			//Add some data.
			var haveVisited = rdf.Uri("app:haveVisited");
			sample.Update
				.For(brazil)
				.With(haveVisited)
				.Add(false);

			//Save some data
			new NTripleFile(Files.AppDataDir.GetFile("Updated.nt"))
				.Write(sample).Wait();
		}
	}
}