using System;
using System.IO;

using kwd.Rdf.Std;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Errors;
using kwd.RdfSeed.Tests.Samples.Rdf;
using kwd.RdfSeed.TypedNodes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Samples
{
	/// <summary>
	/// Demonstration of node creation and re-use.
	/// </summary>
	[TestClass]
	public class NodeReuse
	{
		[TestMethod]
		public void CreateUri()
		{
			var rdf = RdfDataFactory.CreateNoLock();

			var search = new Uri("https://duckduckgo.com/");

			//cannot create a uri node as object.
			Assert.ThrowsException<UriObjectNodeNotSupported>(() => rdf.New(search));

			//but can create with explicit extension.
			var n1 = rdf.Uri(search);
		}

		[TestMethod]
		public void MatchForTypedLiteral()
		{
			var f = new NodeFactory();

			var n1 = f.New("a:value", "x:type");
			var n2 = f.New("a:value", "x:type");

			Assert.IsTrue(ReferenceEquals(n1, n2), 
				"Same string data gives same node");
		}

		[TestMethod]
		public void MatchWithMap()
		{
			//Factory with map for integers.
			var f = new NodeFactory(new IntegerNodeMap());

			var n1 = f.New("001", XMLSchema.Integer);
			var n2 = f.New(1);
			Assert.IsTrue(ReferenceEquals(n1, n2), 
				"Map normalizes value, so node is reused");

			//uri is builtin
			var n3 = f.New("a:uri", XMLSchema.AnyUri);
			var n4 = f.Uri("a:uri");
			
			Assert.IsTrue(ReferenceEquals(n3, n4),
				"Builtin uri node matches when created directly or via typed literal");

			Assert.ThrowsException<UriObjectNodeNotSupported>(
				() => f.New(new Uri("xxx:path")),
				"System.Uri not usable as object node to avoid unexpected " +
				"node mis-match");
		}

		[TestMethod]
		public void MatchForObjects()
		{
			var f = new NodeFactory();

			var n1 = f.New(new FileInfo("c:/temp"));
			var n2 = f.New(new FileInfo("c:/temp"));
			Assert.IsFalse(ReferenceEquals(n1, n2),
				"Different instances are different nodes");

			var o = new FileInfo("c:/tmp");

			var n3 = f.New(o);
			var n4 = f.New(o);
			Assert.IsTrue(ReferenceEquals(n3, n4), 
				"Same instances same node");
		}

		[TestMethod]
		public void MatchForCustomNodeMap()
		{
			var f = new NodeFactory(new FileInfoNodeMap());

			var file = new FileInfo("c:/tmp/test.txt");
            
			var n1 = f.New(file);

			var n2 = f.New(file.FullName, FileInfoNodeMap.TypeString);

			//case ignored for for the FileInfoNodeMap.
			var n3 = f.New("c:/TMP/Test.txt", FileInfoNodeMap.TypeString);

			Assert.IsTrue(ReferenceEquals(n1, n2), 
				"Mapping uses ValueType to reuse node with same logical value");

            Assert.IsTrue(ReferenceEquals(n1, n3),
	            "Mapping normalizes value string, so logically same nodes match");
		}
	}
}