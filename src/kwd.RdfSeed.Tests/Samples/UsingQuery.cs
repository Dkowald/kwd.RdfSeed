using System.Linq;

using kwd.Rdf.Std;
using kwd.RdfSeed.Builder;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Query;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Samples
{
	[TestClass]
	public class UsingQuery
	{
		private Node<UriOrBlank> LoadSampleData(IRdfData data, Node<UriOrBlank> g)
		{
			var me = data.Uri("x:me");
			var link = data.Uri("a:linkTo");
			var name = data.Uri("a:name");

			data.Update.From(g)
				.For(me)
				.With(link)
				.Add(data.Uri("http://my.blog"))
				.Then()
				.With(name).Add(data.New("My Name"));

			return g;
		}

		[TestMethod]
		public void QueryMatchesUpdateForm()
		{
			var rdf = RdfDataFactory.CreateDefault();

			var g = rdf.BlankSelf();
			var subject = rdf.Blank(g, "test");
			var predicate = rdf.Uri(RDF.A);
			var value = rdf.Uri("test:query#basic");

			//add some data
			rdf.Update
				.From(g)
				.For(subject)
				.With(predicate)
				.Add(value);

			//query some data.
			var qry = rdf.Query;
			var uriValue = qry
				.From(g)
				.For(subject)
				.With(predicate)
				.GetUri(out var result)//inline get with result
				.GetUri();//get with select.

			Assert.AreEqual(value.Uri, result.Single(), "from inline select");
			Assert.AreEqual(value.Uri, uriValue.Single(), "from query select");
		}

		[TestMethod]
		public void QueryWithConverters()
		{
			var rdf = RdfDataFactory.CreateNoLock();
			var g = LoadSampleData(rdf, rdf.BlankSelf());

			//Read all linkTo URI's.
			var linkedUris = rdf.Query
				.From(g)
				.For(rdf, "x:me")
				.With(rdf, "a:linkTo")
				.GetUri()
				.ToList();

			Assert.IsTrue(linkedUris.Any(), "found some links");
		}

		[TestMethod]
		public void LinksInMyGraph()
		{
			var rdf = RdfDataFactory.CreateDefault();
			var myGraph = rdf.Uri("x:myGraph");
			
			LoadSampleData(rdf, myGraph);
			
			var linkedTo = rdf.Uri("a:linkTo");

			//all quads in myGraph, with linkedTo predicate
			var linkedUris = rdf.Query
				.From(myGraph)
				.With(linkedTo)
				//select the quad object nodes that are URI's
				.GetUri()
				.ToList();

			Assert.IsTrue(linkedUris.Any(), "found some links");
		}

		[TestMethod]
		public void LinksAndNamedInMyGraph()
		{
			var rdf = RdfDataFactory.CreateDefault();
			var myGraph = rdf.Uri("x:myGraph");
			
			LoadSampleData(rdf, myGraph);
			
			var linkedTo = rdf.Uri("a:linkTo");
			var name = rdf.Uri("a:name");
			
			//Select all links and names within graph g.
			var linksAndNames = rdf.Query
				.From(myGraph)
				.With(p => p == linkedTo || p == name)
				.Select(x => x.Object)
				.ToList();

			Assert.IsTrue(linksAndNames.Any(x => x is Node<string>), "Has literals");
			Assert.IsTrue(linksAndNames.Any(x => x is UriNode), "Has urls");
		}

		[TestMethod]
		public void AddLinkInGraph()
		{
			var rdf = RdfDataFactory.CreateDefault();

			var myGraph = rdf.Uri("g:other");
			var linkTo = rdf.Uri("a:linkTo");
			
			//link to other graph: using assert
			var g = rdf.BlankSelf();
			rdf.Assert(g, g, linkTo, myGraph);

			//link to other graph: using fluent
			rdf.Update
				.From(rdf.BlankSelf())//a graph
				.Let(out var demo)//inline graph.
				.For(demo)//subject is demo
				.With(linkTo)//predicate is linkTo
				.Add(myGraph);//Assert 

			var links = rdf.Query.With(linkTo).ToList();

			Assert.IsTrue(links.Any(), "Have a link");
		}

		[TestMethod]
		public void UseHelpersWithBuilders()
		{
			var rdf = RdfDataFactory.CreateNoLock();

			//Use let to collect generated node(s)
			rdf.Update
				.From("g:aGraph", out var aGraph)
				.For("x:me", out var me)
				.With("a:name", out _).Let(out var name)
				.Add("Fred")
				.Then().Then().Then()
				.From(aGraph)
				.For(me).With(name)
				.Add("Fredo", "fr", out var frName);

			Assert.IsNotNull(aGraph);
			Assert.IsNotNull(me);
			Assert.IsNotNull(name);
			Assert.IsNotNull(frName);
		}

		[TestMethod]
		public void ConverterWithNormalization()
		{
			var rdf = RdfDataFactory.CreateNoLock();

			rdf.Update
				.FromBlank(out var g1)
				.For("x:me ", out _)
				.With("a:Name", out _)
				.Add("Fred");

			var aboutMe = rdf.Query.From(g1)
				.For(rdf, "x:me")
				.ToList();

			Assert.AreEqual(1, aboutMe.Count, "Found entry");
		}
		
	}
}
