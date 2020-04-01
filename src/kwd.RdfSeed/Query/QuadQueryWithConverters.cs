using System.Collections.Generic;
using System.Linq;

using kwd.RdfSeed.Core;
using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.TypedNodes;

namespace kwd.RdfSeed.Query
{
	/// <summary>
	/// Queries that convert data to nodes.
	/// </summary>
	public static class QuadQueryWithConverters
	{
		#region For
		/// <summary>Quads For a URI Subject(s) </summary>
		public static IEnumerable<Quad> For(
			this IEnumerable<Quad> self,
			IBasicNodeFactory f,
			params string[] uriSubjects)
		{
			var nodes = uriSubjects.Select(x => f.Uri(x));
			return self.Where(x => nodes.Contains(x.Subject));
		}

		/// <summary>Quads For a URI Subject(s) </summary>
		public static IEnumerable<Quad> For(
			this IEnumerable<Quad> self,
			IBasicNodeFactory f,
			IEnumerable<string> uriSubjects)
		{
			var nodes = uriSubjects.Select(x => f.Uri(x));
			return self.Where(x => nodes.Contains(x.Subject));
		}

		#endregion

		#region With
		/// <summary>Quads With a Predicate</summary>
		public static IEnumerable<Quad> With(
			this IEnumerable<Quad> self, 
			IBasicNodeFactory f,
			params string[] uriValues)
		{
			var nodes = uriValues.Select(x => f.Uri(x));
			return self.Where(x => nodes.Contains(x.Predicate));
		}

		/// <summary>Quads With a Predicate</summary>
		public static IEnumerable<Quad> With(
			this IEnumerable<Quad> self, 
			IBasicNodeFactory f,
			IEnumerable<string> uriValues)
		{
			var nodes = uriValues.Select(x => f.Uri(x));
			return self.Where(x => nodes.Contains(x.Predicate));
		}

		#endregion

		#region IsType
		/// <summary>
		/// Quads where <see cref="Quad.Object"/> is
		/// <see cref="Text"/> with specified language.
		/// </summary>
		public static IEnumerable<Quad> IsTypeTextLang(
			this IEnumerable<Quad> self,
			IBasicNodeFactory f,
			params string[] language)
		{
			var nodes = language.Select(x => f.Text("", x));
			return self.Where(x =>
				x.Object is Node<Text> txt &&
				nodes.Any(v => v.Value.Language == txt.Value.Language));
		}

		#endregion

		#region IsValue
		/// <summary>Quads where Object is one of the literal values</summary>
		public static IEnumerable<Quad> IsValue(
			this IEnumerable<Quad> self, 
			IBasicNodeFactory f,
			IEnumerable<string> literals)
		{
			var nodes = literals.Select(x => f.New(x));
			return self.Where(x => nodes.Contains(x.Object));
		}

		/// <summary>Quads where Object is one of the literal values</summary>
		public static IEnumerable<Quad> IsValue(
			this IEnumerable<Quad> self, 
			IBasicNodeFactory f,
			params string[] literals)
		{
			var nodes = literals.Select(x => f.New(x));
			return self.Where(x => nodes.Contains(x.Object));
		}

		/// <summary>
		/// Quads that are uri's 
		/// </summary>
		public static IEnumerable<Quad> IsValueUri(
			this IEnumerable<Quad> self, 
			IBasicNodeFactory f, IEnumerable<string> uris)
		{
			var nodes = uris.Select(x => f.Uri(x));
			return self.Where(x => nodes.Contains(x.Object));
		}

		#endregion
	}
}