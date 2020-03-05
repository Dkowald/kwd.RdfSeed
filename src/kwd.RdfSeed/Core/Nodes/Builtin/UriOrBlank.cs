using kwd.RdfSeed.Errors;

namespace kwd.RdfSeed.Core.Nodes.Builtin
{
    /// <summary>
    /// Used to represent a node that can be either a blank or a uri.
    /// </summary>
    public class UriOrBlank
    {
        /// <summary>
        /// Create uri subject node.
        /// </summary>
        /// <exception cref="InvalidUri"></exception>
        public UriOrBlank(string uriValue)
        {
	        uriValue = uriValue.Trim();
	        UriHelper.VerifyIsUri(uriValue);

            Uri = uriValue;
        }

        /// <summary>
        /// Create blank node (null graph for self scoped)
        /// </summary>
        public UriOrBlank(Node<UriOrBlank>? graph, string label)
        {
	        Label = label.Trim();
	        Label = Label.StartsWith("_:") ? Label.Substring(2) : Label;
	        
            Scope = graph;
        }

        /// <summary>
        /// Uri value (if uri node)
        /// </summary>
        public readonly string? Uri;

        /// <summary>
        /// Blank node label
        /// </summary>
        public readonly string? Label;
        
        /// <summary>
        /// Blank node scope
        /// </summary>
        public readonly Node<UriOrBlank>? Scope;
    }
}