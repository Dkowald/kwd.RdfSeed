using kwd.RdfSeed.Compare;
using kwd.RdfSeed.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Compare
{
	[TestClass]
	public class SameTripleTests
	{
		[TestMethod]
		public void SameTriple_()
		{
			var f = new NodeFactory();

			var t1 = new Quad(f.BlankSelf("g1"), 
				f.Uri("x:s1"), f.Uri("x:/p1"), 
				f.New("0", "xs:int"));

			var t2 = new Quad(f.BlankSelf("g2"),
				t1.Subject, t1.Predicate, t1.Object);

			var target = new SameTriple();
            
			Assert.IsTrue(target.Equals(t1, t2), 
				"Different graph and extra data, but same triple");

			var t3 = new Quad(t1.Graph, t1.Subject, f.Uri("x:/other"),t1.Object);
			
			Assert.IsFalse(target.Equals(t1, t3), "Differ by predicate");
		}
	}
}
