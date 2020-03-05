using kwd.RdfSeed.Compare;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.TypedNodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Compare
{
	[TestClass]
	public class SameQuadTests
	{
		[TestMethod]
		public void SameQuad_()
		{
			var f = new NodeFactory();

			var q1 = new Quad(f.BlankGraph(), 
				f.Uri("x:/s"), f.Uri("x:/p"),f.Literal("a"));

			var q2 = new Quad(q1.Graph, q1.Subject, f.Uri("x:/p"), q1.Object);

			var target = new SameQuad();
			Assert.IsTrue(target.Equals(q1, q2));
		}
	}
}