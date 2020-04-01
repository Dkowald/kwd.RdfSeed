using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using kwd.CoreUtil.FileSystem;
using kwd.Rdf.Std;
using kwd.RdfSeed.Builder;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Query;
using kwd.RdfSeed.Serialize.NTriple;
using kwd.RdfSeed.Tests.TestHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Samples.General
{
    /// <summary>
    /// A sample to build and query info about file system.
    /// </summary>
    [TestClass]
    public class DirectoryData
    {
        private readonly FileInfo _dataFile = Files.AppDataDir
            .GetFile(nameof(DirectoryData), "data.nt");

        private readonly DirectoryInfo _rootFolder =
	        new DirectoryInfo("c://program files/");

        [TestMethod]
        public void BuildFileSystemInfo()
        {
            //data store
            var rdf = new RdfData(new NodeFactory());

            //read info
            var g = rdf.GetBlankGraph(nameof(BuildFileSystemInfo));

            g.Update.For(g.Uri(_rootFolder.AsUri()))
	            .With("app:root", out _)
	            .Add(true)
	            .Add(g.Uri(_rootFolder.AsUri()));

            var q = new Queue<DirectoryInfo>();
            q.Enqueue(_rootFolder);

            //limit depth so not too long.
            BreadthFirstReadInfo(q, g.Update, rdf, 2);

            //save data
            _dataFile.EnsureDelete();
            var rdfFile = new NTripleFile(_dataFile);
            rdfFile.Write(g).Wait();

            Assert.IsTrue(_dataFile.Exists());
        }

        [TestMethod]
        public void QueryData()
        {
	        var rdf = new RdfData(new NodeFactory());

            //load data.
            if(!_dataFile.Exists()) 
	            throw new Exception("Data file needs to be pre-built");
            var graph = rdf.GetGraph(rdf.BlankSelf("g1"));
            new NTripleFile(_dataFile).Read(graph).Wait();

            //predicates.
            var root = rdf.Uri("app:root");
            
            var rootFolder = graph.Query
	            .With(root)
                .GetUri()
                .Single();

            var expected = Uri.EscapeUriString(_rootFolder.AsUri().ToString());

            Assert.AreEqual(expected, rootFolder);
        }

        private void BreadthFirstReadInfo(Queue<DirectoryInfo> q, 
            GraphBuilder g, INodeFactory f, int maxDepth = -1)
        {
            var contains = f.Uri("app:contains");
            var itemType = f.Uri("app:type");
            var itemTypeFolder = f.New("folder");
            var itemTypeFile = f.New("file");

            var lastModify = f.Uri(PosixStat.LastModified);
            var size = f.Uri(PosixStat.Size);

            var depth = 0;
            while (q.Count > 0)
            {
                var root = q.Dequeue();

                var subject = f.Uri(root.AsUri());
                try
                {
                    foreach (var file in root.EnumerateFiles())
                    {
                        g.For(subject)
                            .With(contains).To(f.Uri(file.AsUri()))
                            .With(itemType).Add(itemTypeFile)
                            .Then()
                            .With(lastModify).Add(f.New(file.LastWriteTime))
                            .Then()
                            .With(size).Add(f.New(file.Length));
                    }

                    foreach (var dir in root.EnumerateDirectories())
                    {
                        g.For(subject).With(contains).To(f.Uri(dir.AsUri()))
                            .With(itemType).Add(itemTypeFolder);
                    }
                    
                    if (maxDepth > 0 && depth < maxDepth)
                    {
                        foreach (var subDir in root.GetDirectories())
                        {
                            q.Enqueue(subDir);
                        }
                        depth++;
                    }

                    
                }
                catch (UnauthorizedAccessException ex)
                {
                    g.For(subject)
                        .With("app:AccessGranted", out _).Add(false)
                        .Then().With("app:error", out _)
                        .Add(ex.Message)
                        .Add(ex);
                }
            }
        }
    }
}