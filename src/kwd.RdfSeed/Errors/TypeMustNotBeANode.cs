using System;
using kwd.RdfSeed.Core.Nodes;

namespace kwd.RdfSeed.Errors
{
	/// <summary>
	/// Raised if consumer tries to use a <see cref="Node"/>
	/// where the typeof <see cref="Node{T}.Value"/> is expected.
	/// </summary>
	public class TypeMustNotBeANode : Exception
	{
		private static string BuildMessage(Type t) =>
			$"Type {t.Name} cannot be assignable to a {nameof(Node)} use the wrapped type instead";

		/// <summary>
		/// Return true if <paramref name="t"/> is Not a <see cref="Node{T}"/>.
		/// </summary>
		public static bool Check(Type t) => 
			!typeof(Node).IsAssignableFrom(t);

		/// <summary>
		/// Return true if <typeparamref name="T"/> is Not a <see cref="Node{T}"/>.
		/// </summary>
		public static bool Check<T>() => Check(typeof(T));

		/// <summary>
		/// Verify <paramref name="t"/> is not a <see cref="Node{T}"/>.
		/// </summary>
		/// <exception cref="TypeMustNotBeANode"></exception>
		public static bool Verify(Type t)
		{
			return Check(t) ? true : 
				throw new TypeMustNotBeANode(t);
		}

		/// <summary>
		/// Verify <typeparamref name="T"/> is not a <see cref="Node{T}"/>.
		/// </summary>
		/// <exception cref="TypeMustNotBeANode"></exception>
		public static bool Verify<T>() => Verify(typeof(T));

		/// <summary>
		/// Create new <see cref="TypeMustNotBeANode"/>.
		/// </summary>
		public TypeMustNotBeANode(Type t)
			: base(BuildMessage(t))
		{
			Attempted = t;
		}

		/// <summary>The attempted type.</summary>
		public readonly Type Attempted;
	}
}