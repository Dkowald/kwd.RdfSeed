using System;

namespace kwd.RdfSeed.Errors
{
    /// <summary>
    /// Raised when a Uri value is expected.
    /// </summary>
    public class InvalidUri : Exception
    {
        /// <summary>Create a <see cref="InvalidUri"/>.</summary>
        public InvalidUri(ReadOnlySpan<char> attempted, string? message = null)
            :this(new string(attempted), message){}

        /// <summary>Create a <see cref="InvalidUri"/>.</summary>
        public InvalidUri(string attempted, string? message = null)
            : base(message ?? $"Invalid Uri: '{attempted}'")
        {
            Attempted = attempted;
        }

        /// <summary>The attempted uri value</summary>
        public readonly string Attempted;
    }
}