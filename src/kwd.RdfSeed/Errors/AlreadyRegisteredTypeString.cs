using System;

using kwd.RdfSeed.Core.Nodes;

namespace kwd.RdfSeed.Errors
{
	/// <summary>
	/// Raised if try to register mapping for native type that already exists.
	/// </summary>
	public class AlreadyRegisteredNativeType : Exception
	{
		/// <summary>
		/// Create a new <see cref="AlreadyRegisteredNativeType"/>.
		/// </summary>
		public AlreadyRegisteredNativeType(Type attempted)
			: base($"Type {attempted.Name} already registered")
		{
			Attempted = attempted;
		}

		/// <summary>The attempted native type</summary>
		public readonly Type Attempted;
	}

	/// <summary>
	/// Raised if attempt to register a <see cref="NodeMap"/>
	/// with the same <see cref="NodeMap.DataType"/>.
	/// </summary>
	public class AlreadyRegisteredTypeString : Exception
	{
		/// <summary>
		/// Create new <see cref="AlreadyRegisteredTypeString"/>
		/// </summary>
		public AlreadyRegisteredTypeString(string attempted)
		:base($"TypeString {attempted} already registered")
		{
			Attempted = attempted;
		}

		/// <summary>The attempted type string.</summary>
		public readonly string Attempted;
	}
}
