﻿using System.Collections.Generic;
using System.Linq;

using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Errors;
using kwd.RdfSeed.Util;

namespace kwd.RdfSeed.Query
{
	/// <summary>
	/// Queries to collect quads for objects.
	/// </summary>
	public static class ObjectLikeQuery
	{
		/// <summary>
		/// Quads which point to values for the <paramref name="subject"/>.
		/// Complement of <seealso cref="LinksFor"/>.
		/// </summary>
		/// <remarks>
		/// Quads where the object is not a
		/// <see cref="Node{T}"/>
		/// </remarks>
		public static IEnumerable<Quad> PropertiesFor(this IEnumerable<Quad> self, Node<UriOrBlank> subject)
			=> self.For(subject).Where(x => !x.Object.Is<UriOrBlank>());

		/// <summary>
		/// Subject links to other subjects.
		/// Complement for <see cref="PropertiesFor"/>
		/// </summary>
		public static IEnumerable<Quad> LinksFor(this IEnumerable<Quad> self, Node<UriOrBlank> subject)
			=> self.For(subject).IsType<UriOrBlank>();

		#region Value

		/// <summary>Read the first <typeparamref name="T"/> value.</summary>
		/// <exception cref="TypeMustNotBeANode"></exception>
		/// <exception cref="ValueNotFoundError"></exception>
		public static IReadOnlyList<Quad> Value<T>(this IEnumerable<Quad> self,
			UriNode predicate, out T data) where T:notnull
		{
			var ro = self.Ro();
			
			data = ro.FirstOrNull(x => 
				x.Predicate == predicate &&
				x.Object is Node<T>)?.Object is Node<T> node
				? node.Value : throw new ValueNotFoundError();

			return ro;
		}

		/// <summary>Read the first Node of type <typeparamref name="T"/></summary>
		/// <exception cref="TypeMustNotBeANode"></exception>
		public static IReadOnlyList<Quad> ValueOptional<T>(this IEnumerable<Quad> self,
			UriNode predicate, out Node<T>? data) where T:notnull
		{
			var ro = self.Ro();

			data = ro.FirstOrNull(x => 
				x.Predicate == predicate &&
				x.Object is Node<T>)?.Object as Node<T>;

			return ro;
		}

		#endregion
	}
}