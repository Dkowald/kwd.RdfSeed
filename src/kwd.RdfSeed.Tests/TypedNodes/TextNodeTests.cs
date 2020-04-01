using kwd.RdfSeed.Core;
using kwd.RdfSeed.TypedNodes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.TypedNodes
{
    [TestClass]
    public class TextNodeTests
    {
        [TestMethod]
	    public void LiteralMapsToString()
	    {
		    var f = new NodeFactory(new LiteralNodeMap());

		    var node = f.New("A literal ");

            Assert.IsTrue(node.ValueType is LiteralNodeMap);
	    }

        [TestMethod]
        public void ReuseSame()
        {
            var f = new NodeFactory(new Text.TextNodeMap());

            var n1 = f.Text("a", "en");

            var n2 = f.Text("a", "en");

            Assert.AreEqual(n1, n2, "reuse same node");
        }

        [TestMethod]
        public void DifferentAreNotReused()
        {
            var f = new NodeFactory(new Text.TextNodeMap());

            var n1 = f.Text("the", "en");
            var n2 = f.Text("other", "en");
            var n3 = f.Text("the", "en-us");

            Assert.IsFalse(ReferenceEquals(n1, n2), "diff text");
            Assert.IsFalse(ReferenceEquals(n2, n3), "diff");
            Assert.IsFalse(ReferenceEquals(n1, n3), "diff lang");
        }
    }
}