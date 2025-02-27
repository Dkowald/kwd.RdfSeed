﻿using System;
using System.Collections.Generic;

using kwd.RdfSeed.Builder;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;

//Don't lock INodeFactory members; they are only forwards to 
// inner factory. The additional lock adds nothing, but wasted CPU.
// ReSharper disable InconsistentlySynchronizedField

namespace kwd.RdfSeed.Core
{
	/// <summary>
	/// Thread safe <see cref="IRdfData"/>;
	/// assuming original <see cref="INodeFactory"/> is thread safe.
	/// </summary>
	/// <remarks>
	/// Calls are wrapped in lock statements, 
	/// and forwarded to internal <see cref="RdfData"/>.
	///
	/// Except for <see cref="INodeFactory"/> methods
	/// these are forwarded, without any additional locking.
	/// </remarks>
	public class RdfDataThreadLock : IRdfData
	{
		private readonly RdfData _inner;
		private readonly object _locker = new object();

		/// <summary>
		/// Create new <see cref="RdfDataThreadLock"/>.
		/// </summary>
		public RdfDataThreadLock(RdfData inner)
		{
			_inner = inner;
		}

		#region IRdfData

		/// <inheritdoc />
		public RdfBuilder Update => new RdfBuilder(this);

		/// <inheritdoc />
		public IReadOnlyList<Quad> Query
		{ get{ lock (_locker){return _inner.Query;}} }
		
		/// <inheritdoc />
		public UriNode System => _inner.System;

		/// <inheritdoc />
		public UriNode Default => _inner.Default;

		/// <inheritdoc />
		public Node<UriOrBlank>[] GraphIds 
		{ get{lock (_locker){return _inner.GraphIds;}} }

		/// <inheritdoc />
		public Quad[] GraphData(params Node<UriOrBlank>[] graphIds)
		{
			lock (_locker)
			{ return _inner.GraphData(graphIds); }
		}

		/// <inheritdoc />
		public IRdfData Assert(Node<UriOrBlank> graph, 
			Node<UriOrBlank> sub, UriNode predicate, Node val, out Quad q)
		{
			lock (_locker)
			{
				_inner.Assert(graph, sub, predicate, val, out q);
				return this;
			}
		}

		/// <inheritdoc />
		public Graph GetGraph(Node<UriOrBlank> graphId, 
			params Node<UriOrBlank>[] other)
		{
			lock (_locker)
			{ return new Graph(this, graphId, other); }
		}

		/// <inheritdoc />
		public int Retract(params Quad[] quads)
		{
			lock (_locker)
			{ return _inner.Retract(quads); }
		}

		/// <inheritdoc />
		public int Replace(IEnumerable<Quad> newQuads, IEnumerable<Quad> oldQuads)
		{
			lock (_locker)
			{ return _inner.Replace(newQuads, oldQuads); }
		}
		#endregion

		#region INodeFactory

		/// <inheritdoc />
		public BlankNode Blank(Node<UriOrBlank> scope)
			=> _inner.Blank(scope);

		/// <inheritdoc />
		public BlankNode Blank(Node<UriOrBlank> scope, ReadOnlySpan<char> label)
			=> _inner.Blank(scope, label);

		/// <inheritdoc />
		public Node<T> New<T>(T value) where T : notnull
			=> _inner.New(value);

		/// <inheritdoc />
		public Node New(ReadOnlySpan<char> valueString, ReadOnlySpan<char> typeString)
			=> _inner.New(valueString, typeString);

		/// <inheritdoc />
		public UriNode Uri(ReadOnlySpan<char> uriValue)
			=> _inner.Uri(uriValue);

		/// <inheritdoc />
		public BlankNode BlankSelf(ReadOnlySpan<char> label)
			=> _inner.BlankSelf(label);

		/// <inheritdoc />
		public BlankNode BlankSelf()
			=> _inner.BlankSelf();

		/// <inheritdoc />
		public NodeFactoryStats Stats() => _inner.Stats();
		
		#endregion
	}
}