using System;
using System.Linq;

using kwd.RdfSeed.Builder;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Query;
using kwd.RdfSeed.RdfModel;
using kwd.RdfSeed.Util;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Builder
{
    [TestClass]
    public class RdfBuilderTests
    {
        [TestMethod]
	    public void ReplaceValue()
	    {
            var rdf = new RdfData(new NodeFactory());
            var g = rdf.GetBlankGraph("test");
            g.Update
	            .For("app:test", out var test)
	            .With("app:name", out var aName).Add("fred");

            g.Update
	            .For(test).With(aName).Remove<string>();

            var nameNode = g.Query.For(test).With(aName)
	            .FirstOrNull();
            Assert.IsNull(nameNode);

            g.Update
	            .For(test).With(aName).Set("bob");

            var name = g.Query.For(g, "app:test")
	            .With(g, "app:name")
	            .First().Object.Value<string>();

            Assert.AreEqual("bob", name);
	    }

        [TestMethod]
        public void AddListAndKeepingReferenceToStart()
        {
            var rdf = new RdfData(new NodeFactory());
            
            var g = rdf.GetGraph(rdf.Uri("graph:/g1"))
	            .Update
                .Let(out var graphId);

            //(http://www.snee.com/bobdc.blog/2014/04/rdf-lists-and-sparql.html)
            // x:sub rdfs:first "one"
            // x.sub rdfs:rest _:1
            // _:1 rdfs:first "two"
            // _:1 rdfs:rest _:2
            // _:2 rdfs:first "three"
            // _:2 rdfs:rest rsf:nil
            g.For(rdf.Uri("x:sub"))
                .Let(out var listStart)
                .List(rdf.New("one"), rdf.New("two"), rdf.New("three"))
                .With(rdf.Uri("x:p")).Add(rdf.New("Extra data on last item"))
                //And jump back to starting subject.
                .ThenFor(listStart);

            var listModel = new RdfsList(rdf, rdf.GetGraph(graphId));

            var items = listModel.GetList(listStart)
	            .ToList();

            Assert.AreEqual(3, items.Count, "Got 3 items from list");
            Assert.AreEqual("two", items[1].Value<string>(), "2nd is as expected");
        }

        [TestMethod]
        public void AddWithUpDownAndLet()
        {
            var f = new NodeFactory();
            var rdf = new RdfData(f);

            var gId = f.BlankSelf("g1");
            var g = rdf.GetGraph(gId).Update;

            // _:1 x:p1 "1".
            g.ForBlank("1", out _)
                .With(f.Uri("x:p1"))
                .Add(f.New("1"))
                // _:1 x:p2 "123"
                .Then().With(f.Uri("x:p2")).Add(f.New("123"))
                // _:1 x:p3 _:2 
                // _:2 x:p3 "fred" 
                .Then().With(f.Uri("x:p3")).To(f.Blank(gId))
                .With(f.Uri("x:p3")).Add(f.New("fred"))
                //get ref to _:2
                .Then().Let(out _);
        }

        [TestMethod]
        public void BuildingAChain()
        {
            var rdf = new RdfData(new NodeFactory());
            
            var g = rdf.GetBlankGraph().Update
                .Let(out var graphId);

            // x:joe x:phone _:auto1
            // _:auto1 x:home "a"
            // _:auto1 x:home "123"^^int
            g.For(rdf.Uri("x:joe"))
	            .With(rdf.Uri("x:phone"))
                .To(rdf.Blank(graphId))
                .With(rdf.Uri("x:home"))
                .Add(rdf.New("123"), rdf.New(123));

            Assert.AreEqual(3, rdf.Query.Count);

            //now query data.
            var graph = rdf.Query.From(graphId).ToList();
            var linkToData = graph.LinksFor(rdf.Uri("x:joe")).Single();
            
            Assert.AreEqual(rdf.Uri("x:phone"), linkToData.Predicate, "Got Joe's phone entries");

            var properties = graph.PropertiesFor((Node<UriOrBlank>)linkToData.Object).ToList();
            Assert.AreEqual(2, properties.Count, "Expected 2 entries");

            var thePhoneNumber = properties.Select(x => x.Object.As<int>())
                .Single(x => !(x is null)) 
                ?? throw new Exception("Didn't find phone number");

            Assert.AreEqual(123, thePhoneNumber.Value);
        }
    }
}