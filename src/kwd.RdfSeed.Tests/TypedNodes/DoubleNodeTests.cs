using kwd.Rdf.Std;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.TypedNodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.TypedNodes
{
    [TestClass]
    public class DoubleNodeTests
    {
        [TestMethod]
        public void Ctor_WithPrecision()
        {
            var f = new NodeFactory(new DoubleNodeMap(2));
            var target = f.New(123.456);
            
            Assert.AreEqual(123.46, target.Value);
            Assert.AreEqual("123.46", target.ValueString);

            f = new NodeFactory(new DoubleNodeMap());

            target = f.New(.123456789);
            Assert.AreEqual("0.1235", target.ValueString, 
                "xml canonical has zero in front of point (if empty)");
        }

        [TestMethod]
        public void Ctor_FancyNumbers()
        {
            var f = new NodeFactory(new DoubleNodeMap());

            var target = (Node<double>) f.New("-0", XMLSchema.Double);

            Assert.IsTrue(double.IsNegative(target.Value));

            target = f.New(double.NegativeInfinity);
            Assert.IsTrue(double.IsNegativeInfinity(target.Value));
        }
    }
}
