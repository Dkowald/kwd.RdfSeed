using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using kwd.CoreUtil.FileSystem;
using kwd.RdfSeed.Compare;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Query;
using kwd.RdfSeed.Serialize.NTriple;
using kwd.RdfSeed.Tests.TestHelpers;
using kwd.RdfSeed.TypedNodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Serialize.NTriple
{
    [TestClass]
    public class NTripleFileTests
    {
	    [TestMethod]
        public void ReadData()
        {
	        var rdf = new RdfData(new NodeFactory());

            var target = new NTripleFile(Files.TestData.Sample1);

            var defaultGraph = rdf.GetBlankGraph("default");
            target.Read(defaultGraph).Wait();
            
            var quad = rdf.Query.With(rdf, "app:blank")
                .Single();
            Assert.AreEqual(quad.Subject, quad.Object);
            Assert.AreEqual("b1", quad.ValueBlank());
            
            var txt = rdf.Query.With(rdf, "app:literal")
                .Single().Value<string>();
            Assert.AreEqual("x", txt);

            var uriNode= rdf.Query.With(rdf, "app:uri")
                .Select(x => x.Object)
                .OfType<UriNode>().Single();

            txt = uriNode.Uri;

            Assert.AreEqual("http://a.example/s", txt);

            txt = rdf.Query.With(rdf, "app:text")
                .Select(x => x.Object)
                .OfType<Node<Text>>()
                .Single(x => x.Value.Language == "fr")
                .Value.Value;
            Assert.AreEqual("fredo", txt);

            var intValue = rdf.Query.With(rdf, "app:number")
                .Single().Value<int>();
            Assert.AreEqual(123, intValue);
        }

        [TestMethod]
        public void WriteTripleData()
        {
            var rdf = new RdfData(new NodeFactory());

            var target = new NTripleFile(Files.AppDataDir.GetFile(
                nameof(NTripleFileTests), nameof(WriteTripleData)+".nt"));

            var g = rdf.BlankGraph("g1");

            rdf.Assert(g, 
                rdf.Uri("x:sub1"),
                rdf.Uri("x:age"),
                rdf.New(23));

            target.Write(rdf.GetGraph(g)).Wait();
        }

        [TestMethod]
        public async Task WriteRead()
        {
            var file = new FileInfo(
                Path.Combine(Files.AppData.NqFileTests.FullName, 
                    nameof(WriteRead) + ".nt"));

            var rdf = new RdfData(new NodeFactory());

            var g = rdf.GetFullGraph("graph:1")
	            .Assert(rdf.Uri("sub1:/"), rdf.Uri("pred1:/"), rdf.Literal("basic text"));
            
            file.Directory.EnsureDelete();
            var t = new NTripleFile(file);
            await t.Write(g);

            file.Refresh();
            Assert.IsTrue(file.Exists);
            Assert.IsTrue(file.Length > 0, "wrote some data");

            //copy current so it doesn't delete.
            var gCopy = g.Query.ToList();

            g.Clear();
            await t.Read(g);

            IEqualityComparer<Quad> eq = new SameQuad();
            
            var gDiffs = g.Query.Except(gCopy, eq).ToList();

            Assert.IsFalse(gDiffs.Any(), "No difference");
        }
    }
}