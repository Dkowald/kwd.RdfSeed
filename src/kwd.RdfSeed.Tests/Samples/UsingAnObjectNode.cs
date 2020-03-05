using System;
using System.IO;
using System.Linq;

using kwd.CoreUtil.FileSystem;
using kwd.RdfSeed.Builder;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Query;
using kwd.RdfSeed.Tests.TestHelpers;
using kwd.RdfSeed.Util;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Samples
{
    /// <summary>
    /// Samples using object typed node(s).
    /// </summary>
    [TestClass]
    public class UsingAnObjectNode
    {
        private readonly IRdfData _rdf;

        private readonly UriNode _fileNode;
        private readonly Node<UriOrBlank> _gId;
        private readonly Node<UriOrBlank> _id;

        public UsingAnObjectNode()
        {
            _rdf = RdfDataFactory.CreateNoLock();

            _fileNode = _rdf.Uri("x:/fileIno");
            _gId = _rdf.BlankGraph();
            _id = _rdf.Blank(_gId, "myFile");
        }

        public void SetupData()
        {
            var targetFile = _rdf.New(
                (IFileInfo)new PhysicalFileInfo(Files.AppDataDir.GetFile("test")));
            
            _rdf.Assert(_gId, _id, _fileNode, targetFile);
        }

        [TestMethod]
        public void UseAnObjectAsNodeValue()
        {
            SetupData();
            
            //Get the triple.
            var g = _rdf.GetGraph(_gId);
            var fileNode = g.Query.For(_id).With(_fileNode)
                .SingleOrDefault();

            if (fileNode is null)
            {
                Assert.Fail("Expected to get the property");
                throw new Exception();
            }
            
            //so i have a IFileInfo
            var fileValue = fileNode.Object as Node<IFileInfo>
                                ?? throw new Exception("Expected file node");

            //infer extra data: Its not a Directory
            if(!fileValue.Value.IsDirectory)
                //So add a FileInfo node.
                g.Assert(_id, _fileNode, _rdf.New(new FileInfo(fileValue.Value.PhysicalPath)));

            //then later get it.
            var fileInfo = g.Query.For(_id)
                .With(_fileNode)
                .Select(x => x.Object)
                .OfType<Node<FileInfo>>()
                .Single();

            var file = fileInfo.Value;
            Assert.AreEqual("test", file.Name, "Only need to add converter once");
        }

        [TestMethod]
        public void AddMissingValue()
        {
	        var rdf = new RdfData(new NodeFactory());
	        var g = rdf.GetBlankGraph();

	        var file = new FileInfo("c:temp/a file");

            //Have a literal for the path.
	        g.Update
		        .For("app:test", out var test)
		        .With("app:path", out var aPath).Assert(file.FullName);

            //try find FileInfo for path.
            var f = rdf.Query.From(g.Id)
	            .For(test).With(aPath)
	            .SelectValues<FileInfo>()
	            .FirstOrNull();

            Assert.IsNull(f, "don't have file info yet.");

            //no value; lets make it.
            f = new FileInfo(
            rdf.Query.From(g.Id)
	            .For(test).With(aPath)
	            .Value<string>());

            g.Assert(rdf.Uri("app:test"),
	            rdf.Uri("app:path"), rdf.New(f));
        
            //now it is just in the graph.
            f = g.Query.For(g, "app:test")
	            .With(g, "app:path")
	            .SelectValues<FileInfo>()
	            .SingleOrNull();

            Assert.IsNotNull(f, "Now have typed value");
        }
    }
}