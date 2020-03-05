using System.Linq;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Serialize.NTriple;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Serialize.NTriple
{
    [TestClass]
    public class NTripleParseTests
    {
        [TestMethod]
	    public void Parse_LiteralWithDifferentEncoding()
	    {
            var rdf = new RdfData(new NodeFactory());

            var g = rdf.GetBlankGraph("test");

            var aHex = "\\u"+ ((int)'a').ToString("X4");

		    var data = "<a:s1> <a:p> \"a literal\"."+
			    $"\r\n <a:s2> <a:p> \"a liter{aHex}l\".";

			var target = new NTripleParse(g);
            
            target.Load(data);
            var q1 = rdf.Query.ElementAt(0);
            var q2 = rdf.Query.ElementAt(1);

            Assert.AreEqual(q1.Object, q2.Object, "Same literal node");
	    }

        [TestMethod]
        public void ReadMultiLine()
        {
            var data = "Multi \\n Line text \\\" with spaces\\u0021.";
            var expected = "Multi \n Line text \" with spaces!.";

            var f = new NodeFactory();
            var rdf = new RdfData(f);

            var target = new NTripleParse(rdf.GetBlankGraph());
            var quad = "<a:s> <a:p> \"" + data + "\" .";

            target.Load(quad);

            var result = rdf.Query.Single().Value<string>();

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ReadBlanks()
        {
            var f = new NodeFactory();
            var rdf = new RdfData(f);

            var g = rdf.GetBlankGraph();

            var target = new NTripleParse(g);

            target.Load("<s:1> <p:1> _:blank . ");

            var label = g.Query.Single().ValueBlank();
            Assert.AreEqual("blank", label);
        }

        [TestMethod]
        public void Parse()
        {
            var data = "<x:/s1> <x:/p1> \"Blurb\" .";

            var store = new RdfData(new NodeFactory());
            var g = store.GetFullGraph("x:g1");

            var token = NTripleTokenizer.NextToken(data);

            var parse = new NTripleParse(store.GetFullGraph("x:g1"));

            while (!token.IsEnd)
            {
                parse.Next(token);
                token = NTripleTokenizer.NextToken(token.Rest);
            }

            Assert.AreEqual(1, g.Query.Count, "Loaded a triple");
        }
    }
}