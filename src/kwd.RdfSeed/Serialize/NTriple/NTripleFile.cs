using System.IO;
using System.Threading;
using System.Threading.Tasks;
using kwd.RdfSeed.Core;

namespace kwd.RdfSeed.Serialize.NTriple
{
    /// <summary>
    /// Triple read / write.
    /// https://www.w3.org/TR/n-triples/
    /// </summary>
    /// <remarks>
    /// todo: accept stream; so can use chained stream.
    /// </remarks>
	public class NTripleFile
    {
	    private readonly FileInfo _dataFile;

        /// <summary>Create a <see cref="NTripleFile"/>.</summary>
        public NTripleFile(FileInfo dataFile)
        {
            _dataFile = dataFile;
        }

        /// <summary>
        /// Write graph <see cref="Quad"/>'s
        /// to provided <see cref="TextWriter"/>.
        /// </summary>
        public static async Task<int> Write(Graph g, TextWriter wr,
	        CancellationToken cancel = default)
        {
	        var writer = new NodeWriter(g);

            var lineCount = 0;
	        foreach (var quad in g.Query)
	        {
		        await writer.PrintQuad(quad, wr, cancel);
		        lineCount++;
	        }

	        return lineCount;
        }

        /// <summary>
        /// Read lines from <see cref="TextReader"/>,
        /// storing result in <see cref="Graph"/>
        /// <paramref name="g"/>.
        /// </summary>
        public static async Task<int> Read(Graph g, TextReader rd, CancellationToken cancel = default)
        {
            cancel.ThrowIfCancellationRequested();
            var parse = new NTripleParse(g);

            var lineCount = 0;

            string? line;
            while ( (line = await rd.ReadLineAsync()) != null)
            {
	            parse.Load(line);

	            lineCount++;
	            cancel.ThrowIfCancellationRequested();
            }
            
            return lineCount;
        }

        /// <summary>Write triple data</summary>
        public async Task<NTripleFile> Write(Graph graph, CancellationToken cancel = default)
        {
            _dataFile.Directory?.Create();

            await using (var wr = new StreamWriter(_dataFile.Open(FileMode.Create)))
	            await Write(graph, wr, cancel);
	            
            return this;
        }

        /// <summary>
        /// Read triples appending the specified <paramref name="graph"/>
        /// </summary>
        public async Task<NTripleFile> Read(Graph graph, CancellationToken cancel = default)
        {
            _dataFile.Refresh();

            using var rd = _dataFile.OpenText();

			await Read(graph, rd, cancel);

            return this;
        }
    }
}