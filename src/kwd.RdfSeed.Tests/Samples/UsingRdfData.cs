using System.Linq;

using kwd.Rdf.Std.DublinCore;
using kwd.RdfSeed.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Samples
{
	/// <summary>Demonstrate using <see cref="IRdfData"/>.</summary>
	[TestClass]
	public class UsingRdfData
	{
		[TestMethod]
		public void CreateRdfStore()
		{
			//create default thread-locked version.
			IRdfData rdf = RdfDataFactory.CreateDefault();
			Assert.IsTrue(rdf is RdfDataThreadLock);

			//create basic version.
			rdf = RdfDataFactory.CreateNoLock();
			Assert.IsTrue(rdf is RdfData);

			//setup app singleton
			RdfDataFactory.Init(RdfDataFactory.CreateNoLock());

			//use singleton
			rdf = RdfDataFactory.AppData;

			Assert.IsNotNull(rdf);
		}

		[TestMethod]
		public void GeneralUsage()
		{
			var rdf = RdfDataFactory.CreateNoLock();

			//Self scoped blank node.
			var graph = rdf.BlankSelf("self-blank");

			//Blank node scoped to graph
			var subject = rdf.Blank(graph);

			//uri predicate, from Dublin core
			var predicate = rdf.Uri(Terms.Title);

			var @object = rdf.New("a title from my stuff");

			//add quad to rdf data
			rdf.Assert(graph, subject, predicate, @object);

			//query
			var quad = rdf.Query.Single();

			//remove
			rdf.Retract(quad);

			Assert.AreEqual(0, rdf.Query.Count);

			//replace.
			rdf.Replace(new[]{
				new Quad(graph, rdf.Blank(graph), rdf.Uri("a:pred"), rdf.New(123))
			}, new[] {quad});
		}
	}
}