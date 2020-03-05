using System;
using System.Collections.Generic;
using System.Linq;
using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Errors;

namespace kwd.RdfSeed.Query
{
	/// <summary>
	/// Linq friendly extensions to query
	/// the parts of a quad using a readable syntax.
	/// </summary>
	/// <remarks>
	/// todo: Look into linq for IReadonlyList.
	/// It'll be much faster, and this is in-memory so
	/// deferred linq over IEnumerable make less sense.
	/// </remarks>
	public static class QuadQueryExtensions
	{
		#region From
		/// <summary>
		/// Quads From a Graph
		/// </summary>
		public static IEnumerable<Quad> From(this IEnumerable<Quad> self, Func<Node, bool> predicate)
			=> self.Where(x => predicate(x.Graph));

		/// <summary>
		/// Quads From a Graph
		/// </summary>
		public static IEnumerable<Quad> From(this IEnumerable<Quad> self, params Node[] graphs)
			=> self.Where(x => graphs.Contains(x.Graph));

		/// <summary>
		/// Quads From a Graph
		/// </summary>
		public static IEnumerable<Quad> From(this IEnumerable<Quad> self, IEnumerable<Quad> graphs)
			=> self.Where(x => graphs.Select(g => g.Graph).Contains(x.Graph));

		#endregion

		#region For
		/// <summary>
		/// Quads For a Subject
		/// </summary>
		public static IEnumerable<Quad> For(this IEnumerable<Quad> self, Func<Node, bool> subjects)
			=> self.Where(x => subjects(x.Subject));

		/// <summary>
		/// Quads For a Subject
		/// </summary>
		public static IEnumerable<Quad> For(this IEnumerable<Quad> self, params Node[] subjects)
			=> self.Where(x => subjects.Contains(x.Subject));

		/// <summary>
		/// Quads For a Subject
		/// </summary>
		public static IEnumerable<Quad> For(this IEnumerable<Quad> self, IEnumerable<Quad> subjects)
			=> self.Where(x => subjects.Any(s => s.Subject == x.Subject));
		#endregion

		#region With
		/// <summary>
		/// Quads With a Predicate
		/// </summary>
		public static IEnumerable<Quad> With(this IEnumerable<Quad> self, Func<Node, bool> predicate)
			=> self.Where(x => predicate(x.Predicate));

		/// <summary>
		/// Quads With a Predicate
		/// </summary>
		public static IEnumerable<Quad> With(this IEnumerable<Quad> self, params Node[] predicates)
			=> self.Where(x => predicates.Contains(x.Predicate));

		/// <summary>Quads With a Predicate</summary>
		public static IEnumerable<Quad> With(this IEnumerable<Quad> self, IEnumerable<Quad> quads)
			=> self.Where(x => quads.Any(q => q.Predicate == x.Predicate));
		#endregion

		#region IsType
		/// <summary>
		/// Quads where Object is of Type
		/// </summary>
		public static IEnumerable<Quad> IsType(this IEnumerable<Quad> self, Func<Node, bool> predicate)
			=> self.Where(x => predicate(x.Object));

		/// <summary>
		/// Quads where Object is of Type
		/// </summary>
		public static IEnumerable<Quad> IsType(this IEnumerable<Quad> self, params Node[] types)
			=> self.Where(x => types.Any(t => t.ValueType == x.Object.ValueType));

		/// <summary>Quads where Object value is of Type</summary>
		/// <exception cref="TypeMustNotBeANode"></exception>
		public static IEnumerable<Quad> IsType<T>(this IEnumerable<Quad> self, Func<Node, bool>? orPredicate = null)
			where T:notnull
			=> TypeMustNotBeANode.Check<T>()?
				self.Where(x => x.Object is Node<T> 
			                   || orPredicate != null && orPredicate(x.Object)) :
			throw new TypeMustNotBeANode(typeof(T));

		/// <summary>Quads where Object is of Type</summary>
		/// <exception cref="TypeMustNotBeANode"></exception>
		public static IEnumerable<Quad> IsType<T1, T2>(this IEnumerable<Quad> self,
			Func<Node, bool>? orPredicate = null)
			where T1 : notnull where T2 : notnull
		{
			TypeMustNotBeANode.Verify<T1>();
			TypeMustNotBeANode.Verify<T2>();

			return self.Where(x => x.Object is Node<T1> 
			                || x.Object is Node<T2>
			                || orPredicate != null && orPredicate(x.Object));
		}

		/// <summary>Quads where Object is of Type</summary>
		/// <exception cref="TypeMustNotBeANode"></exception>
		public static IEnumerable<Quad> IsType<T1,T2,T3>(this IEnumerable<Quad> self, Func<Node, bool>? orPredicate = null) 
			where T1:notnull where T2: notnull where T3:notnull
		{
			TypeMustNotBeANode.Verify<T1>();
			TypeMustNotBeANode.Verify<T2>();
			TypeMustNotBeANode.Verify<T3>();

			return self.Where(x =>
				x.Object is Node<T1> ||
				x.Object is Node<T2> ||
				x.Object is Node<T3>
				|| orPredicate != null && orPredicate(x.Object));
		}

		/// <summary>Quads where Object is of Type</summary>
		/// <exception cref="TypeMustNotBeANode"></exception>
		public static IEnumerable<Quad> IsType<T1, T2, T3, T4>(this IEnumerable<Quad> self, Func<Node, bool>? orPredicate = null) 
			where T1:notnull where T2: notnull where T3:notnull where T4:notnull
		{
			TypeMustNotBeANode.Verify<T1>();
			TypeMustNotBeANode.Verify<T2>();
			TypeMustNotBeANode.Verify<T3>();
			TypeMustNotBeANode.Verify<T4>();
			return self.Where(x =>
				x.Object is Node<T1> ||
				x.Object is Node<T2> ||
				x.Object is Node<T3> ||
				x.Object is Node<T4> ||
				orPredicate != null && orPredicate(x.Object));
		}

		/// <summary>Quads where Object is of Type <see cref="UriNode"/></summary>
		public static IEnumerable<Quad> IsUri(this IEnumerable<Quad> self)
			=> self.Where(x => x.Object is UriNode);

		/// <summary>Quads where Object is of Type <see cref="BlankNode"/></summary>
		public static IEnumerable<Quad> IsBlank(this IEnumerable<Quad> self)
			=> self.Where(x => x.Object is BlankNode);

		#endregion

		#region IsValue
		/// <summary>
		/// Quads where Object is of Value
		/// </summary>
		public static IEnumerable<Quad> IsValue(this IEnumerable<Quad> self, Func<Node, bool> predicate)
			=> self.Where(x => predicate(x.Object));

		/// <summary>
		/// Quads where Object is of Value
		/// </summary>
		public static IEnumerable<Quad> IsValue(this IEnumerable<Quad> self, params Node[] objects)
			=> self.Where(x => objects.Contains(x.Object));

		/// <summary>
		/// Quads where Object is of Value
		/// </summary>
		public static IEnumerable<Quad> IsValue(this IEnumerable<Quad> self, IEnumerable<Quad> quads)
			=> self.Where(x =>  quads.Any(q => q.Object == x.Object));
		#endregion
	}
}