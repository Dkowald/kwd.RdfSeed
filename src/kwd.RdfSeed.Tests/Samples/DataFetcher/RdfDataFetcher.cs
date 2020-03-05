using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using kwd.CoreUtil.FileSystem;
using kwd.RdfSeed.Builder;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Query;
using kwd.RdfSeed.Serialize.NTriple;
using kwd.RdfSeed.Tests.Samples.Rdf;
using kwd.RdfSeed.Util;

namespace kwd.RdfSeed.Tests.Samples.DataFetcher
{
	/// <summary>
	/// Loads graph data from data-files.
	/// </summary>
	public class RdfDataFetcher : IUseRdf
	{
		private readonly IRdfData _rdf;
		
		public readonly UriNode BackingFile;

		IRdfData IUseRdf.Rdf => _rdf;

		public RdfDataFetcher(IRdfData rdf)
		{
			_rdf = rdf;
			BackingFile = _rdf.Uri("sys:/backingFile");

			if(!_rdf.Mappings.Any(x => x is FileInfoNodeMap))
				throw new Exception(
					$"Must include node map {nameof(FileInfoNodeMap)}");
		}

		/// <summary>
		/// List of currently loaded files.
		/// </summary>
		public IReadOnlyCollection<Node<FileInfo>> List()
		{
			return _rdf.GetSystem().SelfOnly()
				.Query.With(BackingFile)
				.IsType<FileInfo>()
				.Select(x => (Node<FileInfo>) x.Object)
				.ToArray();
		}

		/// <summary>
		/// (re)loads file data; existing graph data is overwritten.
		/// </summary>
		/// <returns>
		/// Self graph for loaded data.
		/// </returns>
		public async Task<Graph> Load(Node<FileInfo> file)
		{
			var id = _rdf.Uri(file.Value.AsUri());

			var g = _rdf.GetFullGraph(id);

			g.Clear();
			await new NTripleFile(file.Value)
				.Read(g);

			_rdf.GetSystem()
				.Update
				.For(id).With(BackingFile)
				.Set(file);

			return g;
		}

		/// <summary>
		/// Loads all specified files,
		/// returning a graph with loaded data.
		/// </summary>
		public async Task<Graph> Load(params Node<FileInfo>[] files)
		{
			var loading = new List<Task<Graph>>();
			foreach (var file in files)
			{
				loading.Add(Load(file));
			}

			await Task.WhenAll(loading);

			var g = _rdf.GetGraph(
				loading.First().Result.Id,
				loading.Skip(1).Select(x => x.Result.Id));
			return g;
		}

		/// <summary>Try get loaded graph</summary>
		public Graph? Get(Node<FileInfo> file)
		{
			var id = _rdf.GetSystem()
				.Query.With(BackingFile)
				.IsValue(file)
				.FirstOrDefault()
				?.Subject;

			return id is null ? null : _rdf.GetGraph(id, _rdf.GraphIds);
		}

		/// <summary>Try get backing file for loaded graph</summary>
		public Node<FileInfo>? Get(Node<UriOrBlank> graphId)
		{
			var val = _rdf.GetSystem().Query
				.For(graphId)
				.With(BackingFile)
				.IsType<FileInfo>()
				.FirstOrNull()?.Object
				as Node<FileInfo>;

			return val;
		}

		/// <summary>Save previously loaded graph </summary>
		/// <exception cref="ErrorNoBackingFile"></exception>
		public async Task<RdfDataFetcher> Save(Node<UriOrBlank> graphId)
		{
			var sys = _rdf.GetSystem().SelfOnly();

			var file = sys.Query
				.For(graphId)
				.With(BackingFile)
				.IsType<FileInfo>()
				.FirstOrNull()
				?.Object as Node<FileInfo> 
				?? throw new ErrorNoBackingFile(graphId);

			await new NTripleFile(file.Value).Write(_rdf.GetSelfGraph(graphId));

			sys.Update.For(graphId)
				.With(BackingFile)
				.Set(file);

			return this;
		}

		/// <summary>
		/// Disconnect file(s) from fetcher (existing data remains).
		/// </summary>
		public RdfDataFetcher Close(params Node<FileInfo>[] files)
		{
			var quads = GetFileQuads(files);
			_rdf.Retract(quads);

			return this;
		}

		/// <summary>
		/// Closes files, and removes any associated graph(s).
		/// </summary>
		public RdfDataFetcher Purge(params Node<FileInfo>[] files)
		{
			var quads = GetFileQuads(files);
			
			foreach ( var g in quads.Select(x => _rdf.GetSelfGraph(x.Subject)))
			{ g.Clear(); }

			_rdf.Retract(quads);

			return this;
		}

		private IReadOnlyCollection<Quad> GetFileQuads(params Node<FileInfo>[] files)
		{
			var sys = _rdf.GetSystem().SelfOnly();

			var quads = sys.Query
				.With(BackingFile)
				.IsValue(files.Contains)
				.ToArray();

			return quads;
		}
	}
}
