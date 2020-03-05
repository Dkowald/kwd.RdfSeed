using System;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using kwd.CoreUtil.FileSystem;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Serialize.NTriple;

namespace kwd.RdfSeed.Benchmark.Tests
{
	public class NTripleReadWrite
	{
		private static readonly DirectoryInfo Project =
			new DirectoryInfo(
				Environment.GetEnvironmentVariable(Program.SourceFolderRoot)??
				throw new Exception("Need Project env for data files path"));

		private static readonly FileInfo Sample1 = Project
				.GetFile("Test_Data", "Sample1.nt");

		private static readonly DirectoryInfo TempFileDir = 
			Project.GetFolder("App_Data", "Tmp").EnsureExists();

		private Graph _sample1Data;
		private VDS.RDF.Graph _sample1VdsData;

		[GlobalSetup]
		public void Write2000_Setup()
		{
			var f = new NodeFactory();
			var rdf = new RdfData(f);

			_sample1Data = rdf.GetBlankGraph();
			new NTripleFile(Sample1)
				.Read(_sample1Data).Wait();

			_sample1VdsData = new VDS.RDF.Graph();
			using(var rd = Sample1.OpenText())
				new VDS.RDF.Parsing.NTriplesParser()
					.Load(_sample1VdsData, rd);
		}

		[GlobalCleanup]
		public void TearDown()
		{
			TempFileDir.EnsureDelete();
		}

		[Benchmark]
		public void Read2000()
		{
			var f = new NodeFactory();
			var rdf = new RdfData(f);

			var g = rdf.GetBlankGraph();
			new NTripleFile(Sample1).Read(g).Wait();
		}

		[Benchmark]
		public void Read2000VDS()
		{
			var g = new VDS.RDF.Graph();

			var parse = new VDS.RDF.Parsing.NTriplesParser();

			using(var rd = Sample1.OpenText())
				parse.Load(g, rd);
		}

		/// <summary>
		/// Use per-iteration file;
		/// to avoid file locks.
		/// </summary>
		[IterationSetup(Target = nameof(Write2000))]
		public void Write2000_TmpFile()
		{
			var count = TempFileDir.GetFolder("kBox").EnsureExists()
				.EnumerateFiles().Count();
			_tmpFile = TempFileDir.GetFile("OutTemp." + count + ".nt");
		}
		
		private FileInfo _tmpFile;
		
		[Benchmark]
		public void Write2000()
		{
			new NTripleFile(_tmpFile).Write(_sample1Data);
		}

		[IterationSetup(Target = nameof(WriteVDS2000))]
		public void WriteVDS2000_TmpFile()
		{
			var count = TempFileDir.GetFolder("VDS")
				.EnsureExists().EnumerateFiles().Count();
			_tmpVDSFile = TempFileDir.GetFile("OutTemp." + count + ".nt");
		}

		private FileInfo _tmpVDSFile;

		[Benchmark]
		public void WriteVDS2000()
		{
			var writer = new VDS.RDF.Writing.NTriplesWriter();
			writer.Save(_sample1VdsData, _tmpVDSFile.FullName);
		}
	}
}