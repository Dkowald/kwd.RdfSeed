using System.Collections.Generic;
using System.Linq;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Errors;
using kwd.RdfSeed.Util;

namespace kwd.RdfSeed.Query
{
	/// <summary>
	/// Query to select nodes from collection of quads.
	/// </summary>
	public static class QuadNodeQuery
	{
		/// <summary>
		/// Get the first object where the value is of type <typeparamref name="T"/>.
		/// </summary>
		/// <exception cref="TypeMustNotBeANode"></exception>
		/// <exception cref="ValueNotFoundError"></exception>
		public static T Value<T>(this IEnumerable<Quad> quads) where T : notnull
		{
			TypeMustNotBeANode.Verify<T>();
			var quad = quads.FirstOrNull(x => x.Object is Node<T>);

			if(quad is null)throw new ValueNotFoundError();

			return quad.Value<T>();
		}

		/// <summary>
		/// Select all values of Type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The wrapped Node type</typeparam>
		/// <exception cref="TypeMustNotBeANode"></exception>
		public static IEnumerable<T> SelectValues<T>(this IEnumerable<Quad> quads)
			where T : notnull
		=> TypeMustNotBeANode.Check<T>() ? 
			quads.Where(x => x.Object is Node<T>)
				.Select(x => ((Node<T>) x.Object).Value) : 
			throw new TypeMustNotBeANode(typeof(T));

		/// <summary>
		/// Select objects of type <see cref="UriNode"/>.
		/// </summary>
		public static IEnumerable<UriNode> SelectUris(this IEnumerable<Quad> quads) =>
			quads.Select(x => x.Object).OfType<UriNode>();

		/// <summary>
		/// Select objects of type <seealso cref="BlankNode"/>
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<BlankNode> SelectBlanks(this IEnumerable<Quad> quads) =>
			quads.Select(x => x.Object).OfType<BlankNode>();
	}
}