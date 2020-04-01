using System;
using System.IO;

using kwd.Rdf.Std;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
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
	public class UsingNodeFactory
	{
		[TestMethod]
		public void UsingObjectNodeMap()
		{
			var f = new NodeFactory();

			//A node from a native object.
			var n1 = f.New(new FileInfo("c:/temp"));
			Assert.IsNull(n1.ValueString, "ObjectNode has no value string");

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
		public void UsingTypedLiteral()
		{
			var f = new NodeFactory();
			
			//Value is a string 
			var n1 = (Node<string>) f.New("a:value", "x:type");
			Assert.AreEqual("a:value", n1.Value);

			//Same input, same node
			var n2 = f.New("a:value", "x:type");
			Assert.IsTrue(ReferenceEquals(n1, n2));

			//Maps to inbuilt for Uri
			var uri = (UriNode)f.New("a:uri", XMLSchema.AnyUri);
			Assert.AreEqual("a:uri", uri.Uri);
		}

		[TestMethod]
		public void UsingBlankOrUri()
		{
			var f = new NodeFactory();

			//a uri
			var uri = f.Uri("a:uri");

			//a blank
			var blank = f.Blank(uri, "label");

			//blank is scoped to uri
			Assert.AreEqual(uri, blank.Scope);

			//cannot create a uri node as object.
			var uri2 = new Uri("https://duckduckgo.com/");
			Assert.ThrowsException<UriObjectNodeNotSupported>(() => f.New(uri2));
		}

		[TestMethod]
		public void UsingPreBuiltTypes()
		{
			var f = new NodeFactory(TypedNodesHelpers.AllNodeMappings());

			//create bool from .net type
			var boolNode = f.New(true);
			Assert.AreEqual(XMLSchema.Boolean, boolNode.ValueType.DataType);

			//create from string data
			var intNode = (Node<int>)f.New("001", XMLSchema.Integer);
			Assert.AreEqual(1, intNode.Value);

			//same int from .net type
			var intNode2 = f.New(1);
			Assert.IsTrue(ReferenceEquals(intNode, intNode2));
		}

		[TestMethod]
		public void UsingCustomNodeMap()
		{
			//node factory with custom mapping
			var f = new NodeFactory(new FileInfoNodeMap());

			var file = new FileInfo("c:/tmp/test.txt");
            
			//map makes these logically the same node.
			var n1 = f.New(file);
			var n2 = f.New(file.FullName, FileInfoNodeMap.TypeString);
			Assert.IsTrue(ReferenceEquals(n1, n2));

			//map ignores file name case, so same node
			var n3 = f.New("c:/TMP/Test.txt", FileInfoNodeMap.TypeString);
			Assert.IsTrue(ReferenceEquals(n1, n3));
		}

		[TestMethod]
		public void NoMapForLogicallyTheSame()
		{
			var f = new NodeFactory();

			var file1 = new FileInfo("c:/tmp/test.txt");
			var file2 = new FileInfo("C:/TMP/TEST.TXT");

			//builtin object node; no mapping 
			var n1 = f.New(file1);
			var n2 = f.New(file2);
			Assert.IsFalse(ReferenceEquals(n1, n2));
		}

		[TestMethod]
		public void SameNodeDifferentText()
		{
			var f = new NodeFactory(new IntegerNodeMap());
			
			//"001"^^xsd:int
			var n1 = (Node<int>)f.New("001", XMLSchema.Integer);
			//"1"^^xsd:int
			var n2 = (Node<int>) f.New("1", XMLSchema.Integer);

			//same logical string value, same node
			Assert.IsTrue(ReferenceEquals(n1,n2));
		}
	}
}