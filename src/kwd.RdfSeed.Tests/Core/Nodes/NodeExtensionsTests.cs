using System;
using kwd.Rdf.Std;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.TypedNodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Core.Nodes
{
    [TestClass]
    public class NodeExtensionsTests
    {
        [TestMethod]
        public void As_()
        {
            var f = new NodeFactory(new DoubleNodeMap());

            var node = f.New(123.45);

            Assert.IsNull(node.As<int>(), "Its not a int");
            
            Assert.AreEqual(123.45, node.As<double>()?.Value);

            var intVal = node.As<int>()?.Value;
            Assert.IsFalse(intVal.HasValue);
        }

        [TestMethod]
        public void Cast_()
        {
            var f = new NodeFactory(new IntegerNodeMap());

            var node = f.New("123", XMLSchema.Integer);

            Assert.AreEqual(123, node.Cast<int>().Value);

            Assert.ThrowsException<InvalidCastException>(() => node.Cast<double>());
        }
    }
}