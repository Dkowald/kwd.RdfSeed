using System.Linq;

using kwd.RdfSeed.Builder;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Query;
using kwd.RdfSeed.Util;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Core
{
	[TestClass]
    public class GraphTests
    {
	    [TestMethod]
	    public void QueryIncludesOtherGraphs()
	    {
		    var rdf = new RdfData(new NodeFactory());
			
		    var g1 = rdf.GetGraph(rdf.BlankSelf());
		    g1.Assert(rdf.Uri("app:test"), rdf.Uri("app:name"),
			    rdf.New("baseName"));

		    var g2 = rdf.GetGraph(rdf.BlankSelf());
		    g2.Assert(rdf.Uri("app:test"),rdf.Uri("app:name"),
				rdf.New("mainName"));

		    var target = rdf.GetGraph(g2.Id, g1.Id);

		    var names = target.Query.With(rdf, "app:name").ToArray();
		    Assert.AreEqual(2, names.Length, "Finds both entries");

			//Get the name value.
			var name = names
				.Get<string>()
				.FirstOrNull();
			    
		    Assert.AreEqual("mainName", name, "Main graph is first in the query");
	    }

        [TestMethod]
	    public void HasCrossGraphBlank()
	    {
		    var rdf = new RdfData(new NodeFactory());

		    var g1 = rdf.GetGraph(rdf.BlankSelf());
		    var g2 = rdf.GetFullGraph("app:test");

		    var who = rdf.Uri("app:who");
		    var b1 = g1.Blank("b1");

            //b1 in g1 points to blank in g2 for details.
		    g1.Assert(b1, who, g2.Id);
            
            //b1 has name fred.
            var name = rdf.Uri("app:name");
		    g2.Assert(b1, name, rdf.New("Fred"));

            //Now find via cross graph query.
            var otherG = g1.Query.Single(x => 
	            x.Predicate == who &&
                x.Object is Node<UriOrBlank>)
	            .Object.Cast<UriOrBlank>();

            var myName = rdf.GetGraph(otherG).Query.Where(x =>
		            x.Subject == b1 &&
		            x.Predicate == name)
	            .Get<string>()
	            .Single();

            Assert.AreEqual("Fred", myName);
	    }

        [TestMethod]
        public void ClearGraph()
        {
            var data = new RdfData(new NodeFactory());

            data.Update.From("sys:/data/", out _)
	            .For("sub:s1", out _)
	            .With("x:/p1", out _).Add(123);

            var g = data.GetGraph(data.BlankSelf())
	            .Assert(data.Uri("x:/s1"),
		            data.Uri("p:/1"),
		            data.New("text"));

            Assert.AreEqual(2, data.Query.Count, "have the nodes");

            g.Clear();
            Assert.AreEqual(0, g.Query.Count, "no graph nodes");
            Assert.AreEqual(1, data.Query.Count, "still have other data");
        }

        [TestMethod]
        public void RdfUpdate_AvailableInGraph()
        {
            var f = new NodeFactory();

            var rdf = new RdfData(f);

            var g1 = f.BlankSelf();
            var g2 = f.BlankSelf();

            var graphPreUpdates = rdf.GetGraph(g1, g2);
			
			//add directly via rdf data.
			var sub = rdf.Uri("x:/s1");
			var pred = rdf.Uri("x:/p1");
			var val = rdf.New("text");
            rdf.Assert(g1, sub, pred, val);
            rdf.Assert(g2, sub, pred, val);

			//immediately have the owned quad,
			//but not the data from other graph. 
			Assert.AreEqual(1, graphPreUpdates.Query.Count, "Got the new entry");

			var graphReInit = rdf.GetGraph(g1, g2);
			Assert.AreEqual(2, graphReInit.Query.Count, "Re-access graph gets latest");
        }
    }
}