using System;

using kwd.RdfSeed.Core.Nodes.Builtin;

namespace kwd.RdfSeed.Core
{
    /// <summary>
    /// Extensions for <see cref="INodeFactory"/>
    /// </summary>
    public static class BasicNodeFactoryExtensions
    {
	    /// <summary>
	    /// Create new <see cref="UriNode"/> from <see cref="Uri"/>.
	    /// </summary>
	    public static UriNode Uri(this IBasicNodeFactory self, Uri uri)
		    => self.Uri(uri.ToString());
    }
}