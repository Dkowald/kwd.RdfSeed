using kwd.RdfSeed.Core.Nodes.Builtin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Core.Nodes
{
    [TestClass]
    public class NodeTests
    {
        [TestMethod]
        public void ValueHashMatchesOnSame()
        {
            var builder = new TypedLiteralMap("app:type");
            var n1 = builder.Create("A");
            var n2 = builder.Create("A");
            Assert.AreEqual(n1.ValueStringHash, n2.ValueStringHash);
        }
    }
}