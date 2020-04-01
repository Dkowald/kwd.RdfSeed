using System.Linq;

using kwd.Rdf.Std;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Query;
using kwd.RdfSeed.Util;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Samples
{
	[TestClass]
	public class UsingGraph
	{
		[TestMethod]
		public void CreateFromRdfData()
		{
			//Create new Full graph
			var rdf = RdfDataFactory.CreateNoLock();

			//Full graph with new labeled blank node
			var myGraph = rdf.GetBlankGraph("test");

			//A new graph; after I got my first graph.
			var subject = rdf.Uri("test:sub");
			rdf.Assert(rdf.Uri("test:g1"), 
				subject, rdf.Uri("test:int"), rdf.New(123));

			var quadFromOther = myGraph.Query
				.FirstOrNull(x => x.Subject == subject);

			Assert.IsNull(quadFromOther);
		}

		[TestMethod]
		public void ConvertToSelfGraph()
		{
			//Convert from mixed to self only graph.
			var rdf = RdfDataFactory.CreateNoLock();

			//Mixed graph has main and some other graphs.
			var myGraph = rdf.GetGraph(
				rdf.BlankSelf("MainGraph"),
				rdf.BlankSelf("Other1"),
				rdf.BlankSelf("Other2"));

			//Self only graph.
			myGraph = myGraph.SelfOnly();

			Assert.IsFalse(myGraph.Other.Any());

			//Or directly get self-only from rdf.
			myGraph = rdf.GetSelfGraph(rdf.BlankSelf("MainGraph"));
			Assert.IsFalse(myGraph.Other.Any());
		}

		[TestMethod]
		public void AddDataToGraph()
		{
			//Assert some data for a graph.
			var rdf = RdfDataFactory.CreateNoLock();

			//My base graph.
			var baseGraph = rdf.GetFullGraph("app:base");

			//The inbuilt system graph
			var sys = rdf.GetSystem();

			//Assert some data.
			sys.Assert(baseGraph.Id, sys.Uri(RDF.A), sys.Uri("app:graphId"));
		}

		[TestMethod]
		public void ReadFromStart()
		{
			//Graph data ordered.
			var rdf = RdfDataFactory.CreateNoLock();

			var g1 = rdf.GetBlankGraph("g1");
			var g2 = rdf.GetBlankGraph("g2");

			var s = rdf.Uri("app:test");
			var p = rdf.Uri("app:count");

			var name = rdf.Uri(RDFS.Label);

			g1.Assert(s, p, g1.New(1))
			  .Assert(s, name, g1.New("OriginalName"), out var qName);

			g2.Assert(s, p, g2.New(2));

			//Get Full data
			g2 = rdf.GetFullGraph(g2.Id);

			var theCount = g2.Query
				.For(s).With(p)
				.Get<int>().First();

			Assert.AreEqual(2, theCount);

			//isolated updates.
			
			//changing dependent graph g1:
			g1.Retract(qName);
			g1.Assert(s, name, g1.New("Updated"));

			//pre-existing g2 uses original g1 data.
			var theName = g2.Query
				.For(s).With(name).Get<string>().First();

			Assert.AreEqual("OriginalName", theName);
		}

		[TestMethod]
		public void IsolatedFromOtherGraphs()
		{
			var rdf = RdfDataFactory.CreateDefault();
			
			//Get 2 graphs.
			var g1 = rdf.GetFullGraph(rdf.BlankSelf("graph1"));
			var g2 = rdf.GetFullGraph(rdf.BlankSelf("graph2"));

			//update g2.
			g2.Assert(g2.Uri("app:me"), g2.Uri("app:update"), g2.New(true));

			//graph1 isolated from change in graph2
			Assert.AreEqual(0, g1.Query.Count,
				"Update in graph2 not reflected in graph1 snapshot");

			//graph2 has the update ready to use.
			Assert.AreEqual(1, g2.Query.Count, 
				"Update in graph 2 is available");
		}
	}
}