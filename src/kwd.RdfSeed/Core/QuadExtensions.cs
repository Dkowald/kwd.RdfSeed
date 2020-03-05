using kwd.RdfSeed.Core.Nodes;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Errors;

namespace kwd.RdfSeed.Core
{
    /// <summary>
    /// Extensions for <see cref="Quad"/>.
    /// </summary>
    public static class QuadExtensions
    {
        /// <summary>
        /// Extract the value from the Quad Object node.
        /// </summary>
        /// <exception cref="System.InvalidCastException"></exception>
        /// <exception cref="TypeMustNotBeANode"></exception>
        public static T Value<T>(this Quad quad) where T: notnull
            => quad.Object.Cast<T>().Value;

        /// <summary>
        /// Cast object to <see cref="UriNode"/> and return its value
        /// </summary>
        /// <exception cref="System.InvalidCastException"></exception>
        public static string ValueUri(this Quad quad)
            => ((UriNode) quad.Object).Uri;

        /// <summary>
        /// Cast Object to <see cref="BlankNode"/> and return its label
        /// </summary>
        /// <exception cref="System.InvalidCastException"></exception>
        public static string ValueBlank(this Quad quad)
            => ((BlankNode) quad.Object).Label;
    }
}