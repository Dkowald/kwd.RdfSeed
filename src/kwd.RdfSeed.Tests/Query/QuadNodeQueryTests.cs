using System.Linq;

using kwd.RdfSeed.Builder;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Query;
using kwd.RdfSeed.TypedNodes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Query
{
	[TestClass]
	public class QuadNodeQueryTests
	{
		[TestMethod]
		public void FindWithOr()
		{
			var rdf = new RdfData(new NodeFactory());

			var g = rdf.GetBlankGraph();
			g.Update
				.For("app:test", out var test)
				.With("app:name", out var aName)
				.Add("fred")
				.Add("fred", "en");

			g.Query.For(test)
				.With(aName)
				.IsType(n => n is Node<Text> || n is Node<string>);
		}

		[TestMethod]
		public void FindForValue()
		{
			var rdf = new RdfData(new NodeFactory());

			var g = rdf.GetBlankGraph();
			var sub = rdf.Uri("app:me");
			var name = rdf.Uri("app:name");

			g.Update
				.For(sub)
				.With(name).Add("fred", "en", out _)
				.Add("fred-literal", out _)
				.Then()
				.With("app:age", out _).Add(23, out _)
				.Then().Then()
				.For("app:other", out _)
				.With("app:friend", out _).Add(sub);

			//look for name as text.
			var found = rdf.Query.From(g.Id)
				.For(sub).With(name)
				.IsType<Text>()
				.Get<Text>()
				.Single();
			Assert.AreEqual("fred", found.Value);
		}
	}
}