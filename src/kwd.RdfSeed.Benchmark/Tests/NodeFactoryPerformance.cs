using System.IO;
using BenchmarkDotNet.Attributes;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.TypedNodes;

namespace kwd.RdfSeed.Benchmark.Tests
{
    public class NodeFactoryPerformance
    {
        [Benchmark(Description = "Create 100X4 unique nodes of various types")]
        public void NonRepeatNewNodes()
        {
            var f = new NodeFactory();

            var loops = 100;
            for (var count = 0; count < loops; count++)
            {
                //literal
                f.New("literal-" + count++);

                //object node with value string
                f.New(new FileInfo("c:/tmp/test" + $".{count++}"));

                //custom typed literal
                f.New("at " + count++, "test:customType");

                //uri
                f.Uri("test:uri" + count++);
            }
        }
        
        [Benchmark(Description = "100 new text nodes")]
        public void NewTextNodes()
        {
            var f = new NodeFactory();
            for (var i = 0; i < 100; i++)
                f.Text("i", "en");
        }

        [Benchmark(Description = "100 new uri nodes")]
        public void NewUriNodes()
        {
            var f = new NodeFactory();
            for (var i = 0; i < 100; i++)
                f.Uri("app:"+i);
        }

        [Benchmark(Description = "100 new literal nodes")]
        public void NewLiteralNodes()
        {
            var f = new NodeFactory();
            for (var i = 0; i < 100; i++)
                f.Literal(i.ToString());
        }

        [Benchmark(Description = "100 new double nodes")]
        public void NewDoubleNodes()
        {
            var f = new NodeFactory();
            for (double i = 0; i < 100; i++)
                f.New(i);
        }

        [Benchmark(Description = "100 new typed literal nodes")]
        public void NewTypedLiteralNodes()
        {
            const string myType = "app:testType";
            var f = new NodeFactory();
            for (int i = 0; i < 100; i++)
                f.New(i.ToString(), myType);
        }
    }
}