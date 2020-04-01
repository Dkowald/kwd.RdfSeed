using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using kwd.CoreUtil.FileSystem;
using kwd.Rdf.Std;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Errors;
using kwd.RdfSeed.Serialize.NTriple;
using kwd.RdfSeed.Tests.TestHelpers;
using kwd.RdfSeed.TypedNodes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Core
{
	[TestClass]
    public class NodeFactoryTests
    {
        [TestMethod]
	    public void LoadSameData()
	    {
		    var f = new NodeFactory();
		    var rdf = new RdfData(new NodeFactoryThreadSafe(f));

		    var data = Files.TestDataDir.GetFile("brazil.nt");

		    var f1 = data.CopyTo(
			    Files.AppDataDir.GetFile(
				    nameof(NodeFactoryTests), "f1.nt"), true);
		    var g1 = rdf.GetBlankGraph("g1");

		    var f2 = data.CopyTo(
			    Files.AppDataDir.GetFile(
				    nameof(NodeFactoryTests), "f2.nt"), true);
		    var g2 = rdf.GetBlankGraph("g2");

		    new NTripleFile(f1).Read(g1).Wait();
		    
            var blankCount = rdf.Query.Where(x => x.Subject is BlankNode)
	            .Select(x => x.Subject)
	            .Union(rdf.Query.Where(x => x.Object is BlankNode)
	                    .Select(x => x.Object))
	            .Distinct().Count();
	    
		    var count1 = f.Stats().TotalNodes - blankCount;
		    new NTripleFile(f2).Read(g2).Wait();
		    var count2 = f.Stats().TotalNodes - blankCount*2;

            Assert.AreEqual(count1, count2, "No extra nodes, except blanks.");
	    }

        [TestMethod]
	    public void Stats_()
	    {
		    var rdf = RdfDataFactory.CreateDefault();

		    var g = rdf.GetBlankGraph();

            new NTripleFile(Files.TestData.Brazil)
                .Read(g).Wait();

		    var stats = rdf.Stats();

            Assert.AreEqual(80, stats.TotalNodes - stats.BlankNodes, 
	            "Total non blank nodes");

            Assert.AreEqual(1, stats.AutoNodes, "Graph is only auto blank");

            Assert.AreEqual(31 + 1, stats.BlankNodes,
	            "Total blank nodes (plus graph itself)");
	    }

        [TestMethod]
	    public void LoadFromMultipleThreads()
	    {
		    var rdf = RdfDataFactory.CreateDefault();
		    var data = Files.TestDataDir.GetFile("brazil.nt");

            var tasks = new[] {
                DelayedDataRead(CopyDataFile(data, 1), rdf, 1),
                DelayedDataRead(CopyDataFile(data, 1), rdf, 2),
                DelayedDataRead(CopyDataFile(data, 1), rdf, 3),
                DelayedDataRead(CopyDataFile(data, 1), rdf, 4),
            };

            //start in fast-loop so they run at same time.
            foreach (var item in tasks){item.Start();}

            Task.WaitAll(tasks);
            
            var g1 = rdf.GetSelfGraph(rdf.BlankSelf("g1"));
            var g2 = rdf.GetSelfGraph(rdf.BlankSelf("g2"));
            var g3 = rdf.GetSelfGraph(rdf.BlankSelf("g3"));
            var g4 = rdf.GetSelfGraph(rdf.BlankSelf("g4"));

            //138 quads per graph.
            Assert.AreEqual(138, g1.Query.Count, "Count for graph-1");
            Assert.AreEqual(138, g2.Query.Count, "Count for graph-2");
            Assert.AreEqual(138, g3.Query.Count, "Count for graph-3");
            Assert.AreEqual(138, g4.Query.Count, "Count for graph-4");
            
            //the nodes in doc, and extras for graphs.
            var expectedCount = 80 + //normal nodes.
                                32 * tasks.Length;//blanks from doc and graph.

            var stats = rdf.Stats();
            Assert.AreEqual(expectedCount, stats.TotalNodes, "no dupe nodes");
	    }

	    private FileInfo CopyDataFile(FileInfo srcFile, int idx)
	    {
		    var f = Files.AppDataDir
			    .GetFile(nameof(NodeFactoryTests), $"f{idx}.nt");
		    return srcFile.CopyTo(f, true);
	    }

	    private Task DelayedDataRead(FileInfo srcFile, IRdfData rdf, int id)
	    {
		    return new Task(() =>
		    {
			    var g = rdf.GetSelfGraph(rdf.BlankSelf($"g{id}"));
			    using var rd = srcFile.OpenText();
			    NTripleFile.Read(g, rd).Wait();

		    }, TaskCreationOptions.LongRunning);
	    }

        [TestMethod]
        public void UseBuiltinTypes()
        {
            var f = new NodeFactory();

            var s1 = f.New("aaa:aaa", UriOrBlankMap.ValueType) as Node<UriOrBlank>;

            Assert.IsNotNull(s1, "Expect it to be builtin subject node");
        }

        [TestMethod]
        public void ReuseCreatedValueType()
        {
            var intBuilder = new IntegerNodeMap();

            var f = new NodeFactory(intBuilder);

            var n1 = f.New("32", intBuilder.DataType);

            var n2 = f.New(32);
            
            Assert.AreEqual(n1, n2);
        }

        [TestMethod]
        public void ReuseObjectNode()
        {
            var f = new NodeFactory();

            var data = new DirectoryInfo("c:/temp/");

            var n1 = f.New(data);

            var n2 = f.New(data);

            Assert.IsTrue(n1 == n2, "same object gives same node");
        }

        [TestMethod]
        public void MappedTypedNodes()
        {
            var mapDouble = new DoubleNodeMap();
            var f = new NodeFactory(mapDouble);

            var n = f.New(13.5);
            
            Assert.AreEqual(XMLSchema.Double, n.ValueType.DataType);
            Assert.AreEqual(n.Value, 13.5);
        }

        [TestMethod]
        public void CustomTypeLiteralNode()
        {
            var f = new NodeFactory();

            var n = f.New("test:mytype", "test:XXX");
            Assert.AreEqual("test:mytype", n.ValueString);
            Assert.AreEqual("test:XXX", n.ValueType.DataType);

            var n2 = f.New("Val2", "test:XXX");
            Assert.AreEqual(n.ValueType, n2.ValueType,
                "Same value type string");

            var n3 = f.New("test:mytype", "test:XXX");

            Assert.AreEqual(n, n3, "same data, same node");
        }

        [TestMethod]
        public void TypedLiteralWithStringLiteral()
        {
            var f = new NodeFactory();

            var n1 = f.New("a literal");

            var n2 = f.New("a literal", "typed:other");

            Assert.AreNotEqual(n1, n2);
        }
        
        [TestMethod]
        public void Literals()  
        {
            var textNode = new Text.TextNodeMap();
            var f = new NodeFactory(textNode, new LiteralNodeMap());

            //no lang for normal node.
            var the = f.New("The");
            Assert.AreEqual("The", the.Value);
            Assert.AreEqual(XMLSchema.String, the.ValueType.DataType, "typed as a string");

            var la = f.Text("La", "fr");
            Assert.AreEqual("fr", la.Value.Language);
            Assert.AreEqual("La", la.Value.Value);
            Assert.AreEqual(Text.TextNodeMap.ValueType, la.ValueType.DataType);
        }

        [TestMethod]
        public void GraphNode()
        {
            var f = new NodeFactory();

            var g1 = f.BlankSelf();
            var g2 = f.BlankSelf("g2");

            Assert.IsTrue(g1.IsSelfScoped());
            Assert.IsTrue(g1.Label.StartsWith(NodeFactory.BlankGraphPrefix));

            Assert.IsTrue(g2.IsSelfScoped());
            Assert.IsTrue(g2.Label == "g2");
        }

        [TestMethod]
        public void GraphNodeReused()
        {
            var f = new NodeFactory();

            var n1 = f.BlankSelf("g1");

            var n2 = f.BlankSelf("g1");

            Assert.AreEqual(n1, n2);
        }

        [TestMethod]
        public void BlankNodes()
        {
            var f = new NodeFactory();
            var g = f.BlankSelf();
            var labeled = f.Blank(g, "anon");
            Assert.IsTrue(labeled.GetValueString().EndsWith("anon"), "Expect blank with label.");

            var auto = f.Blank(g);
            Assert.IsTrue(auto.GetValueString().Contains(NodeFactory.BlankNodePrefix),
                "Has standard prefix.");

            Assert.IsTrue(f.Blank(g, "_:xxA").GetValueString().EndsWith("xxA"), "Blank node with label");

            Assert.IsTrue(f.Blank(g, $"{NodeFactory.BlankNodePrefix}ABc")
                    .GetValueString().EndsWith("ABc"), "blank node with full prefix");
        }

        [TestMethod]
        public void BlankNode_ScopeToGraph()
        {
            var f = new NodeFactory();

            //blanks in a scope can be found via same label.
            // but different scope, with same label in not the same blank.

            var g1 = f.BlankSelf();
            var g2 = f.BlankSelf();

            var b1 = f.Blank(g1, "A");
            var b2 = f.Blank(g2, "A");
            Assert.IsFalse(b1 == b2);

            //same scope and label will match.
            var b3 = f.Blank(g1, "A");
            Assert.AreEqual(b1, b3);
        }

        [TestMethod]
        public void Blank_ReuseAuto()
        {
            var f = new NodeFactory();

            var g = f.BlankSelf();

            var n1 = f.Blank(g);
            var n2 = f.Blank(g, n1.Value.Label);

            Assert.AreEqual(n1, n2, "matching auto matches node");
        }

        [TestMethod]
        public void Blank_NewAutoNoCollide()
        {
            var f = new NodeFactory();
            var g = f.BlankSelf();

            var n1 = f.Blank(g, "auto#1");

            var n2 = f.Blank(g);

            Assert.IsFalse(n1 == n2, "Auto blank not same if label matches");
        }

        [TestMethod]
        public void AlternateCreateForUriNode()
        {
            var f = new NodeFactory();

            var n1 = f.New("xxx:uri", XMLSchema.AnyUri);

            var n2 = f.Uri("xxx:uri");

            Assert.IsTrue(n1 == n2, "Reuse same uri node.");

            Assert.ThrowsException<UriObjectNodeNotSupported>(() => f.New(new Uri("xxx:uri")));
        }
    }
}