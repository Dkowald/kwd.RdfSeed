using kwd.RdfSeed.Core;
using kwd.RdfSeed.Errors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Core.Nodes
{
	[TestClass]
    public class SubjectNodeTests
    {
        [TestMethod]
        public void SubjectCanBeUriOrBlank()
        {
            var f = new NodeFactory();
            var g = f.BlankSelf();
            var s1 = f.Blank(g, "_:node1");
            Assert.AreEqual("node1", s1.Value.Label);
            
            var s2 = f.Blank(g);
            Assert.IsNotNull(s2.Value.Label, "generates new label");

            var s3 = f.Uri("x:/someone#here");
            Assert.IsNull(s3.Value.Label);
            Assert.IsNull(s3.Value.Scope);

            Assert.ThrowsException<InvalidUri>(() => f.Uri("23:/path"));
        }
    }
}