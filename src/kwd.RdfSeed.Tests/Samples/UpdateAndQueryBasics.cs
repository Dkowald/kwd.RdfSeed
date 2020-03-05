using System.Linq;

using kwd.RdfSeed.Builder;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Query;
using kwd.RdfSeed.TypedNodes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Samples
{
	[TestClass]
	public class UpdateAndQueryBasics
	{
		public Node<UriOrBlank> LoadSampleData(IRdfData data, Node<UriOrBlank> g)
		{
			var me = data.Uri("x:me");
			var link = data.Uri("a:linkTo");
			var name = data.Uri("a:name");

			data.Update.From(g)
				.For(me)
				.With(link)
				.Add(data.Uri("http://my.blog"))
				.Then()
				.With(name).Add(data.Literal("My Name"));

			return g;
		}

		[TestMethod]
		public void FluentUpdates()
		{
			var rdf = RdfDataFactory.CreateNoLock();

		}

		[TestMethod]
		public void QueryWithConverters()
		{
			var rdf = RdfDataFactory.CreateNoLock();
			var g = LoadSampleData(rdf, rdf.BlankGraph());

			var linkedUris = rdf.Query
				.From(g)
				.For(rdf, "x:me")
				.With(rdf, "a:linkTo")
				.SelectUris()
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

			var linkedUris = rdf.Query
				.From(myGraph)
				.With(linkedTo)
				.SelectUris()
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

			//link g to other graph.
			rdf.Update.From(rdf.BlankGraph())
				.Let(out var demo)
				.For(demo).With(linkTo)
				.Add(myGraph);

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
				.Assert("Fred")
				.Then().Then().Then()
				.From(aGraph)
				.For(me).With(name)
				.Assert("Fredo", "fr", out var frName);

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
				.Assert("Fred");

			var aboutMe = rdf.Query.From(g1)
				.For(rdf, "x:me")
				.ToList();

			Assert.AreEqual(1, aboutMe.Count, "Found entry");
		}
		
	}
}
