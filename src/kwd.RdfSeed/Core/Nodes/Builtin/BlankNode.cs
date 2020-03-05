using System;

namespace kwd.RdfSeed.Core.Nodes.Builtin
{
    /// <summary>
    /// A blank node <see cref="UriOrBlank"/>.
    /// </summary>
    public class BlankNode : Node<UriOrBlank>
    {
        /// <summary>Get normalized value string for <see cref="BlankNode"/>.</summary>
        public static ReadOnlySpan<char> ValueStringFromLabel(ReadOnlySpan<char> value)
        {
	        var trimmed = value.Trim();
            return value.StartsWith("_:") ? trimmed : "_:" + new string(trimmed);
        }

        /// <summary>Get normalized value string for blank node.</summary>
        public static string ValueStringFromLabel(string value)
        {
	        var trimmed = value.Trim();
            return value.StartsWith("_:") ? trimmed : "_:" + trimmed;
        }

        /// <summary>Create new <see cref="BlankNode"/></summary>
        public BlankNode(UriOrBlankMap b, UriOrBlank data)
	        : base(b, data, ValueStringFromLabel(data.Label ?? throw new Exception("Must be blank with label")))
        {}
        
        /// <summary>Get <see cref="BlankNode"/> Label.</summary>
        public string Label => 
            Value.Label ??
            throw new Exception("Blank nodes must have label");

        /// <summary>Get <see cref="BlankNode"/> Scope.</summary>
        public Node<UriOrBlank>? Scope =>Value.Scope;
    }
}