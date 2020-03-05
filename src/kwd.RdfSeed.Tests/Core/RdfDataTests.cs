using System.Linq;

using kwd.RdfSeed.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Core
{
	[TestClass]
	public class RdfDataTests
	{
		[TestMethod]
		public void GraphIds_Ordered()
		{
			var rdf = new RdfData(new NodeFactory());

			var g1 = rdf.BlankGraph("test");
			rdf.Assert(g1, rdf.Blank(g1), rdf.Uri("a:pred"), rdf.New(123));

			var graphs = rdf.GraphIds;
			Assert.AreEqual(3, graphs.Length);
			Assert.AreEqual(rdf.System, graphs.ElementAt(0));
			Assert.AreEqual(rdf.Default, graphs.ElementAt(1));
		}
	}
}