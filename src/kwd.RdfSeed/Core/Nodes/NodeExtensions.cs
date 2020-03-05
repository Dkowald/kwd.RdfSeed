using System;
using kwd.RdfSeed.Core.Nodes.Builtin;
using kwd.RdfSeed.Errors;

namespace kwd.RdfSeed.Core.Nodes
{
    /// <summary>Extensions for <see cref="Node"/>.</summary>
    public static class NodeExtensions
    {
	    /// <summary>
	    /// Try convert node to typed node.
	    /// </summary>
	    /// <exception cref="TypeMustNotBeANode"></exception>
	    public static Node<T>? As<T>(this Node node) where T : notnull
	    {
		    TypeMustNotBeANode.Verify<T>();
		    return node as Node<T>;
	    }

        /// <summary>Try cast to uri node</summary>
        public static UriNode? AsUriNode(this Node node)
            => node as UriNode;

        /// <summary>Cast to a Uri node</summary>
        /// <exception cref="InvalidCastException"></exception>
        public static UriNode CastUriNode(this Node node)
	        => (UriNode) node;

        /// <summary>Try get node as <see cref="BlankNode"/></summary>
        public static BlankNode? AsBlankNode(this Node node)
            => node as BlankNode;

        /// <summary>Get node value as type.</summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="TypeMustNotBeANode"></exception>
        public static Node<T> Cast<T>(this Node node) where T : notnull
			=> node.As<T>() ??
			   throw new InvalidCastException($"Node is not of type {typeof(T).Name}");

        /// <summary>Get value from node.</summary>
        /// <exception cref="InvalidCastException">
        /// Raise if node is not of type <typeparamref name="T"/>
        /// </exception>
        public static T Value<T>(this Node node) where T : notnull
	        => node.Cast<T>().Value;

        /// <summary>
        /// Test if node is of type <typeparamref name="T"/>.
        /// </summary>
        public static bool Is<T>(this Node node)where T: notnull
            => !(node.As<T>() is null);

        /// <summary>
        /// Get the node <see cref="Node.ValueString"/>,
        /// raising exception if not exist.
        /// </summary>
        /// <exception cref="ValueStringIsNull"></exception>
        public static string GetValueString(this Node node)
            => node.ValueString ??
               throw new ValueStringIsNull();

        #region Subject nodes
        /// <summary>
        /// True if <seealso cref="UriOrBlank"/> node has self as scope.
        /// </summary>
        public static bool IsSelfScoped(this Node<UriOrBlank> subject)
            => subject.Value.Scope is null;

        #endregion
    }
}