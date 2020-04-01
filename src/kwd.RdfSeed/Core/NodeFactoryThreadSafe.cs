using System;

using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;

namespace kwd.RdfSeed.Core
{
	/// <summary>
	/// Forwards call through to normal node factory,
	/// wrapping a sync lock around them.
	/// </summary>
	public class NodeFactoryThreadSafe : INodeFactory
	{
		private readonly NodeFactory _inner;
		private readonly object _locker = new object();

		/// <summary>Create new <see cref="NodeFactoryThreadSafe"/>.</summary>
		public NodeFactoryThreadSafe(NodeFactory inner)
		{
			_inner = inner;
		}

		/// <inheritdoc />
		public Node<T> New<T>(T value) where T : notnull
		{
			lock (_locker) {return _inner.New(value);}
		}

		/// <inheritdoc />
		public Node New(ReadOnlySpan<char> valueString, ReadOnlySpan<char> typeString)
		{
			lock(_locker) {return _inner.New(valueString, typeString);}
		}

		/// <inheritdoc />
		public UriNode Uri(ReadOnlySpan<char> uriValue)
		{
			lock (_locker) {return _inner.Uri(uriValue);}
		}

		/// <inheritdoc />
		public BlankNode Blank(Node<UriOrBlank> scope)
		{
			lock (_locker) {return _inner.Blank(scope);}
		}

		/// <inheritdoc />
		public BlankNode Blank(Node<UriOrBlank> scope, ReadOnlySpan<char> label)
		{
			lock (_locker){ return _inner.Blank(scope, label);}
		}

		/// <inheritdoc />
		public BlankNode BlankSelf(ReadOnlySpan<char> label)
		{
			lock (_locker){ return _inner.BlankSelf(label);}
		}

		/// <inheritdoc />
		public BlankNode BlankSelf()
		{
			lock (_locker){ return _inner.BlankSelf();}
		}

		/// <inheritdoc />
		public NodeFactoryStats Stats()
		{
			lock (_locker)
			{ return _inner.Stats(); }
		}
	}
}