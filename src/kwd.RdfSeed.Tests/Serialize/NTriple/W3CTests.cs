using System;
using System.Linq;
using kwd.CoreUtil.FileSystem;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Serialize.NTriple;
using kwd.RdfSeed.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.RdfSeed.Tests.Serialize.NTriple
{
    /// <summary>
    /// Test cases from
    /// https://www.w3.org/2013/N-TriplesTests/
    /// </summary>
    [TestClass]
    public class W3CTests
    {
        [TestMethod]
        public void Positive()
        {
            var f = new NodeFactory();
            var rdf = new RdfData(f);

            var fileRoot = Files.TestDataDir.GetFolder("w3c");

            var files = fileRoot.EnumerateFiles("*.nt")
                .Where(x => !x.Name.Contains("-bad-"));

            var idx = 0;
            foreach (var item in files.Skip(idx))
            {
                new NTripleFile(item)
                    .Read(rdf.GetBlankGraph($"Test-{idx}")).Wait();

                idx++;
            }
        }

        [TestMethod]
        public void Negative()
        {
            var f = new NodeFactory();
            var rdf = new RdfData(f);

            var fileRoot = Files.TestDataDir.GetFolder("w3c");

            var files = fileRoot.EnumerateFiles("*.nt")
                .Where(x => x.Name.Contains("-bad-"));
             
            var idx = 18;
            foreach (var item in files.Skip(idx))
            {
	            Exception? error = null;
	            try
	            {
		            new NTripleFile(item)
			            .Read(rdf.GetBlankGraph($"BadTest-{idx}")).Wait();
	            }
	            catch (Exception ex)
	            {
		            error = ex;
	            }

                Assert.IsNotNull(error, $"Negative test : {item.Name} failed");

                idx++;
            }
        }
    }
}