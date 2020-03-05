using System;
using System.Collections.Generic;
using System.Linq;

namespace kwd.RdfSeed.Util
{
	/// <summary>
	/// Linq extensions that explicitly return null
	/// </summary>
	/// <remarks>
	/// Helpful when using C#8 nullable,
	/// and compiler fails to correctly identify reference type my be null.
	/// </remarks>
	public static class LinqWithNulls
	{
		/// <summary>
		/// Returns the first element of a sequence, or null if the sequence contains no elements.
		/// </summary>
		public static T? FirstOrNull<T>(this IEnumerable<T> source)
			where T : class 
			=> source.FirstOrDefault();

		/// <summary>
		/// Returns the first element of a sequence, or null if the sequence contains no elements.
		/// </summary>
		public static TSource? FirstOrNull<TSource>(
			this IEnumerable<TSource> source, 
			Func<TSource, bool> predicate)
			where TSource : class 
			=> source.FirstOrDefault(predicate);

		/// <summary>Single item or NULL.</summary>
		public static T? SingleOrNull<T>(this IEnumerable<T> source)
			where T : class => source.SingleOrDefault();

		/// <summary>Single item or NULL.</summary>
		public static T? SingleOrNull<T>(
			this IEnumerable<T> source, 
			Func<T, bool> predicate)
			where T : class => source.SingleOrDefault(predicate);
	}
}