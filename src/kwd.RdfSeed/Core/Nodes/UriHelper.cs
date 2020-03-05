using System;
using kwd.RdfSeed.Errors;

namespace kwd.RdfSeed.Core.Nodes
{
	/// <summary>Utility to check uri form</summary>
	public static class UriHelper
	{
		/// <summary>
		/// Tests if string is absolute uri.
		/// </summary>
		public static void VerifyIsUri(ReadOnlySpan<char> value)
		{
			if(!char.IsLetter(value[0]))
				throw new InvalidUri(value, "Uri must start with alpha character");

			if(value.IndexOf(':') < 0)
				throw new InvalidUri(value, "Uri must contain a ':'");

		}
	}
}