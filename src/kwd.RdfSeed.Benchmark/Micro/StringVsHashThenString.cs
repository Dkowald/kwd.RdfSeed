using System.Linq;
using BenchmarkDotNet.Attributes;

// ReSharper disable UnusedVariable

namespace kwd.RdfSeed.Benchmark.Micro
{
    public class StringVsHashThenString
    {
        public class Item
        {
            public Item(string text)
            {
                Text = text;
                Hash = text.GetHashCode();
            }
            public string Text;
            public int Hash;
        }

        public static readonly Item[] SampleData = {
            new Item("a string"),
            new Item("other string"),
            new Item(@"C:\Program Files\Greenshot"),
            new Item(@"C:\Program Files\IIS Express\en-us"),
            new Item(@"C:\Windows\Boot"),
            new Item(@"Team Update Template"),
            new Item(@"7094391103"),
            new Item(" in the middle of text. All "), 
            new Item(@"aivjnlcedvtzewkosfvxceoxttgmxyna"),
            new Item("From your Internet address "),
            new Item("waldo"),
            new Item("sheets containing Lorem Ipsum passages"),
            new Item("below for those interested") 
        };

        public static readonly int WaldoHash = "waldo".GetHashCode();

        [Benchmark(Baseline = true)]
        public void FindByString()
        {
            var found = SampleData.Single(x => x.Text == "waldo");
        }

        [Benchmark]
        public void FindByHashThenString()
        {
            var found = SampleData
                .Single(x => x.Hash == WaldoHash && x.Text == "waldo");
        }
    }
}