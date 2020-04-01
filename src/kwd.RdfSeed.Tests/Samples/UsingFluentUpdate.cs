using System.Linq;

using kwd.RdfSeed.Builder;
using kwd.RdfSeed.Query;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Samples
{
	public class UsingFluentUpdate
	{
		[TestMethod]
		public void AssertVsFluent()
		{
			//Assert vs Fluent
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
		public void FluentWithNodeCreator()
		{
			//Fluent builder with node creation
			var rdf = RdfDataFactory.CreateNoLock();

			rdf.Update
				.From("g:aGraph", out var aGraph)
				.For("x:me", out var me)
				.With("a:name", out var name)
				.Add("Fred", out var myName)
				.Then()
				//ignore created node
				.With("a:age", out _)
				.Add(123, out _);

			Assert.AreEqual("g:aGraph", aGraph.Uri);
			Assert.AreEqual("x:me", me.Uri);
			Assert.AreEqual("a:name", name.Value);
			Assert.AreEqual("Fred", myName.Value);
		}
	}
}