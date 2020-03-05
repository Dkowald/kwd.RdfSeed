using System;
using kwd.RdfSeed.Errors;

namespace kwd.RdfSeed.Core.Nodes.Builtin
{
    /// <summary>
    /// A uri <seealso cref="UriOrBlank"/> node.
    /// </summary>
    public class UriNode : Node<UriOrBlank>
    {
	    /// <summary>Create  a new <see cref="UriNode"/></summary>
	    /// <exception cref="InvalidUri"></exception>
	    public UriNode(UriOrBlankMap b, UriOrBlank data)
		    : base(b, data, data.Uri ?? throw new Exception("Data must be for a uri"))
	    {}

        /// <summary>The URI Value</summary>
        public string Uri =>
            Value.Uri ??
            throw new Exception("Uri node must have uri value");
    }
}