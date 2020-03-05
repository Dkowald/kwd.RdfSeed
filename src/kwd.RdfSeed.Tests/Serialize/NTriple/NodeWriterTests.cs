using System.IO;
using System.Threading.Tasks;
using kwd.CoreUtil.FileSystem;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Serialize.Errors;
using kwd.RdfSeed.Serialize.NTriple;
using kwd.RdfSeed.Tests.TestHelpers;
using kwd.RdfSeed.TypedNodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Serialize.NTriple
{
    [TestClass]
    public class NodeWriterTests
    {
	    [TestMethod]
	    public async Task WriteWithCrossGraphBlank()
	    {
		    var rdf = new RdfData(new NodeFactory());
		    
		    var g1 = rdf.GetBlankGraph();
		    g1.Assert(rdf.Uri("bad:link"), rdf.Uri("app:test"), rdf.BlankGraph("other"));

		    var file = Files.AppDataDir.GetFile(nameof(NodeWriterTests),
			    nameof(WriteWithCrossGraphBlank) + ".nt")
			    .EnsureDelete();

		    var target = new NTripleFile(file);

		    try
		    {
			    await target.Write(g1);
			    Assert.Fail("Cannot write if blank is scoped to other graph");
		    }catch(BlankNodeScopedToInvalidGraph){}
	    }

        [TestMethod]
        public void PrintLiteral()
        {
            var rdf = new RdfData(new NodeFactory());
            var g = rdf.GetBlankGraph();

            var expected = "_:g <app:test> \"Literal\\rMultiline\\\\\" .";

            g.Assert(g.Blank("g"),
                rdf.Uri("app:test"), 
                rdf.Literal("Literal\rMultiline\\"));

            var target = new NodeWriter(g);

            using var wr = new StringWriter();

            target.Print(wr);

            wr.Flush();
            var result = wr.GetStringBuilder().ToString().Trim();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void PrintText()
        {
            var f = new NodeFactory();
            var rdf = new RdfData(f);
            var g = rdf.GetBlankGraph();

            var expected = "_:g <app:test> \"Literal\\rMultiline\\\\\"@ge .";

            g.Assert(g.Blank("g"),
                f.Uri("app:test"), 
                f.Text("Literal\rMultiline\\", "@ge"));

            var target = new NodeWriter(g);

            using (var wr = new StringWriter())
            {
                target.Print(wr);

                wr.Flush();
                var result = wr.GetStringBuilder()
                    .ToString().Trim();
                Assert.AreEqual(expected, result);
            }
            
        }
    }
}